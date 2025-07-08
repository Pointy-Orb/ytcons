using Newtonsoft.Json;

namespace YTCons;

#nullable disable
public class Entry
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("channel")]
    public string Channel { get; set; }

    [JsonProperty("channel_id")]
    public string ChannelId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("availability")]
    public object Availability { get; set; }

    [JsonProperty("channel_follower_count")]
    public int ChannelFollowerCount { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("tags")]
    public List<object> Tags { get; set; }

    [JsonProperty("thumbnails")]
    public List<Thumbnail> Thumbnails { get; set; }

    [JsonProperty("uploader_id")]
    public string UploaderId { get; set; }

    [JsonProperty("uploader_url")]
    public string UploaderUrl { get; set; }

    [JsonProperty("modified_date")]
    public object ModifiedDate { get; set; }

    [JsonProperty("view_count")]
    public long? ViewCount { get; set; }

    [JsonProperty("playlist_count")]
    public int PlaylistCount { get; set; }

    [JsonProperty("uploader")]
    public string Uploader { get; set; }

    [JsonProperty("channel_url")]
    public string ChannelUrl { get; set; }

    [JsonProperty("_type")]
    public string Type { get; set; }

    [JsonProperty("entries")]
    public List<Entry> Entries { get; set; }

    [JsonProperty("extractor_key")]
    public string ExtractorKey { get; set; }

    [JsonProperty("extractor")]
    public string Extractor { get; set; }

    [JsonProperty("webpage_url")]
    public string WebpageUrl { get; set; }

    [JsonProperty("__x_forwarded_for_ip")]
    public object XForwardedForIp { get; set; }

    [JsonProperty("release_year")]
    public object ReleaseYear { get; set; }

    [JsonProperty("epoch")]
    public int Epoch { get; set; }

    [JsonProperty("ie_key")]
    public string IeKey { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("timestamp")]
    public object Timestamp { get; set; }

    [JsonProperty("release_timestamp")]
    public object ReleaseTimestamp { get; set; }

    [JsonProperty("live_status")]
    public object LiveStatus { get; set; }

    [JsonProperty("channel_is_verified")]
    public bool ChannelIsVerified { get; set; }
}

public class ChannelData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("channel")]
    public string Channel { get; set; }

    [JsonProperty("channel_id")]
    public string ChannelId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("availability")]
    public object Availability { get; set; }

    [JsonProperty("channel_follower_count")]
    public int ChannelFollowerCount { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("tags")]
    public List<object> Tags { get; set; }

    [JsonProperty("thumbnails")]
    public List<Thumbnail> Thumbnails { get; set; }

    [JsonProperty("uploader_id")]
    public string UploaderId { get; set; }

    [JsonProperty("uploader_url")]
    public string UploaderUrl { get; set; }

    [JsonProperty("modified_date")]
    public object ModifiedDate { get; set; }

    [JsonProperty("view_count")]
    public long? ViewCount { get; set; }

    [JsonProperty("playlist_count")]
    public int PlaylistCount { get; set; }

    [JsonProperty("uploader")]
    public string Uploader { get; set; }

    [JsonProperty("channel_url")]
    public string ChannelUrl { get; set; }

    [JsonProperty("_type")]
    public string Type { get; set; }

    [JsonProperty("entries")]
    public List<Entry> Entries { get; set; }

    [JsonProperty("webpage_url")]
    public string WebpageUrl { get; set; }

    [JsonProperty("original_url")]
    public string OriginalUrl { get; set; }

    [JsonProperty("webpage_url_basename")]
    public string WebpageUrlBasename { get; set; }

    [JsonProperty("webpage_url_domain")]
    public string WebpageUrlDomain { get; set; }

    [JsonProperty("extractor")]
    public string Extractor { get; set; }

    [JsonProperty("extractor_key")]
    public string ExtractorKey { get; set; }

    [JsonProperty("release_year")]
    public object ReleaseYear { get; set; }

    [JsonProperty("epoch")]
    public int Epoch { get; set; }
}
