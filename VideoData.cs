using Newtonsoft.Json;

namespace YTCons;

#nullable disable
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class A
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Aa
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ab
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Af
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ak
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Am
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ar
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class AutomaticCaptions
{
    [JsonProperty("ab")]
    public List<Ab> Ab { get; } = new List<Ab>();

    [JsonProperty("aa")]
    public List<Aa> Aa { get; } = new List<Aa>();

    [JsonProperty("af")]
    public List<Af> Af { get; } = new List<Af>();

    [JsonProperty("ak")]
    public List<Ak> Ak { get; } = new List<Ak>();

    [JsonProperty("sq")]
    public List<Sq> Sq { get; } = new List<Sq>();

    [JsonProperty("am")]
    public List<Am> Am { get; } = new List<Am>();

    [JsonProperty("ar")]
    public List<Ar> Ar { get; } = new List<Ar>();

    [JsonProperty("hy")]
    public List<Hy> Hy { get; } = new List<Hy>();

    [JsonProperty("as")]
    public List<A> As { get; } = new List<A>();

    [JsonProperty("ay")]
    public List<Ay> Ay { get; } = new List<Ay>();

    [JsonProperty("az")]
    public List<Az> Az { get; } = new List<Az>();

    [JsonProperty("bn")]
    public List<Bn> Bn { get; } = new List<Bn>();

    [JsonProperty("ba")]
    public List<Ba> Ba { get; } = new List<Ba>();

    [JsonProperty("eu")]
    public List<Eu> Eu { get; } = new List<Eu>();

    [JsonProperty("be")]
    public List<Be> Be { get; } = new List<Be>();

    [JsonProperty("bho")]
    public List<Bho> Bho { get; } = new List<Bho>();

    [JsonProperty("bs")]
    public List<B> Bs { get; } = new List<B>();

    [JsonProperty("br")]
    public List<Br> Br { get; } = new List<Br>();

    [JsonProperty("bg")]
    public List<Bg> Bg { get; } = new List<Bg>();

    [JsonProperty("my")]
    public List<My> My { get; } = new List<My>();

    [JsonProperty("ca")]
    public List<Ca> Ca { get; } = new List<Ca>();

    [JsonProperty("ceb")]
    public List<Ceb> Ceb { get; } = new List<Ceb>();

    [JsonProperty("zh-Hans")]
    public List<ZhHan> ZhHans { get; } = new List<ZhHan>();

    [JsonProperty("zh-Hant")]
    public List<ZhHant> ZhHant { get; } = new List<ZhHant>();

    [JsonProperty("co")]
    public List<Co> Co { get; } = new List<Co>();

    [JsonProperty("hr")]
    public List<Hr> Hr { get; } = new List<Hr>();

    [JsonProperty("cs")]
    public List<C> Cs { get; } = new List<C>();

    [JsonProperty("da")]
    public List<Dum> Da { get; } = new List<Dum>();

    [JsonProperty("dv")]
    public List<Dv> Dv { get; } = new List<Dv>();

    [JsonProperty("nl")]
    public List<Nl> Nl { get; } = new List<Nl>();

    [JsonProperty("dz")]
    public List<Dz> Dz { get; } = new List<Dz>();

    [JsonProperty("en-orig")]
    public List<EnOrig> EnOrig { get; } = new List<EnOrig>();

    [JsonProperty("en")]
    public List<En> En { get; } = new List<En>();

    [JsonProperty("eo")]
    public List<Eo> Eo { get; } = new List<Eo>();

    [JsonProperty("et")]
    public List<Et> Et { get; } = new List<Et>();

    [JsonProperty("ee")]
    public List<Ee> Ee { get; } = new List<Ee>();

    [JsonProperty("fo")]
    public List<Fo> Fo { get; } = new List<Fo>();

    [JsonProperty("fj")]
    public List<Fj> Fj { get; } = new List<Fj>();

    [JsonProperty("fil")]
    public List<Fil> Fil { get; } = new List<Fil>();

    [JsonProperty("fi")]
    public List<Fi> Fi { get; } = new List<Fi>();

    [JsonProperty("fr")]
    public List<Fr> Fr { get; } = new List<Fr>();

    [JsonProperty("gaa")]
    public List<Gaa> Gaa { get; } = new List<Gaa>();

    [JsonProperty("gl")]
    public List<Gl> Gl { get; } = new List<Gl>();

    [JsonProperty("lg")]
    public List<Lg> Lg { get; } = new List<Lg>();

    [JsonProperty("ka")]
    public List<Ka> Ka { get; } = new List<Ka>();

    [JsonProperty("de")]
    public List<De> De { get; } = new List<De>();

    [JsonProperty("el")]
    public List<El> El { get; } = new List<El>();

    [JsonProperty("gn")]
    public List<Gn> Gn { get; } = new List<Gn>();

    [JsonProperty("gu")]
    public List<Gu> Gu { get; } = new List<Gu>();

    [JsonProperty("ht")]
    public List<Ht> Ht { get; } = new List<Ht>();

    [JsonProperty("ha")]
    public List<Ha> Ha { get; } = new List<Ha>();

    [JsonProperty("haw")]
    public List<Haw> Haw { get; } = new List<Haw>();

    [JsonProperty("iw")]
    public List<Iw> Iw { get; } = new List<Iw>();

    [JsonProperty("hi")]
    public List<Hi> Hi { get; } = new List<Hi>();

    [JsonProperty("hmn")]
    public List<Hmn> Hmn { get; } = new List<Hmn>();

    [JsonProperty("hu")]
    public List<Hu> Hu { get; } = new List<Hu>();

    [JsonProperty("is")]
    public List<Is> Is { get; } = new List<Is>();

    [JsonProperty("ig")]
    public List<Ig> Ig { get; } = new List<Ig>();

    [JsonProperty("id")]
    public List<Id> Id { get; } = new List<Id>();

    [JsonProperty("iu")]
    public List<Iu> Iu { get; } = new List<Iu>();

    [JsonProperty("ga")]
    public List<Ga> Ga { get; } = new List<Ga>();

    [JsonProperty("it")]
    public List<It> It { get; } = new List<It>();

    [JsonProperty("ja")]
    public List<Ja> Ja { get; } = new List<Ja>();

    [JsonProperty("jv")]
    public List<Jv> Jv { get; } = new List<Jv>();

    [JsonProperty("kl")]
    public List<Kl> Kl { get; } = new List<Kl>();

    [JsonProperty("kn")]
    public List<Kn> Kn { get; } = new List<Kn>();

    [JsonProperty("kk")]
    public List<Kk> Kk { get; } = new List<Kk>();

    [JsonProperty("kha")]
    public List<Kha> Kha { get; } = new List<Kha>();

    [JsonProperty("km")]
    public List<Km> Km { get; } = new List<Km>();

    [JsonProperty("rw")]
    public List<Rw> Rw { get; } = new List<Rw>();

    [JsonProperty("ko")]
    public List<Ko> Ko { get; } = new List<Ko>();

    [JsonProperty("kri")]
    public List<Kri> Kri { get; } = new List<Kri>();

    [JsonProperty("ku")]
    public List<Ku> Ku { get; } = new List<Ku>();

    [JsonProperty("ky")]
    public List<Ky> Ky { get; } = new List<Ky>();

    [JsonProperty("lo")]
    public List<Lo> Lo { get; } = new List<Lo>();

    [JsonProperty("la")]
    public List<La> La { get; } = new List<La>();

    [JsonProperty("lv")]
    public List<Lv> Lv { get; } = new List<Lv>();

    [JsonProperty("ln")]
    public List<Ln> Ln { get; } = new List<Ln>();

    [JsonProperty("lt")]
    public List<Lt> Lt { get; } = new List<Lt>();

    [JsonProperty("lua")]
    public List<Lua> Lua { get; } = new List<Lua>();

    [JsonProperty("luo")]
    public List<Luo> Luo { get; } = new List<Luo>();

    [JsonProperty("lb")]
    public List<Lb> Lb { get; } = new List<Lb>();

    [JsonProperty("mk")]
    public List<Mk> Mk { get; } = new List<Mk>();

    [JsonProperty("mg")]
    public List<Mg> Mg { get; } = new List<Mg>();

    [JsonProperty("ms")]
    public List<M> Ms { get; } = new List<M>();

    [JsonProperty("ml")]
    public List<Ml> Ml { get; } = new List<Ml>();

    [JsonProperty("mt")]
    public List<Mt> Mt { get; } = new List<Mt>();

    [JsonProperty("gv")]
    public List<Gv> Gv { get; } = new List<Gv>();

    [JsonProperty("mi")]
    public List<Mi> Mi { get; } = new List<Mi>();

    [JsonProperty("mr")]
    public List<Mr> Mr { get; } = new List<Mr>();

    [JsonProperty("mn")]
    public List<Mn> Mn { get; } = new List<Mn>();

    [JsonProperty("mfe")]
    public List<Mfe> Mfe { get; } = new List<Mfe>();

    [JsonProperty("ne")]
    public List<Ne> Ne { get; } = new List<Ne>();

    [JsonProperty("new")]
    public List<New> New { get; } = new List<New>();

    [JsonProperty("nso")]
    public List<Nso> Nso { get; } = new List<Nso>();

    [JsonProperty("no")]
    public List<No> No { get; } = new List<No>();

    [JsonProperty("ny")]
    public List<Ny> Ny { get; } = new List<Ny>();

    [JsonProperty("oc")]
    public List<Oc> Oc { get; } = new List<Oc>();

    [JsonProperty("or")]
    public List<Or> Or { get; } = new List<Or>();

    [JsonProperty("om")]
    public List<Om> Om { get; } = new List<Om>();

    [JsonProperty("os")]
    public List<O> Os { get; } = new List<O>();

    [JsonProperty("pam")]
    public List<Pam> Pam { get; } = new List<Pam>();

    [JsonProperty("ps")]
    public List<P> Ps { get; } = new List<P>();

    [JsonProperty("fa")]
    public List<Fa> Fa { get; } = new List<Fa>();

    [JsonProperty("pl")]
    public List<Pl> Pl { get; } = new List<Pl>();

    [JsonProperty("pt")]
    public List<Pt> Pt { get; } = new List<Pt>();

    [JsonProperty("pt-PT")]
    public List<PtPT> PtPT { get; } = new List<PtPT>();

    [JsonProperty("pa")]
    public List<Pa> Pa { get; } = new List<Pa>();

    [JsonProperty("qu")]
    public List<Qu> Qu { get; } = new List<Qu>();

    [JsonProperty("ro")]
    public List<Ro> Ro { get; } = new List<Ro>();

    [JsonProperty("rn")]
    public List<Rn> Rn { get; } = new List<Rn>();

    [JsonProperty("ru")]
    public List<Ru> Ru { get; } = new List<Ru>();

    [JsonProperty("sm")]
    public List<Sm> Sm { get; } = new List<Sm>();

    [JsonProperty("sg")]
    public List<Sg> Sg { get; } = new List<Sg>();

    [JsonProperty("sa")]
    public List<Sa> Sa { get; } = new List<Sa>();

    [JsonProperty("gd")]
    public List<Gd> Gd { get; } = new List<Gd>();

    [JsonProperty("sr")]
    public List<Sr> Sr { get; } = new List<Sr>();

    [JsonProperty("crs")]
    public List<Cr> Crs { get; } = new List<Cr>();

    [JsonProperty("sn")]
    public List<Sn> Sn { get; } = new List<Sn>();

    [JsonProperty("sd")]
    public List<Sd> Sd { get; } = new List<Sd>();

    [JsonProperty("si")]
    public List<Si> Si { get; } = new List<Si>();

    [JsonProperty("sk")]
    public List<Sk> Sk { get; } = new List<Sk>();

    [JsonProperty("sl")]
    public List<Sl> Sl { get; } = new List<Sl>();

    [JsonProperty("so")]
    public List<So> So { get; } = new List<So>();

    [JsonProperty("st")]
    public List<St> St { get; } = new List<St>();

    [JsonProperty("es")]
    public List<E> Es { get; } = new List<E>();

    [JsonProperty("su")]
    public List<Su> Su { get; } = new List<Su>();

    [JsonProperty("sw")]
    public List<Sw> Sw { get; } = new List<Sw>();

    [JsonProperty("ss")]
    public List<Ss> Ss { get; } = new List<Ss>();

    [JsonProperty("sv")]
    public List<Sv> Sv { get; } = new List<Sv>();

    [JsonProperty("tg")]
    public List<Tg> Tg { get; } = new List<Tg>();

    [JsonProperty("ta")]
    public List<Tum> Ta { get; } = new List<Tum>();

    [JsonProperty("tt")]
    public List<Tt> Tt { get; } = new List<Tt>();

    [JsonProperty("te")]
    public List<Te> Te { get; } = new List<Te>();

    [JsonProperty("th")]
    public List<Th> Th { get; } = new List<Th>();

    [JsonProperty("bo")]
    public List<Bo> Bo { get; } = new List<Bo>();

    [JsonProperty("ti")]
    public List<Ti> Ti { get; } = new List<Ti>();

    [JsonProperty("to")]
    public List<To> To { get; } = new List<To>();

    [JsonProperty("ts")]
    public List<T> Ts { get; } = new List<T>();

    [JsonProperty("tn")]
    public List<Tn> Tn { get; } = new List<Tn>();

    [JsonProperty("tum")]
    public List<Tum> Tum { get; } = new List<Tum>();

    [JsonProperty("tr")]
    public List<Tr> Tr { get; } = new List<Tr>();

    [JsonProperty("tk")]
    public List<Tk> Tk { get; } = new List<Tk>();

    [JsonProperty("uk")]
    public List<Uk> Uk { get; } = new List<Uk>();

    [JsonProperty("ur")]
    public List<Ur> Ur { get; } = new List<Ur>();

    [JsonProperty("ug")]
    public List<Ug> Ug { get; } = new List<Ug>();

    [JsonProperty("uz")]
    public List<Uz> Uz { get; } = new List<Uz>();

    [JsonProperty("ve")]
    public List<Ve> Ve { get; } = new List<Ve>();

    [JsonProperty("vi")]
    public List<Vi> Vi { get; } = new List<Vi>();

    [JsonProperty("war")]
    public List<War> War { get; } = new List<War>();

    [JsonProperty("cy")]
    public List<Cy> Cy { get; } = new List<Cy>();

    [JsonProperty("fy")]
    public List<Fy> Fy { get; } = new List<Fy>();

    [JsonProperty("wo")]
    public List<Wo> Wo { get; } = new List<Wo>();

    [JsonProperty("xh")]
    public List<Xh> Xh { get; } = new List<Xh>();

    [JsonProperty("yi")]
    public List<Yi> Yi { get; } = new List<Yi>();

    [JsonProperty("yo")]
    public List<Yo> Yo { get; } = new List<Yo>();

    [JsonProperty("zu")]
    public List<Zu> Zu { get; } = new List<Zu>();
}

public class Ay
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Az
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class B
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ba
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Be
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Bg
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Bho
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Bn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Bo
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Br
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class C
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ca
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ceb
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

public class Co
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Cr
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Cy
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class De
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class DownloaderOptions
{
    [JsonProperty("http_chunk_size")]
    public int? HttpChunkSize { get; set; }
}

public class Dum
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Dv
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Dz
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class E
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ee
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class El
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class En
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class EnOrig
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Eo
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Et
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Eu
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Fa
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Fi
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Fil
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Fj
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Fo
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
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

public class Fr
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Fragment
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("duration")]
    public double? Duration { get; set; }
}

public class Fy
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ga
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Gaa
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Gd
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Gl
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Gn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Gu
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Gv
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ha
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Haw
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Hi
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Hmn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Hr
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ht
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
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

public class Hu
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Hy
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Id
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ig
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Is
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class It
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Iu
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Iw
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ja
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Jv
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ka
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Kha
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Kk
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Kl
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Km
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Kn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ko
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Kri
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ku
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ky
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class La
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Lb
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Lg
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ln
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Lo
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Lt
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Lua
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Luo
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Lv
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class M
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Mfe
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Mg
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Mi
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Mk
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ml
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Mn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Mr
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Mt
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class My
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ne
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class New
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Nl
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class No
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Nso
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ny
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class O
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Oc
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Om
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Or
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class P
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Pa
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Pam
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Pl
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Pt
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class PtPT
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Qu
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Rn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ro
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
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
    public AutomaticCaptions AutomaticCaptions { get; set; }

    [JsonProperty("subtitles")]
    public Subtitles Subtitles { get; set; }

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

public class Ru
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Rw
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sa
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sd
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sg
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Si
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sk
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sl
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sm
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class So
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sq
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sr
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ss
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class St
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Su
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Subtitles
{
}

public class Sv
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Sw
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class T
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Te
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Tg
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Th
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
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

public class Ti
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Tk
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Tn
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class To
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Tr
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Tt
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Tum
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Tum2
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ug
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Uk
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ur
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Uz
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Ve
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class VersionData
{
    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("repository")]
    public string Repository { get; set; }
}

public class Vi
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class War
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Wo
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Xh
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Yi
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Yo
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class ZhHan
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class ZhHant
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Zu
{
    [JsonProperty("ext")]
    public string Ext { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}
