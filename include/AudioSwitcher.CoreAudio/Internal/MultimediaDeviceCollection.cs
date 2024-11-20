using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AudioSwitcher.CoreAudio.Internal.Interfaces;
using AudioSwitcher.CoreAudio.Threading;

namespace AudioSwitcher.CoreAudio.Internal;

internal class MultimediaDeviceCollection : IEnumerable<IMultimediaDevice>, IDisposable
{
    private IMultimediaDeviceCollection _multimediaDeviceCollection;

    /// <summary>
    ///     Device count
    /// </summary>
    public int Count
    {
        get
        {
            ComThread.Assert();
            uint result;
            Marshal.ThrowExceptionForHR(_multimediaDeviceCollection.GetCount(out result));
            return Convert.ToInt32(result);
        }
    }

    /// <summary>
    ///     Get device by index
    /// </summary>
    /// <param name="index">Device index</param>
    /// <returns>Device at the specified index</returns>
    public IMultimediaDevice this[int index]
    {
        get
        {
            ComThread.Assert();
            IMultimediaDevice result;
            _multimediaDeviceCollection.Item(Convert.ToUInt32(index), out result);
            return result;
        }
    }

    internal MultimediaDeviceCollection(IMultimediaDeviceCollection parent)
    {
        ComThread.Assert();
        _multimediaDeviceCollection = parent;
    }

    ~MultimediaDeviceCollection()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        _multimediaDeviceCollection = null;
    }

    public IEnumerator<IMultimediaDevice> GetEnumerator()
    {
        for (var index = 0; index < Count; index++)
        {
            yield return this[index];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}