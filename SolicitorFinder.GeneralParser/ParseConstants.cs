namespace SolicitorFinder.GeneralParser;

internal static class ParseConstants
{
    internal static readonly string[] SelfClosingTags = new[]
    {
        "img", "br", "hr", "input", "meta", "link",
        "area", "base", "col", "embed", "source", "track", "wbr"
    };

    internal const string DefaultTag = "div";
    internal const string ClassAttribute = "class";
    internal const string IdAttribute = "id";
    internal const string HrefAttribute = "href";
    internal const string TelPrefix = "tel:";

    internal const char TagStartChar = '<';
    internal const char TagEndChar = '>';
    internal const char TagCloseChar = '/';
    internal const char SpaceChar = ' ';
    internal const char SingleQuoteChar = '\'';
    internal const char DoubleQuoteChar = '"';

    internal const string CommentStart = "<!--";
    internal const string CommentEnd = "-->";
    internal const string DoctypePrefix = "!doctype";
    internal const string ClassAttributeFull = "class=";
    internal const string IdAttributeFull = "id=";
    internal const string SelfClosingEnd = "/>";
    internal const string SelfClosingEndWithSpace = "/ >";
}
