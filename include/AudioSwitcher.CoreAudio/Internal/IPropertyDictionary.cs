using System;

namespace AudioSwitcher.CoreAudio.Internal;

internal interface IPropertyDictionary : IDisposable
{
    AccessMode Mode { get; }

    int Count { get; }

    object this[PropertyKey key] { get; set; }

    bool Contains(PropertyKey key);
}

[Flags]
internal enum AccessMode
{
    Read = 1,
    Write = 2,
    ReadWrite = Read | Write
}