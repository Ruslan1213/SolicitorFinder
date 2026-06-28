namespace SolicitorFinder.GeneralParser.Models;

public sealed class ParseResult
{
    public string? Url { get; set; }

    public string? Title { get; set; }

    public List<PageElement> Elements { get; set; } = new();

    public Dictionary<string, object> ExtractedData { get; set; } = new();

    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public TimeSpan ParseTime { get; set; }

    public DateTime ParsedAt { get; set; } = DateTime.UtcNow;

    public PageElement? FirstOrDefault(Func<object, bool> value)
    {
        throw new NotImplementedException();
    }

    public T? GetData<T>(string key)
    {
        if (ExtractedData.TryGetValue(key, out var value))
        {
            return (T?)value;
        }
        return default;
    }

    public void SetData(string key, object value)
    {
        ExtractedData[key] = value;
    }
}
