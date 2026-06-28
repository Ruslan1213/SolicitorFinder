namespace SolicitorFinder.Services.Constants;

internal sealed class ScraperConstants
{
    public const string BaseUrl = "https://www.solicitors.com";
    public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
    public const string UserAgentHeader = "User-Agent";
    public const string UrlTemplate = "https://www.solicitors.com/{0}-solicitors.html";

    public static class Selectors
    {
        public const string ResultItem = ".result-item";
        public const string ResultItemH2 = ".result-item .h2";
        public const string PhoneBlockLink = ".phone-block a";
        public const string LinkMapAddress = ".link-map address";
        public const string ResultItemP = ".result-item p";
        public const string StarFull = ".star-full";
        public const string StarHalf = ".star-half";
        public const string SelectClass = "jcf-hidden";
    }

    public static class Html
    {
        public const string HrefAttribute = "href";
        public const string HrefPattern = @"href\s*=\s*[""']?([^""'\s>]+)";
        public const string EnquiryFormPath = "/enquiry-form.asp";
        public const string DivTag = "div";
        public const string SpanTag = "span";
        public const string ATag = "a";
        public const string PTag = "p";
        public const string UlTag = "ul";
        public const string AddressTag = "address";
        public const string OptionsTag = "option";
        public const string SelectTag = "select";
    }

    public static class CssClasses
    {
        public const string PhoneBlock = "phone-block";
        public const string LinkMap = "link-map";
        public const string ResultItem = "result-item";
        public const string H2 = "h2";
        public const string StarFull = "star-full";
        public const string StarHalf = "star-half";
        public const string Row = "row";
        public const string ColLg12 = "col-lg-12";
    }

    public static class Defaults
    {
        public const string UnknownName = "Unknown";
        public const int Zero = 0;
        public const double ZeroDouble = 0.0;
    }

    public static class RegexPatterns
    {
        public const string ReviewCount = @"\((\d+)\)";
    }


}
