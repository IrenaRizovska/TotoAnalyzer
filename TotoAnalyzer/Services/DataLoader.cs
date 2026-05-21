using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

public class DataLoader
{
    private readonly HttpClient _client;
    private const string BaseUrl = "https://info.toto.bg/statistika/6x49";
    private const string CacheFolder = "cache";

    public DataLoader()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _client.DefaultRequestHeaders.Add("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        _client.DefaultRequestHeaders.Add("Accept-Language", "bg,en;q=0.5");
    }

    public async Task<IEnumerable<Draw>> LoadAllAsync()
    {
        Directory.CreateDirectory(CacheFolder);

        var html = await _client.GetStringAsync(BaseUrl);
        var links = ExtractLinks(html);

        var allDraws = new List<Draw>();

        foreach (var link in links)
        {
            try
            {
                var fileName = Path.Combine(CacheFolder, Path.GetFileName(link));

                byte[] data = File.Exists(fileName)
                    ? await File.ReadAllBytesAsync(fileName)
                    : await DownloadAndCache(link, fileName);

                if (link.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    allDraws.AddRange(ParseTxt(data));
                else if (link.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                    allDraws.AddRange(ParseDocx(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Грешка при файл {link}: {ex.Message}");
            }
        }

        return allDraws;
    }

    private async Task<byte[]> DownloadAndCache(string link, string fileName)
    {
        var data = await _client.GetByteArrayAsync(link);
        await File.WriteAllBytesAsync(fileName, data);
        return data;
    }

    private List<string> ExtractLinks(string html)
    {
        var matches = Regex.Matches(html, @"href=""(.*?\.(txt|docx))""");

        return matches.Select(m =>
            m.Groups[1].Value.StartsWith("http")
                ? m.Groups[1].Value
                : "https://info.toto.bg" + m.Groups[1].Value
        ).ToList();
    }

    private IEnumerable<Draw> ParseTxt(byte[] bytes)
    {
        string text;
        try
        {
            text = Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            text = Encoding.GetEncoding(1251).GetString(bytes);
        }

        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var nums = Regex.Matches(line, @"\b(\d{1,2})\b")
                .Select(m => int.Parse(m.Value))
                .Where(n => n >= 1 && n <= 49)
                .ToList();

            if (nums.Count >= 6)
            {
                yield return new Draw
                {
                    Numbers = nums.Take(6).ToList(),
                    Date = DateTime.MinValue
                };
            }
        }
    }

    private IEnumerable<Draw> ParseDocx(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var doc = WordprocessingDocument.Open(stream, false);

        var body = doc.MainDocumentPart?.Document?.Body;
        if (body == null) yield break;

        var paragraphs = body.Elements<Paragraph>();

        foreach (var para in paragraphs)
        {
            var lineText = para.InnerText;

            var nums = Regex.Matches(lineText, @"\b(\d{1,2})\b")
                .Select(m => int.Parse(m.Value))
                .Where(n => n >= 1 && n <= 49)
                .ToList();

            if (nums.Count >= 6)
            {
                yield return new Draw
                {
                    Numbers = nums.Take(6).ToList(),
                    Date = DateTime.MinValue
                };
            }
        }
    }
}