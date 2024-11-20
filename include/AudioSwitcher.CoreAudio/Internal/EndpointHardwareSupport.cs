using System;

namespace AudioSwitcher.CoreAudio.Internal;

[Flags]
public enum EndpointHardwareSupport
{
    Volume = 0x0001,
    Mute = 0x0002,
    Meter = 0x0004
}