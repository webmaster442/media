﻿using System.Runtime.InteropServices;

namespace Media.Interop;

internal static partial class AudioController
{
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEndpointVolume
    {
        [PreserveSig]
        int NotImpl1();

        [PreserveSig]
        int NotImpl2();

        /// <summary>
        /// Gets a count of the channels in the audio stream.
        /// </summary>
        /// <param name="channelCount">The number of channels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetChannelCount(
            [Out][MarshalAs(UnmanagedType.U4)] out UInt32 channelCount);

        /// <summary>
        /// Sets the master volume level of the audio stream, in decibels.
        /// </summary>
        /// <param name="level">The new master volume level in decibels.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetMasterVolumeLevel(
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Sets the master volume level, expressed as a normalized, audio-tapered value.
        /// </summary>
        /// <param name="level">The new master volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetMasterVolumeLevelScalar(
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Gets the master volume level of the audio stream, in decibels.
        /// </summary>
        /// <param name="level">The volume level in decibels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetMasterVolumeLevel(
            [Out][MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Gets the master volume level, expressed as a normalized, audio-tapered value.
        /// </summary>
        /// <param name="level">The volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetMasterVolumeLevelScalar(
            [Out][MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Sets the volume level, in decibels, of the specified channel of the audio stream.
        /// </summary>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="level">The new volume level in decibels.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetChannelVolumeLevel(
            [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Sets the normalized, audio-tapered volume level of the specified channel in the audio stream.
        /// </summary>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="level">The new master volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetChannelVolumeLevelScalar(
            [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Gets the volume level, in decibels, of the specified channel in the audio stream.
        /// </summary>
        /// <param name="channelNumber">The zero-based channel number.</param>
		/// <param name="level">The volume level in decibels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetChannelVolumeLevel(
            [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [Out][MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Gets the normalized, audio-tapered volume level of the specified channel of the audio stream.
        /// </summary>
        /// <param name="channelNumber">The zero-based channel number.</param>
		/// <param name="level">The volume level expressed as a normalized value between 0.0 and 1.0.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetChannelVolumeLevelScalar(
            [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
            [Out][MarshalAs(UnmanagedType.R4)] out float level);

        /// <summary>
        /// Sets the muting state of the audio stream.
        /// </summary>
        /// <param name="isMuted">True to mute the stream, or false to unmute the stream.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetMute(
            [In][MarshalAs(UnmanagedType.Bool)] Boolean isMuted,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Gets the muting state of the audio stream.
        /// </summary>
        /// <param name="isMuted">The muting state. True if the stream is muted, false otherwise.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetMute(
            [Out][MarshalAs(UnmanagedType.Bool)] out Boolean isMuted);

        /// <summary>
        /// Gets information about the current step in the volume range.
        /// </summary>
        /// <param name="step">The current zero-based step index.</param>
        /// <param name="stepCount">The total number of steps in the volume range.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetVolumeStepInfo(
            [Out][MarshalAs(UnmanagedType.U4)] out UInt32 step,
            [Out][MarshalAs(UnmanagedType.U4)] out UInt32 stepCount);

        /// <summary>
        /// Increases the volume level by one step.
        /// </summary>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int VolumeStepUp(
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Decreases the volume level by one step.
        /// </summary>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int VolumeStepDown(
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        /// <summary>
        /// Queries the audio endpoint device for its hardware-supported functions.
        /// </summary>
        /// <param name="hardwareSupportMask">A hardware support mask that indicates the capabilities of the endpoint.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int QueryHardwareSupport(
            [Out][MarshalAs(UnmanagedType.U4)] out UInt32 hardwareSupportMask);

        /// <summary>
        /// Gets the volume range of the audio stream, in decibels.
        /// </summary>
		/// <param name="volumeMin">The minimum volume level in decibels.</param>
		/// <param name="volumeMax">The maximum volume level in decibels.</param>
		/// <param name="volumeStep">The volume increment level in decibels.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetVolumeRange(
            [Out][MarshalAs(UnmanagedType.R4)] out float volumeMin,
            [Out][MarshalAs(UnmanagedType.R4)] out float volumeMax,
            [Out][MarshalAs(UnmanagedType.R4)] out float volumeStep);
    }

    [ComImport]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    internal class MMDeviceEnumerator
    {
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        int NotImpl1();

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

        // the rest is not implemented
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

        // the rest is not implemented
    }

    internal sealed class SystemAudio
    {
        private readonly IAudioEndpointVolume _audioEndpointVolume;

        public bool IsSystemMuted { get; }

        internal SystemAudio(IAudioEndpointVolume audioEndpointVolume)
        {
            _audioEndpointVolume = audioEndpointVolume;
            audioEndpointVolume.GetMute(out bool isMuted);
            IsSystemMuted = isMuted;
        }

        public void Mute(bool mute)
        {
            _audioEndpointVolume.GetMute(out bool isMuted);

            if (isMuted ^ mute)
            {
                _audioEndpointVolume.SetMute(mute, Guid.NewGuid());
            }
        }

        public void SetVolume(float volume)
            => _audioEndpointVolume.SetMasterVolumeLevel(volume, Guid.NewGuid());

        public float GetVolume()
        {
            _audioEndpointVolume.GetMasterVolumeLevelScalar(out float volume);
            return volume;
        }
    }

    internal enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
        EDataFlow_enum_count
    }

    internal enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
        ERole_enum_count
    }

    public static void Interact(Action<SystemAudio> interaction)
    {
        IntPtr deviceEnumeratorPtr = IntPtr.Zero;
        IntPtr audioEndpointVolumePtr = IntPtr.Zero;

        IMMDeviceEnumerator? deviceEnumerator = null;
        IMMDevice? speakers = null;
        try
        {
            deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());

            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender,
                                                     ERole.eMultimedia,
                                                     out speakers);

            Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;

            speakers.Activate(ref IID_IAudioEndpointVolume,
                              0,
                              IntPtr.Zero,
                              out object o);

            IAudioEndpointVolume masterVol = (IAudioEndpointVolume)o;

            interaction(new SystemAudio(masterVol));
        }
        finally
        {
            if (speakers != null)
                Marshal.ReleaseComObject(speakers);
            if (deviceEnumerator != null)
                Marshal.ReleaseComObject(deviceEnumerator);
        }
    }
}
