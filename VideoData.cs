using Newtonsoft.Json;

namespace YTCons;

#nullable disable
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class SubLang
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Chapter
{
    [JsonProperty("start_time")]
    public double? StartTime { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("end_time")]
    public double? EndTime { get; set; }
}

public class DownloaderOptions
{
    [JsonProperty("http_chunk_size")]
    public int? HttpChunkSize { get; set; }
}

public class FormatData
{
    [JsonProperty("format_id")]
    public string FormatId { get; set; }

    [JsonProperty("format_note")]
    public string FormatNote { get; set; }

    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("protocol")]
    public string Protocol { get; set; }

    [JsonProperty("acodec")]
    public string Acodec { get; set; }

    [JsonProperty("vcodec")]
    public string Vcodec { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("fps")]
    public double? Fps { get; set; }

    [JsonProperty("rows")]
    public int? Rows { get; set; }

    [JsonProperty("columns")]
    public int? Columns { get; set; }

    [JsonProperty("fragments")]
    public List<Fragment> Fragments { get; } = new List<Fragment>();

    [JsonProperty("audio_ext")]
    public string AudioExt { get; set; }

    [JsonProperty("video_ext")]
    public string VideoExt { get; set; }

    [JsonProperty("vbr")]
    public double? Vbr { get; set; }

    [JsonProperty("abr")]
    public double? Abr { get; set; }

    [JsonProperty("resolution")]
    public string Resolution { get; set; }

    [JsonProperty("aspect_ratio")]
    public double? AspectRatio { get; set; }

    [JsonProperty("http_headers")]
    public HttpHeaders HttpHeaders { get; set; }

    [JsonProperty("format")]
    public string Format { get; set; }

    [JsonProperty("manifest_url")]
    public string ManifestUrl { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("quality")]
    public double? Quality { get; set; }

    [JsonProperty("has_drm")]
    public bool? HasDrm { get; set; }

    [JsonProperty("source_preference")]
    public int? SourcePreference { get; set; }

    [JsonProperty("asr")]
    public int? Asr { get; set; }

    [JsonProperty("filesize")]
    public long? Filesize { get; set; }

    [JsonProperty("audio_channels")]
    public int? AudioChannels { get; set; }

    [JsonProperty("tbr")]
    public double? Tbr { get; set; }

    [JsonProperty("filesize_approx")]
    public long? FilesizeApprox { get; set; }

    [JsonProperty("language_preference")]
    public int? LanguagePreference { get; set; }

    [JsonProperty("container")]
    public string Container { get; set; }

    [JsonProperty("downloader_options")]
    public DownloaderOptions DownloaderOptions { get; set; }

    [JsonProperty("dynamic_range")]
    public string DynamicRange { get; set; }
}

public class Fragment
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("duration")]
    public double? Duration { get; set; }
}

public class HttpHeaders
{
    [JsonProperty("User-Agent")]
    public string UserAgent { get; set; }

    [JsonProperty("Accept")]
    public string Accept { get; set; }

    [JsonProperty("Accept-Language")]
    public string AcceptLanguage { get; set; }

    [JsonProperty("Sec-Fetch-Mode")]
    public string SecFetchMode { get; set; }
}

public class VideoData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("formats")]
    public List<FormatData> Formats { get; } = new List<FormatData>();

    [JsonProperty("thumbnails")]
    public List<Thumbnail> Thumbnails { get; } = new List<Thumbnail>();

    [JsonProperty("thumbnail")]
    public string Thumbnail { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("channel_id")]
    public string ChannelId { get; set; }

    [JsonProperty("channel_url")]
    public string ChannelUrl { get; set; }

    [JsonProperty("duration")]
    public int? Duration { get; set; }

    [JsonProperty("view_count")]
    public int? ViewCount { get; set; }

    [JsonProperty("age_limit")]
    public int? AgeLimit { get; set; }

    [JsonProperty("webpage_url")]
    public string WebpageUrl { get; set; }

    [JsonProperty("categories")]
    public List<string> Categories { get; } = new List<string>();

    [JsonProperty("tags")]
    public List<string> Tags { get; } = new List<string>();

    [JsonProperty("playable_in_embed")]
    public bool? PlayableInEmbed { get; set; }

    [JsonProperty("live_status")]
    public string LiveStatus { get; set; }

    [JsonProperty("media_type")]
    public string MediaType { get; set; }

    [JsonProperty("_format_sort_fields")]
    public List<string> FormatSortFields { get; } = new List<string>();

    [JsonProperty("automatic_captions")]
    public Dictionary<string, SubLang[]> AutomaticCaptions { get; set; }

    [JsonProperty("subtitles")]
    public Dictionary<string, SubLang[]> Subtitles { get; set; }

    [JsonProperty("comment_count")]
    public int? CommentCount { get; set; }

    [JsonProperty("chapters")]
    public List<Chapter> Chapters { get; } = new List<Chapter>();

    [JsonProperty("like_count")]
    public int? LikeCount { get; set; }

    [JsonProperty("channel")]
    public string Channel { get; set; }

    [JsonProperty("channel_follower_count")]
    public int? ChannelFollowerCount { get; set; }

    [JsonProperty("channel_is_verified")]
    public bool? ChannelIsVerified { get; set; } = false;

    [JsonProperty("uploader")]
    public string Uploader { get; set; }

    [JsonProperty("uploader_id")]
    public string UploaderId { get; set; }

    [JsonProperty("uploader_url")]
    public string UploaderUrl { get; set; }

    [JsonProperty("upload_date")]
    public string UploadDate { get; set; }

    [JsonProperty("timestamp")]
    public int? Timestamp { get; set; }

    [JsonProperty("availability")]
    public string Availability { get; set; }

    [JsonProperty("webpage_url_basename")]
    public string WebpageUrlBasename { get; set; }

    [JsonProperty("webpage_url_domain")]
    public string WebpageUrlDomain { get; set; }

    [JsonProperty("extractor")]
    public string Extractor { get; set; }

    [JsonProperty("extractor_key")]
    public string ExtractorKey { get; set; }

    [JsonProperty("display_id")]
    public string DisplayId { get; set; }

    [JsonProperty("fulltitle")]
    public string Fulltitle { get; set; }

    [JsonProperty("duration_string")]
    public string DurationString { get; set; }

    [JsonProperty("is_live")]
    public bool? IsLive { get; set; }

    [JsonProperty("was_live")]
    public bool? WasLive { get; set; }

    [JsonProperty("epoch")]
    public int? Epoch { get; set; }

    [JsonProperty("format")]
    public string Format { get; set; }

    [JsonProperty("format_id")]
    public string FormatId { get; set; }

    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("protocol")]
    public string Protocol { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("format_note")]
    public string FormatNote { get; set; }

    [JsonProperty("filesize_approx")]
    public long? FilesizeApprox { get; set; }

    [JsonProperty("tbr")]
    public double? Tbr { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("resolution")]
    public string Resolution { get; set; }

    [JsonProperty("fps")]
    public double? Fps { get; set; }

    [JsonProperty("dynamic_range")]
    public string DynamicRange { get; set; }

    [JsonProperty("vcodec")]
    public string Vcodec { get; set; }

    [JsonProperty("vbr")]
    public double? Vbr { get; set; }

    [JsonProperty("aspect_ratio")]
    public double? AspectRatio { get; set; }

    [JsonProperty("acodec")]
    public string Acodec { get; set; }

    [JsonProperty("abr")]
    public double? Abr { get; set; }

    [JsonProperty("asr")]
    public int? Asr { get; set; }

    [JsonProperty("audio_channels")]
    public int? AudioChannels { get; set; }

    [JsonProperty("_type")]
    public string Type { get; set; }

    [JsonProperty("_version")]
    public VersionData Version { get; set; }
}

public class Thumbnail
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("preference")]
    public int? Preference { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("resolution")]
    public string Resolution { get; set; }
}
public class VersionData
{
    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("repository")]
    public string Repository { get; set; }
}
