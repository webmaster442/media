using System;

namespace AudioSwitcher.CoreAudio.Internal;

[Flags]
internal enum ERole : uint
{
    Console = 0,
    Multimedia = 1,
    Communications = 2
}