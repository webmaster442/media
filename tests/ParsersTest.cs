using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFCmd.Dto.Internals;
using FFCmd.Infrastructure;

namespace FFCmd.Tests;

[TestFixture]
internal class ParsersTest
{
    public const string TestCase = """
        [youtube] Extracting URL: https://www.youtube.com/watch?v=WEBaZPsBQKE
        [youtube] WEBaZPsBQKE: Downloading webpage
        [youtube] WEBaZPsBQKE: Downloading ios player API JSON
        [youtube] WEBaZPsBQKE: Downloading player 38c5c1c9
        [youtube] WEBaZPsBQKE: Downloading web player API JSON
        [youtube] WEBaZPsBQKE: Downloading m3u8 information
        [info] Available formats for WEBaZPsBQKE:
        ID  EXT   RESOLUTION FPS CH |   FILESIZE    TBR PROTO | VCODEC           VBR ACODEC      ABR ASR MORE INFO
        -----------------------------------------------------------------------------------------------------------------------
        sb3 mhtml 48x27        0    |                   mhtml | images                                   storyboard
        sb2 mhtml 80x45        1    |                   mhtml | images                                   storyboard
        sb1 mhtml 160x90       1    |                   mhtml | images                                   storyboard
        sb0 mhtml 320x180      1    |                   mhtml | images                                   storyboard
        233 mp4   audio only        |                   m3u8  | audio only           unknown             [en] Default
        234 mp4   audio only        |                   m3u8  | audio only           unknown             [en] Default
        139 m4a   audio only      2 |    1.24MiB    49k https | audio only           mp4a.40.5   49k 22k [en] low, m4a_dash
        249 webm  audio only      2 |    1.25MiB    49k https | audio only           opus        49k 48k [en] low, webm_dash
        250 webm  audio only      2 |    1.65MiB    65k https | audio only           opus        65k 48k [en] low, webm_dash
        140 m4a   audio only      2 |    3.28MiB   130k https | audio only           mp4a.40.2  130k 44k [en] medium, m4a_dash
        251 webm  audio only      2 |    3.28MiB   129k https | audio only           opus       129k 48k [en] medium, webm_dash
        602 mp4   256x144     13    | ~  3.01MiB   119k m3u8  | vp09.00.10.08   119k video only
        269 mp4   256x144     25    | ~  4.95MiB   195k m3u8  | avc1.4D400C     195k video only
        160 mp4   256x144     25    |    3.18MiB   126k https | avc1.4D400C     126k video only          144p, mp4_dash
        603 mp4   256x144     25    | ~  5.13MiB   202k m3u8  | vp09.00.11.08   202k video only
        278 webm  256x144     25    |    3.11MiB   123k https | vp9             123k video only          144p, webm_dash
        229 mp4   426x240     25    | ~  9.22MiB   363k m3u8  | avc1.4D4015     363k video only
        133 mp4   426x240     25    |    7.02MiB   277k https | avc1.4D4015     277k video only          240p, mp4_dash
        604 mp4   426x240     25    | ~  9.80MiB   386k m3u8  | vp09.00.20.08   386k video only
        242 webm  426x240     25    |    6.33MiB   250k https | vp9             250k video only          240p, webm_dash
        230 mp4   640x360     25    | ~ 22.38MiB   881k m3u8  | avc1.4D401E     881k video only
        134 mp4   640x360     25    |   16.11MiB   636k https | avc1.4D401E     636k video only          360p, mp4_dash
        18  mp4   640x360     25  2 |   18.55MiB   732k https | avc1.42001E          mp4a.40.2       44k [en] 360p
        605 mp4   640x360     25    | ~ 18.00MiB   709k m3u8  | vp09.00.21.08   709k video only
        243 webm  640x360     25    |   10.43MiB   412k https | vp9             412k video only          360p, webm_dash
        231 mp4   854x480     25    | ~ 38.76MiB  1526k m3u8  | avc1.4D401E    1526k video only
        135 mp4   854x480     25    |   30.10MiB  1188k https | avc1.4D401E    1188k video only          480p, mp4_dash
        606 mp4   854x480     25    | ~ 32.39MiB  1276k m3u8  | vp09.00.30.08  1276k video only
        244 webm  854x480     25    |   17.97MiB   709k https | vp9             709k video only          480p, webm_dash
        232 mp4   1280x720    25    | ~ 73.73MiB  2904k m3u8  | avc1.64001F    2904k video only
        136 mp4   1280x720    25    |   57.53MiB  2270k https | avc1.64001F    2270k video only          720p, mp4_dash
        609 mp4   1280x720    25    | ~ 71.35MiB  2810k m3u8  | vp09.00.31.08  2810k video only
        247 webm  1280x720    25    |   35.43MiB  1398k https | vp9            1398k video only          720p, webm_dash
        270 mp4   1920x1080   25    | ~149.37MiB  5883k m3u8  | avc1.640028    5883k video only
        137 mp4   1920x1080   25    |  119.81MiB  4728k https | avc1.640028    4728k video only          1080p, mp4_dash
        614 mp4   1920x1080   25    | ~102.43MiB  4034k m3u8  | vp09.00.40.08  4034k video only
        248 webm  1920x1080   25    |   54.85MiB  2165k https | vp9            2165k video only          1080p, webm_dash
        620 mp4   2560x1440   25    | ~228.57MiB  9002k m3u8  | vp09.00.50.08  9002k video only
        271 webm  2560x1440   25    |  179.39MiB  7080k https | vp9            7080k video only          1440p, webm_dash
        625 mp4   3840x2160   25    | ~474.45MiB 18685k m3u8  | vp09.00.50.08 18685k video only
        313 webm  3840x2160   25    |  335.96MiB 13259k https | vp9           13259k video only          2160p, webm_dash
        """;

    [Test]
    public void TestParseCount()
    {
        var result = Parsers.ParseFormats(TestCase).ToArray();
        Assert.That(result, Has.Length.EqualTo(41));
    }

    [Test]
    public void TestFormatParse()
    {
        var result = Parsers.ParseFormats(TestCase).ToArray();
        var expected = new YtDlpFormat
        {
            Id = "313",
            Format = "webm",
            Width = 3840,
            Height = 2160,
            BitrateInK = 13259,
            Codec = "vp9",
        };
        Assert.That(result[40], Is.EqualTo(expected));
    }
}
