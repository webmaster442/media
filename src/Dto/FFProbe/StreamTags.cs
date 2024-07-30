// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.FFProbe;

public record StreamTags(
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("_STATISTICS_WRITING_APP")] string STATISTICSWRITINGAPP,
    [property: JsonPropertyName("_STATISTICS_WRITING_APPeng")] string STATISTICSWRITINGAPPeng,
    [property: JsonPropertyName("_STATISTICS_WRITING_DATE_UTC")] string STATISTICS_WRITING_DATE_UTC,
    [property: JsonPropertyName("_STATISTICS_WRITING_DATE_UTCeng")] string STATISTICS_WRITING_DATE_UTCeng,
    [property: JsonPropertyName("_STATISTICS_TAGS")] string STATISTICSTAGS,
    [property: JsonPropertyName("_STATISTICS_TAGSeng")] string STATISTICSTAGSeng,
    [property: JsonPropertyName("BPS")] string BPS,
    [property: JsonPropertyName("BPSeng")] string BPSeng,
    [property: JsonPropertyName("DURATIONeng")] string DURATIONeng,
    [property: JsonPropertyName("NUMBER_OF_FRAMES")] string NUMBEROFFRAMES,
    [property: JsonPropertyName("NUMBER_OF_FRAMESeng")] string NUMBEROFFRAMESeng,
    [property: JsonPropertyName("NUMBER_OF_BYTES")] string NUMBEROFBYTES,
    [property: JsonPropertyName("NUMBER_OF_BYTESeng")] string NUMBEROFBYTESeng,
    [property: JsonPropertyName("DURATION")] string DURATION,
    [property: JsonPropertyName("ENCODER")] string ENCODER,
    [property: JsonPropertyName("HANDLER_NAME")] string HANDLERNAME,
    [property: JsonPropertyName("VENDOR_ID")] string VENDORID);
