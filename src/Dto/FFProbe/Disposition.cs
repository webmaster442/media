using System.Text.Json.Serialization;

namespace FFCmd.Dto.FFProbe;

public record Disposition(
    [property: JsonPropertyName("_default")] int Default,
    [property: JsonPropertyName("dub")] int Dub,
    [property: JsonPropertyName("original")] int Original,
    [property: JsonPropertyName("comment")] int Comment,
    [property: JsonPropertyName("lyrics")] int Lyrics,
    [property: JsonPropertyName("karaoke")] int Karaoke,
    [property: JsonPropertyName("forced")] int Forced,
    [property: JsonPropertyName("hearing_impaired")] int HearingImpaired,
    [property: JsonPropertyName("visual_impaired")] int VisualImpaired,
    [property: JsonPropertyName("clean_effects")] int CleanEffects,
    [property: JsonPropertyName("attached_pic")] int AttachedPic,
    [property: JsonPropertyName("timed_thumbnails")] int TimedThumbnails,
    [property: JsonPropertyName("non_diegetic")] int NonDiegetic,
    [property: JsonPropertyName("captions")] int Captions,
    [property: JsonPropertyName("descriptions")] int Descriptions,
    [property: JsonPropertyName("metadata")] int Metadata,
    [property: JsonPropertyName("dependent")] int Dependent,
    [property: JsonPropertyName("still_image")] int StillImage,
    [property: JsonPropertyName("multilayer")] int Multilayer);
