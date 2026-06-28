using SolicitorFinder.GeneralParser.Enums;
using SolicitorFinder.GeneralParser.Extensions;
using SolicitorFinder.GeneralParser.Interfaces;
using SolicitorFinder.GeneralParser.Models;
using System.Text;
using static SolicitorFinder.GeneralParser.ParseConstants;

namespace SolicitorFinder.GeneralParser.Core;

public sealed class HtmlParser : IHtmlParser
{
    public List<PageElement> ParseElements(string html, string selector, SearchType searchType = SearchType.All)
    {
        var elements = new List<PageElement>();
        ElementInfo elementInfo = ParseSelector(selector);
        var matches = FindElementsByTag(html, elementInfo, searchType);

        foreach (var match in matches)
        {
            var element = ParseElement(match);
            elements.Add(element);
        }

        return elements;
    }

    public string? ExtractText(string html, string selector)
    {
        var elementInfo = ParseSelector(selector);
        var matches = FindElementsByTag(html, elementInfo, SearchType.First);

        if (matches.Count == 0)
        {
            return null;
        }

        var element = ParseElement(matches[0]);

        return element.InnerText;
    }

    public string? ExtractAttribute(string html, string selector, string attribute)
    {
        ElementInfo elementInfo = ParseSelector(selector);
        var matches = FindElementsByTag(html, elementInfo, SearchType.First);

        if (matches.Count == 0)
        {
            return null;
        }

        var element = ParseElement(matches[0]);

        return element.GetAttribute(attribute);
    }

    #region Private Methods

    private ElementInfo ParseSelector(string selector)
    {
        if (!selector.Contains('.') && !selector.Contains('#'))
        {
            return new ElementInfo
            {
                Tag = selector,
                ClassName = null,
                Id = null,
            };
        }

        string tag = DefaultTag;
        string? className = null;
        string? id = null;

        var parts = selector.Split('.', '#');

        if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]) &&
            !parts[0].StartsWith('.') && !parts[0].StartsWith('#'))
        {
            tag = parts[0];
        }

        foreach (var part in parts.Skip(1))
        {
            if (selector.Contains($"#{part}"))
            {
                id = part;
            }
            else if (selector.Contains($".{part}"))
            {
                className = part;
            }
        }

        return new ElementInfo
        {
            Tag = tag,
            ClassName = className,
            Id = id,
        };
    }

    private List<string> FindElementsByTag(string html, ElementInfo elementInfo, SearchType searchType)
    {
        var matches = new List<string>();
        var index = 0;
        var openTag = $"{TagStartChar}{elementInfo.Tag}";

        while (true)
        {
            var start = html.IndexOf(openTag, index, StringComparison.OrdinalIgnoreCase);

            if (start == -1)
            {
                break;
            }

            var tagEnd = html.IndexOf(TagEndChar, start);

            if (tagEnd == -1)
            {
                break;
            }

            var fullOpenTag = html.Substring(start, tagEnd - start + 1);

            var hasClass = string.IsNullOrEmpty(elementInfo.ClassName) || HasClass(fullOpenTag, elementInfo.ClassName);
            var hasId = string.IsNullOrEmpty(elementInfo.Id) || HasId(fullOpenTag, elementInfo.Id);

            if (hasClass && hasId)
            {
                var tagName = ExtractTagName(fullOpenTag);
                var isSelfClosing = IsSelfClosingTag(fullOpenTag, tagName);

                if (isSelfClosing)
                {
                    var element = html.Substring(start, tagEnd - start + 1);
                    matches.Add(element);

                    if (searchType == SearchType.First || searchType == SearchType.Single)
                    {
                        break;
                    }

                    index = tagEnd + 1;
                }
                else
                {
                    var end = FindClosingTagWithNesting(html, start, elementInfo.Tag ?? string.Empty);
                    if (end == -1)
                    {
                        end = html.Length - 1;
                    }

                    var element = html.Substring(start, end - start + 1);
                    matches.Add(element);

                    if (searchType == SearchType.First || searchType == SearchType.Single)
                    {
                        break;
                    }

                    index = end + 1;
                }
            }
            else
            {
                index = start + 1;
            }
        }

        return matches;
    }

    private bool IsSelfClosingTag(string tagHtml, string tagName)
    {
        if (SelfClosingTags.Contains(tagName))
        {
            return true;
        }

        var trimmed = tagHtml.TrimEnd();

        return trimmed.EndsWith(SelfClosingEnd) || trimmed.EndsWith(SelfClosingEndWithSpace);
    }

    private string ExtractTagName(string tagHtml)
    {
        var start = 1;
        var end = tagHtml.IndexOfAny(new[] { SpaceChar, TagEndChar, TagCloseChar }, start);

        if (end == -1)
        {
            return tagHtml.Substring(start).Trim();
        }

        return tagHtml.Substring(start, end - start).ToLower();
    }

    private int FindClosingTagWithNesting(string html, int start, string tagName)
    {
        var openTag = $"{TagStartChar}{tagName}";
        var closeTag = $"</{tagName}>";
        var depth = 0;
        var pos = start;

        while (true)
        {
            var nextOpen = html.IndexOf(TagStartChar, pos + 1);
            if (nextOpen == -1)
            {
                return -1;
            }

            if (nextOpen + 1 < html.Length && html[nextOpen + 1] == TagCloseChar)
            {
                var closeEnd = html.IndexOf(TagEndChar, nextOpen);
                if (closeEnd == -1)
                {
                    pos = nextOpen + 1;
                    continue;
                }

                var closeNameStart = nextOpen + 2;
                var closeName = html.Substring(closeNameStart, closeEnd - closeNameStart).Trim().ToLower();

                if (closeName == tagName)
                {
                    if (depth == 0)
                    {
                        return closeEnd;
                    }
                    depth--;
                }

                pos = closeEnd;
                continue;
            }

            var tagEnd = html.IndexOf(TagEndChar, nextOpen);

            if (tagEnd == -1)
            {
                pos = nextOpen + 1;
                continue;
            }

            var nameStart = nextOpen + 1;
            var nameEnd = html.IndexOfAny(new[] { SpaceChar, TagEndChar, TagCloseChar }, nameStart);

            if (nameEnd == -1 || nameEnd > tagEnd)
            {
                pos = tagEnd;
                continue;
            }

            var currentTagName = html.Substring(nameStart, nameEnd - nameStart).ToLower();
            var fullOpenTag = html.Substring(nextOpen, tagEnd - nextOpen + 1);

            if (IsSelfClosingTag(fullOpenTag, currentTagName))
            {
                pos = tagEnd;
                continue;
            }

            if (currentTagName == tagName)
            {
                depth++;
            }

            pos = tagEnd;
        }
    }

    private bool HasClass(string tagContent, string className)
    {
        var classStart = tagContent.IndexOf(ClassAttributeFull, StringComparison.OrdinalIgnoreCase);

        if (classStart == -1)
        {
            return false;
        }

        var quoteChar = tagContent[classStart + ClassAttributeFull.Length];

        if (quoteChar != DoubleQuoteChar && quoteChar != SingleQuoteChar)
        {
            return false;
        }

        var classEnd = tagContent.IndexOf(quoteChar, classStart + ClassAttributeFull.Length + 1);

        if (classEnd == -1)
        {
            return false;
        }

        var classValue = tagContent.Substring(classStart + ClassAttributeFull.Length + 1, classEnd - classStart - ClassAttributeFull.Length - 1);
        var classes = classValue.Split(SpaceChar);

        return classes.Any(c => c.Equals(className, StringComparison.OrdinalIgnoreCase));
    }

    private bool HasId(string tagContent, string id)
    {
        var idStart = tagContent.IndexOf(IdAttributeFull, StringComparison.OrdinalIgnoreCase);

        if (idStart == -1)
        {
            return false;
        }

        var quoteChar = tagContent[idStart + IdAttributeFull.Length];

        if (quoteChar != DoubleQuoteChar && quoteChar != SingleQuoteChar)
        {
            return false;
        }

        var idEnd = tagContent.IndexOf(quoteChar, idStart + IdAttributeFull.Length + 1);

        if (idEnd == -1)
        {
            return false;
        }

        var idValue = tagContent.Substring(idStart + IdAttributeFull.Length + 1, idEnd - idStart - IdAttributeFull.Length - 1);
        return idValue.Equals(id, StringComparison.OrdinalIgnoreCase);
    }

    private PageElement ParseElement(string html)
    {
        var element = new PageElement();

        var tagStart = html.IndexOf(TagStartChar) + 1;
        var tagEnd = html.IndexOfAny(new[] { SpaceChar, TagEndChar }, tagStart);

        if (tagEnd == -1)
        {
            return element;
        }

        element.TagName = html.Substring(tagStart, tagEnd - tagStart);

        var attrStart = tagEnd;
        var attrEnd = html.IndexOf(TagEndChar, attrStart);
        if (attrEnd > attrStart)
        {
            var attrString = html.Substring(attrStart, attrEnd - attrStart);
            ParseAttributes(attrString, element);
        }

        var tagName = element.TagName?.ToLower() ?? string.Empty;

        if (SelfClosingTags.Contains(tagName))
        {
            return element;
        }

        var fullOpenTag = html.Substring(tagStart - 1, attrEnd - tagStart + 2);
        if (fullOpenTag.TrimEnd().EndsWith(SelfClosingEnd) || fullOpenTag.TrimEnd().EndsWith(SelfClosingEndWithSpace))
        {
            return element;
        }

        var closeTag = $"</{element.TagName}>";
        var closeTagStart = html.LastIndexOf(closeTag, StringComparison.OrdinalIgnoreCase);

        if (closeTagStart > attrEnd)
        {
            var contentStart = attrEnd + 1;
            var contentEnd = closeTagStart;

            element.InnerHtml = html.Substring(contentStart, contentEnd - contentStart);
            element.InnerText = StripHtml(element.InnerHtml);

            element.Children = ParseChildren(element.InnerHtml, element.TagName);

            foreach (var child in element.Children)
            {
                child.Parent = element;
            }
        }

        return element;
    }

    private List<PageElement> ParseChildren(string html, string? parentTagName = null)
    {
        PageElement element;
        string elementHtml;
        var children = new List<PageElement>();
        var index = 0;

        while (index < html.Length)
        {
            var tagStart = html.IndexOf(TagStartChar, index);

            if (tagStart == -1)
            {
                break;
            }

            if (tagStart + 1 < html.Length && html[tagStart + 1] == TagCloseChar)
            {
                break;
            }

            if (tagStart + 1 < html.Length && html[tagStart + 1] == '!')
            {
                var commentEnd = html.IndexOf(CommentEnd, tagStart + 4);
                if (commentEnd != -1)
                {
                    index = commentEnd + CommentEnd.Length;
                    continue;
                }
                index = tagStart + 1;
                continue;
            }

            if (tagStart + 1 < html.Length && html[tagStart + 1] == '?' ||
                tagStart + 1 < html.Length && tagStart + 9 < html.Length &&
                html.Substring(tagStart + 1, DoctypePrefix.Length).ToLower() == DoctypePrefix)
            {
                var doctypeEnd = html.IndexOf(TagEndChar, tagStart);

                if (doctypeEnd != -1)
                {
                    index = doctypeEnd + 1;
                    continue;
                }

                index = tagStart + 1;
                continue;
            }

            var tagEnd = html.IndexOf(TagEndChar, tagStart);

            if (tagEnd == -1)
            {
                break;
            }

            var nameStart = tagStart + 1;
            var nameEnd = html.IndexOfAny(new[] { SpaceChar, TagEndChar, TagCloseChar }, nameStart);
            if (nameEnd == -1 || nameEnd > tagEnd)
            {
                index = tagEnd + 1;
                continue;
            }

            var tagName = html.Substring(nameStart, nameEnd - nameStart).ToLower();
            var fullOpenTag = html.Substring(tagStart, tagEnd - tagStart + 1);

            if (IsSelfClosingTag(fullOpenTag, tagName))
            {
                elementHtml = html.Substring(tagStart, tagEnd - tagStart + 1);
                element = ParseElement(elementHtml);
                children.Add(element);
                index = tagEnd + 1;
                continue;
            }

            var closeTagStart = FindClosingTagWithNesting(html, tagStart, tagName);

            if (closeTagStart == -1)
            {
                var nextTagStart = html.IndexOf(TagStartChar, tagEnd + 1);
                if (nextTagStart != -1)
                {
                    if (nextTagStart + 1 < html.Length && html[nextTagStart + 1] == TagCloseChar)
                    {
                        elementHtml = html.Substring(tagStart, nextTagStart - tagStart);
                        element = ParseElement(elementHtml);
                        children.Add(element);
                        index = nextTagStart;
                        continue;
                    }

                    elementHtml = html.Substring(tagStart, nextTagStart - tagStart);
                    element = ParseElement(elementHtml);
                    children.Add(element);
                    index = nextTagStart;
                }
                else
                {
                    elementHtml = html.Substring(tagStart);
                    element = ParseElement(elementHtml);
                    children.Add(element);
                    index = html.Length;
                }

                continue;
            }

            var fullElementHtml = html.Substring(tagStart, closeTagStart - tagStart + 1);
            element = ParseElement(fullElementHtml);
            children.Add(element);

            index = closeTagStart + 1;
        }

        return children;
    }

    private string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return string.Empty;
        }

        var result = new StringBuilder();
        var inTag = false;

        foreach (var ch in html)
        {
            if (ch == TagStartChar)
            {
                inTag = true;
                continue;
            }
            if (ch == TagEndChar)
            {
                inTag = false;
                continue;
            }
            if (!inTag)
            {
                result.Append(ch);
            }
        }

        var text = result.ToString();

        while (text.Contains("  "))
        {
            text = text.Replace("  ", " ");
        }

        return text.Trim();
    }

    private void ParseAttributes(string attrString, PageElement element)
    {
        var parts = attrString.Split(SpaceChar, StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (part.Contains('='))
            {
                var kv = part.Split('=', 2);
                var key = kv[0].Trim();
                var value = kv[1].Trim(DoubleQuoteChar, SingleQuoteChar);
                element.Attributes[key] = value;

                if (key == IdAttribute)
                {
                    element.Id = value;
                }

                if (key == ClassAttribute)
                {
                    element.ClassName = value;
                }
            }
        }
    }


    #endregion Private Methods
}