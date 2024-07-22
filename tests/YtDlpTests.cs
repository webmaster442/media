using FFCmd.Infrastructure;
using FFCmd.Interop;

namespace FFCmd.Tests;

[TestFixture]
internal class YtDlpTests
{
    [Test]
    public void TestSelection()
    {
        var parsed  = Parsers.ParseFormats(ParsersTest.TestCase);
        var result = YtDlp.CreateDownloadArguments(parsed, YtDlpQuality.Hd1080Mp4, "https://www.youtube.com/watch?v=1234");
        Assert.That(result, Is.EqualTo("-f 270+140 https://www.youtube.com/watch?v=1234"));
    }
}
