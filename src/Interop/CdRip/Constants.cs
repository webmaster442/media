// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop.CdRip;

internal static class Constants
{
    public const int NSECTORS = 13;
    public const int CB_CDROMSECTOR = 2048;
    public const int CB_QSUBCHANNEL = 16;
    public const int CB_CDDASECTOR = 2368;
    public const int CB_AUDIO = (CB_CDDASECTOR - CB_QSUBCHANNEL);
}
