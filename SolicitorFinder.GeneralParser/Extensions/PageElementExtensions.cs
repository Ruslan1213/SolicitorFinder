using SolicitorFinder.GeneralParser.Models;

namespace SolicitorFinder.GeneralParser.Extensions;

public static class PageElementExtensions
{
    public static string? GetAttribute(this PageElement element, string name)
    {
        return element.Attributes.TryGetValue(name, out var value) ? value : null;
    }

    public static PageElement? FindFirst(this PageElement element, string selector)
    {
        var elementInfo = ParseSimpleSelector(selector);

        foreach (var child in element.Children)
        {
            if (Matches(child, elementInfo))
            {
                return child;
            }

            var result = child.FindFirst(selector);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static List<PageElement> FindAll(this PageElement element, string selector)
    {
        var result = new List<PageElement>();
        ElementInfo elementInfo = ParseSimpleSelector(selector);

        foreach (var child in element.Children)
        {
            if (Matches(child, elementInfo))
            {
                result.Add(child);
            }

            result.AddRange(child.FindAll(selector));
        }

        return result;
    }

    private static ElementInfo ParseSimpleSelector(string selector)
    {
        if (!selector.Contains('.') && !selector.Contains('#'))
        {
            return new ElementInfo
            {
                Tag = selector,
                ClassName = null,
                Id = null
            };
        }

        string tag = "div";
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
            Id = id
        };
    }

    private static bool Matches(this PageElement element, ElementInfo elementInfo)
    {
        if (!string.IsNullOrEmpty(elementInfo.Tag) && !element.TagName?.Equals(elementInfo.Tag, StringComparison.OrdinalIgnoreCase) == true)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(elementInfo.ClassName) && element.ClassName != elementInfo.ClassName)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(elementInfo.Id) && element.Id != elementInfo.Id)
        {
            return false;
        }

        return true;
    }
}
