// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Interop;

namespace Media.Tests.Units;

[TestFixture]
internal class YtDlpTests
{
    [Test]
    public void TestSelection()
    {
        var parsed = Parsers.ParseFormats(ParsersTests.YtDlpTestCase);
        var result = YtDlp.CreateDownloadArguments(parsed, YtDlpQuality.Hd1080Mp4, "https://www.youtube.com/watch?v=1234");
        Assert.That(result, Is.EqualTo("-f 270+140 https://www.youtube.com/watch?v=1234"));
    }
}
