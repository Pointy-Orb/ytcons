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
public class Captions
{
    [JsonProperty("ab")]
    public List<SubLang> ab { get; } = new List<SubLang>();

    [JsonProperty("aa")]
    public List<SubLang> aa { get; } = new List<SubLang>();

    [JsonProperty("af")]
    public List<SubLang> af { get; } = new List<SubLang>();

    [JsonProperty("ak")]
    public List<SubLang> ak { get; } = new List<SubLang>();

    [JsonProperty("sq")]
    public List<SubLang> sq { get; } = new List<SubLang>();

    [JsonProperty("am")]
    public List<SubLang> am { get; } = new List<SubLang>();

    [JsonProperty("ar")]
    public List<SubLang> ar { get; } = new List<SubLang>();

    [JsonProperty("hy")]
    public List<SubLang> hy { get; } = new List<SubLang>();

    [JsonProperty("as")]
    public List<SubLang> ase { get; } = new List<SubLang>();

    [JsonProperty("ay")]
    public List<SubLang> ay { get; } = new List<SubLang>();

    [JsonProperty("az")]
    public List<SubLang> az { get; } = new List<SubLang>();

    [JsonProperty("bn")]
    public List<SubLang> bn { get; } = new List<SubLang>();

    [JsonProperty("ba")]
    public List<SubLang> ba { get; } = new List<SubLang>();

    [JsonProperty("eu")]
    public List<SubLang> eu { get; } = new List<SubLang>();

    [JsonProperty("be")]
    public List<SubLang> be { get; } = new List<SubLang>();

    [JsonProperty("bho")]
    public List<SubLang> bho { get; } = new List<SubLang>();

    [JsonProperty("bs")]
    public List<SubLang> bs { get; } = new List<SubLang>();

    [JsonProperty("br")]
    public List<SubLang> br { get; } = new List<SubLang>();

    [JsonProperty("bg")]
    public List<SubLang> bg { get; } = new List<SubLang>();

    [JsonProperty("my")]
    public List<SubLang> my { get; } = new List<SubLang>();

    [JsonProperty("ca")]
    public List<SubLang> ca { get; } = new List<SubLang>();

    [JsonProperty("ceb")]
    public List<SubLang> ceb { get; } = new List<SubLang>();

    [JsonProperty("zh-Hans")]
    public List<SubLang> zhhans { get; } = new List<SubLang>();

    [JsonProperty("zh-Hant")]
    public List<SubLang> zhhant { get; } = new List<SubLang>();

    [JsonProperty("co")]
    public List<SubLang> co { get; } = new List<SubLang>();

    [JsonProperty("hr")]
    public List<SubLang> hr { get; } = new List<SubLang>();

    [JsonProperty("cs")]
    public List<SubLang> cs { get; } = new List<SubLang>();

    [JsonProperty("da")]
    public List<SubLang> da { get; } = new List<SubLang>();

    [JsonProperty("dv")]
    public List<SubLang> dv { get; } = new List<SubLang>();

    [JsonProperty("nl")]
    public List<SubLang> nl { get; } = new List<SubLang>();

    [JsonProperty("dz")]
    public List<SubLang> dz { get; } = new List<SubLang>();

    [JsonProperty("en")]
    public List<SubLang> en { get; } = new List<SubLang>();

    [JsonProperty("eo")]
    public List<SubLang> eo { get; } = new List<SubLang>();

    [JsonProperty("et")]
    public List<SubLang> et { get; } = new List<SubLang>();

    [JsonProperty("ee")]
    public List<SubLang> ee { get; } = new List<SubLang>();

    [JsonProperty("fo")]
    public List<SubLang> fo { get; } = new List<SubLang>();

    [JsonProperty("fj")]
    public List<SubLang> fj { get; } = new List<SubLang>();

    [JsonProperty("fil")]
    public List<SubLang> fil { get; } = new List<SubLang>();

    [JsonProperty("fi")]
    public List<SubLang> fi { get; } = new List<SubLang>();

    [JsonProperty("fr")]
    public List<SubLang> fr { get; } = new List<SubLang>();

    [JsonProperty("gaa")]
    public List<SubLang> gaa { get; } = new List<SubLang>();

    [JsonProperty("gl")]
    public List<SubLang> gl { get; } = new List<SubLang>();

    [JsonProperty("lg")]
    public List<SubLang> lg { get; } = new List<SubLang>();

    [JsonProperty("ka")]
    public List<SubLang> ka { get; } = new List<SubLang>();

    [JsonProperty("de")]
    public List<SubLang> de { get; } = new List<SubLang>();

    [JsonProperty("el")]
    public List<SubLang> el { get; } = new List<SubLang>();

    [JsonProperty("gn")]
    public List<SubLang> gn { get; } = new List<SubLang>();

    [JsonProperty("gu")]
    public List<SubLang> gu { get; } = new List<SubLang>();

    [JsonProperty("ht")]
    public List<SubLang> ht { get; } = new List<SubLang>();

    [JsonProperty("ha")]
    public List<SubLang> ha { get; } = new List<SubLang>();

    [JsonProperty("haw")]
    public List<SubLang> haw { get; } = new List<SubLang>();

    [JsonProperty("iw")]
    public List<SubLang> iw { get; } = new List<SubLang>();

    [JsonProperty("hi")]
    public List<SubLang> hi { get; } = new List<SubLang>();

    [JsonProperty("hmn")]
    public List<SubLang> hmn { get; } = new List<SubLang>();

    [JsonProperty("hu")]
    public List<SubLang> hu { get; } = new List<SubLang>();

    [JsonProperty("is")]
    public List<SubLang> isl { get; } = new List<SubLang>();

    [JsonProperty("ig")]
    public List<SubLang> ig { get; } = new List<SubLang>();

    [JsonProperty("id")]
    public List<SubLang> id { get; } = new List<SubLang>();

    [JsonProperty("iu")]
    public List<SubLang> iu { get; } = new List<SubLang>();

    [JsonProperty("ga")]
    public List<SubLang> ga { get; } = new List<SubLang>();

    [JsonProperty("it")]
    public List<SubLang> it { get; } = new List<SubLang>();

    [JsonProperty("ja")]
    public List<SubLang> ja { get; } = new List<SubLang>();

    [JsonProperty("jv")]
    public List<SubLang> jv { get; } = new List<SubLang>();

    [JsonProperty("kl")]
    public List<SubLang> kl { get; } = new List<SubLang>();

    [JsonProperty("kn")]
    public List<SubLang> kn { get; } = new List<SubLang>();

    [JsonProperty("kk")]
    public List<SubLang> kk { get; } = new List<SubLang>();

    [JsonProperty("kha")]
    public List<SubLang> kha { get; } = new List<SubLang>();

    [JsonProperty("km")]
    public List<SubLang> km { get; } = new List<SubLang>();

    [JsonProperty("rw")]
    public List<SubLang> rw { get; } = new List<SubLang>();

    [JsonProperty("ko")]
    public List<SubLang> ko { get; } = new List<SubLang>();

    [JsonProperty("kri")]
    public List<SubLang> kri { get; } = new List<SubLang>();

    [JsonProperty("ku")]
    public List<SubLang> ku { get; } = new List<SubLang>();

    [JsonProperty("ky")]
    public List<SubLang> ky { get; } = new List<SubLang>();

    [JsonProperty("lo")]
    public List<SubLang> lo { get; } = new List<SubLang>();

    [JsonProperty("la")]
    public List<SubLang> la { get; } = new List<SubLang>();

    [JsonProperty("lv")]
    public List<SubLang> lv { get; } = new List<SubLang>();

    [JsonProperty("ln")]
    public List<SubLang> ln { get; } = new List<SubLang>();

    [JsonProperty("lt")]
    public List<SubLang> lt { get; } = new List<SubLang>();

    [JsonProperty("lua")]
    public List<SubLang> lua { get; } = new List<SubLang>();

    [JsonProperty("luo")]
    public List<SubLang> luo { get; } = new List<SubLang>();

    [JsonProperty("lb")]
    public List<SubLang> lb { get; } = new List<SubLang>();

    [JsonProperty("mk")]
    public List<SubLang> mk { get; } = new List<SubLang>();

    [JsonProperty("mg")]
    public List<SubLang> mg { get; } = new List<SubLang>();

    [JsonProperty("ms")]
    public List<SubLang> ms { get; } = new List<SubLang>();

    [JsonProperty("ml")]
    public List<SubLang> ml { get; } = new List<SubLang>();

    [JsonProperty("mt")]
    public List<SubLang> mt { get; } = new List<SubLang>();

    [JsonProperty("gv")]
    public List<SubLang> gv { get; } = new List<SubLang>();

    [JsonProperty("mi")]
    public List<SubLang> mi { get; } = new List<SubLang>();

    [JsonProperty("mr")]
    public List<SubLang> mr { get; } = new List<SubLang>();

    [JsonProperty("mn")]
    public List<SubLang> mn { get; } = new List<SubLang>();

    [JsonProperty("mfe")]
    public List<SubLang> mfe { get; } = new List<SubLang>();

    [JsonProperty("ne")]
    public List<SubLang> ne { get; } = new List<SubLang>();

    [JsonProperty("new")]
    public List<SubLang> nwa { get; } = new List<SubLang>();

    [JsonProperty("nso")]
    public List<SubLang> nso { get; } = new List<SubLang>();

    [JsonProperty("no")]
    public List<SubLang> no { get; } = new List<SubLang>();

    [JsonProperty("ny")]
    public List<SubLang> ny { get; } = new List<SubLang>();

    [JsonProperty("oc")]
    public List<SubLang> oc { get; } = new List<SubLang>();

    [JsonProperty("or")]
    public List<SubLang> or { get; } = new List<SubLang>();

    [JsonProperty("om")]
    public List<SubLang> om { get; } = new List<SubLang>();

    [JsonProperty("os")]
    public List<SubLang> os { get; } = new List<SubLang>();

    [JsonProperty("pam")]
    public List<SubLang> pam { get; } = new List<SubLang>();

    [JsonProperty("ps")]
    public List<SubLang> ps { get; } = new List<SubLang>();

    [JsonProperty("fa")]
    public List<SubLang> fa { get; } = new List<SubLang>();

    [JsonProperty("pl")]
    public List<SubLang> pl { get; } = new List<SubLang>();

    [JsonProperty("pt")]
    public List<SubLang> pt { get; } = new List<SubLang>();

    [JsonProperty("pt-PT")]
    public List<SubLang> ptpt { get; } = new List<SubLang>();

    [JsonProperty("pa")]
    public List<SubLang> pa { get; } = new List<SubLang>();

    [JsonProperty("qu")]
    public List<SubLang> qu { get; } = new List<SubLang>();

    [JsonProperty("ro")]
    public List<SubLang> ro { get; } = new List<SubLang>();

    [JsonProperty("rn")]
    public List<SubLang> rn { get; } = new List<SubLang>();

    [JsonProperty("ru")]
    public List<SubLang> ru { get; } = new List<SubLang>();

    [JsonProperty("sm")]
    public List<SubLang> sm { get; } = new List<SubLang>();

    [JsonProperty("sg")]
    public List<SubLang> sg { get; } = new List<SubLang>();

    [JsonProperty("sa")]
    public List<SubLang> sa { get; } = new List<SubLang>();

    [JsonProperty("gd")]
    public List<SubLang> gd { get; } = new List<SubLang>();

    [JsonProperty("sr")]
    public List<SubLang> sr { get; } = new List<SubLang>();

    [JsonProperty("crs")]
    public List<SubLang> crs { get; } = new List<SubLang>();

    [JsonProperty("sn")]
    public List<SubLang> sn { get; } = new List<SubLang>();

    [JsonProperty("sd")]
    public List<SubLang> sd { get; } = new List<SubLang>();

    [JsonProperty("si")]
    public List<SubLang> si { get; } = new List<SubLang>();

    [JsonProperty("sk")]
    public List<SubLang> sk { get; } = new List<SubLang>();

    [JsonProperty("sl")]
    public List<SubLang> sl { get; } = new List<SubLang>();

    [JsonProperty("so")]
    public List<SubLang> so { get; } = new List<SubLang>();

    [JsonProperty("st")]
    public List<SubLang> st { get; } = new List<SubLang>();

    [JsonProperty("es")]
    public List<SubLang> es { get; } = new List<SubLang>();

    [JsonProperty("su")]
    public List<SubLang> su { get; } = new List<SubLang>();

    [JsonProperty("sw")]
    public List<SubLang> sw { get; } = new List<SubLang>();

    [JsonProperty("ss")]
    public List<SubLang> ss { get; } = new List<SubLang>();

    [JsonProperty("sv")]
    public List<SubLang> sv { get; } = new List<SubLang>();

    [JsonProperty("tg")]
    public List<SubLang> tg { get; } = new List<SubLang>();

    [JsonProperty("ta")]
    public List<SubLang> ta { get; } = new List<SubLang>();

    [JsonProperty("tt")]
    public List<SubLang> tt { get; } = new List<SubLang>();

    [JsonProperty("te")]
    public List<SubLang> te { get; } = new List<SubLang>();

    [JsonProperty("th")]
    public List<SubLang> th { get; } = new List<SubLang>();

    [JsonProperty("bo")]
    public List<SubLang> bo { get; } = new List<SubLang>();

    [JsonProperty("ti")]
    public List<SubLang> ti { get; } = new List<SubLang>();

    [JsonProperty("to")]
    public List<SubLang> to { get; } = new List<SubLang>();

    [JsonProperty("ts")]
    public List<SubLang> ts { get; } = new List<SubLang>();

    [JsonProperty("tn")]
    public List<SubLang> tn { get; } = new List<SubLang>();

    [JsonProperty("tum")]
    public List<SubLang> tum { get; } = new List<SubLang>();

    [JsonProperty("tr")]
    public List<SubLang> tr { get; } = new List<SubLang>();

    [JsonProperty("tk")]
    public List<SubLang> tk { get; } = new List<SubLang>();

    [JsonProperty("uk")]
    public List<SubLang> uk { get; } = new List<SubLang>();

    [JsonProperty("ur")]
    public List<SubLang> ur { get; } = new List<SubLang>();

    [JsonProperty("ug")]
    public List<SubLang> ug { get; } = new List<SubLang>();

    [JsonProperty("uz")]
    public List<SubLang> uz { get; } = new List<SubLang>();

    [JsonProperty("ve")]
    public List<SubLang> ve { get; } = new List<SubLang>();

    [JsonProperty("vi")]
    public List<SubLang> vi { get; } = new List<SubLang>();

    [JsonProperty("war")]
    public List<SubLang> war { get; } = new List<SubLang>();

    [JsonProperty("cy")]
    public List<SubLang> cy { get; } = new List<SubLang>();

    [JsonProperty("fy")]
    public List<SubLang> fy { get; } = new List<SubLang>();

    [JsonProperty("wo")]
    public List<SubLang> wo { get; } = new List<SubLang>();

    [JsonProperty("xh")]
    public List<SubLang> xh { get; } = new List<SubLang>();

    [JsonProperty("yi")]
    public List<SubLang> yi { get; } = new List<SubLang>();

    [JsonProperty("yo")]
    public List<SubLang> yo { get; } = new List<SubLang>();

    [JsonProperty("zu")]
    public List<SubLang> zu { get; } = new List<SubLang>();
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
    public Captions AutomaticCaptions { get; set; }

    [JsonProperty("subtitles")]
    public Captions Subtitles { get; set; }

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
    public bool? ChannelIsVerified { get; set; }

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
