﻿using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using AudioSwitcher.CoreAudio.Internal;
using AudioSwitcher.CoreAudio.Internal.Interfaces;
using AudioSwitcher.CoreAudio.Threading;

namespace AudioSwitcher.CoreAudio;

internal sealed class CoreAudioSessionController : IAudioSessionController, IAudioSessionNotification, IDisposable
{
    private readonly IAudioSessionManager2 _audioSessionManager;

    private readonly CoreAudioDevice _device;
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private readonly IDisposable _processTerminatedSubscription;

    private readonly Broadcaster<IAudioSession> _sessionCreated;
    private readonly Broadcaster<string> _sessionDisconnected;

    private List<CoreAudioSession> _sessionCache;

    public CoreAudioSessionController(CoreAudioDevice device, IAudioSessionManager2 audioSessionManager)
    {
        if (audioSessionManager == null)
            throw new ArgumentNullException(nameof(audioSessionManager));

        ComThread.Assert();

        _device = device;
        _audioSessionManager = audioSessionManager;
        _audioSessionManager.RegisterSessionNotification(this);
        _sessionCache = new List<CoreAudioSession>(0);

        _sessionCreated = new Broadcaster<IAudioSession>();
        _sessionDisconnected = new Broadcaster<string>();

        RefreshSessions();

        _processTerminatedSubscription = ProcessMonitor.ProcessTerminated.Subscribe(processId => RemoveSessions(_sessionCache.Where(x => x.ProcessId == processId)));

    }

    public IObservable<IAudioSession> SessionCreated => _sessionCreated.AsObservable();

    public IObservable<string> SessionDisconnected => _sessionDisconnected.AsObservable();

    public IEnumerable<IAudioSession> All()
    {
        var acquiredLock = _lock.AcquireReadLockNonReEntrant();

        try
        {
            return _sessionCache.ToList();
        }
        finally
        {
            if (acquiredLock)
                _lock.ExitReadLock();
        }
    }

    public Task<IEnumerable<IAudioSession>> AllAsync()
    {
        return Task.FromResult(All());
    }

    public IEnumerable<IAudioSession> ActiveSessions()
    {
        var acquiredLock = _lock.AcquireReadLockNonReEntrant();

        try
        {
            return _sessionCache.Where(x => x.SessionState == AudioSessionState.Active).ToList();
        }
        finally
        {
            if (acquiredLock)
                _lock.ExitReadLock();
        }
    }

    public Task<IEnumerable<IAudioSession>> ActiveSessionsAsync()
    {
        return Task.FromResult(ActiveSessions());
    }

    public IEnumerable<IAudioSession> InactiveSessions()
    {
        var acquiredLock = _lock.AcquireReadLockNonReEntrant();

        try
        {
            return _sessionCache.Where(x => x.SessionState == AudioSessionState.Inactive).ToList();
        }
        finally
        {
            if (acquiredLock)
                _lock.ExitReadLock();
        }
    }

    public Task<IEnumerable<IAudioSession>> InactiveSessionsAsync()
    {
        return Task.FromResult(InactiveSessions());
    }

    public IEnumerable<IAudioSession> ExpiredSessions()
    {
        var acquiredLock = _lock.AcquireReadLockNonReEntrant();

        try
        {
            return _sessionCache.Where(x => x.SessionState == AudioSessionState.Expired).ToList();
        }
        finally
        {
            if (acquiredLock)
                _lock.ExitReadLock();
        }
    }

    public Task<IEnumerable<IAudioSession>> ExpiredSessionsAsync()
    {
        return Task.FromResult(ExpiredSessions());
    }

    public IEnumerator<IAudioSession> GetEnumerator()
    {
        return All().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    int IAudioSessionNotification.OnSessionCreated(IAudioSessionControl sessionControl)
    {
        Task.Run(async () => await CreateSession(sessionControl));

        return 0;
    }

    private async Task CreateSession(IAudioSessionControl sessionControl)
    {
        var managedSession = await ComThread.BeginInvoke(() => CacheSessionWrapper(sessionControl)).ConfigureAwait(false);

        if (managedSession != null)
            OnSessionCreated(managedSession);
    }

    public void Dispose()
    {
        _processTerminatedSubscription.Dispose();

        _sessionCreated.Dispose();
        _sessionDisconnected.Dispose();
        _lock.Dispose();

        if (_sessionCache != null)
        {
            foreach (var item in _sessionCache)
            {
                item.Dispose();
            }
        }

        Marshal.FinalReleaseComObject(_audioSessionManager);
    }

    private void RefreshSessions()
    {
        IAudioSessionEnumerator enumerator;
        _audioSessionManager.GetSessionEnumerator(out enumerator);

        if (enumerator == null)
            return;

        int count;
        enumerator.GetCount(out count);

        _sessionCache = new List<CoreAudioSession>(count);

        for (var i = 0; i < count; i++)
        {
            IAudioSessionControl session;
            enumerator.GetSession(i, out session);
#pragma warning disable CA2000 // Dispose objects before losing scope
            var managedSession = CacheSessionWrapper(session);
#pragma warning restore CA2000 // Dispose objects before losing scope
            OnSessionCreated(managedSession);
        }
    }

    private CoreAudioSession CacheSessionWrapper(IAudioSessionControl session)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        var managedSession = new CoreAudioSession(_device, session);
#pragma warning restore CA2000 // Dispose objects before losing scope

        //There's some dumb crap in the Api that causes the sessions to still appear
        //even after the process has been terminated
        if (Process.GetProcesses().All(x => x.Id != managedSession.ProcessId))
            return null;

        var acquiredLock = _lock.AcquireReadLockNonReEntrant();
        try
        {
            var existing = _sessionCache.FirstOrDefault(x => x.ProcessId == managedSession.ProcessId && string.Equals(x.Id, managedSession.Id));
            if (existing != null)
            {
                managedSession.Dispose();
                return existing;
            }
        }
        finally
        {
            if (acquiredLock)
                _lock.ExitReadLock();
        }

        managedSession.StateChanged.Subscribe(ManagedSessionOnStateChanged);
        managedSession.Disconnected.Subscribe(ManagedSessionOnDisconnected);

        acquiredLock = _lock.AcquireWriteLockNonReEntrant();

        try
        {
            _sessionCache.Add(managedSession);
        }
        finally
        {
            if (acquiredLock)
                _lock.ExitWriteLock();
        }

        return managedSession;
    }

    private void ManagedSessionOnStateChanged(SessionStateChangedArgs changedArgs)
    {
    }

    private void ManagedSessionOnDisconnected(SessionDisconnectedArgs disconnectedArgs)
    {
        var sessions = _sessionCache.Where(x => x.Id == disconnectedArgs.Session.Id);

        RemoveSessions(sessions);
    }

    private void RemoveSessions(IEnumerable<CoreAudioSession> sessions)
    {
        var coreAudioSessions = sessions as IList<CoreAudioSession> ?? sessions.ToList();

        if (!coreAudioSessions.Any())
            return;

        var acquiredLock = _lock.AcquireWriteLockNonReEntrant();

        try
        {
            _sessionCache.RemoveAll(x => coreAudioSessions.Any(y => y.Id == x.Id));
        }
        finally
        {
            if (acquiredLock)
                _lock.ExitWriteLock();
        }

        foreach (var s in coreAudioSessions)
        {
            OnSessionDisconnected(s);
            s.Dispose();
        }
    }

    private void OnSessionCreated(IAudioSession session)
    {
        _sessionCreated.OnNext(session);
    }

    private void OnSessionDisconnected(IAudioSession session)
    {
        _sessionDisconnected.OnNext(session.Id);
    }
}