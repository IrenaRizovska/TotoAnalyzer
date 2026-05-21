public class Statistics
{
    private readonly IEnumerable<Draw> _draws;

    public Statistics(IEnumerable<Draw> draws)
    {
        _draws = draws;
    }

    public Dictionary<int, int> TopNumbers(int n)
    {
        return _draws
            .SelectMany(d => d.Numbers)
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .Take(n)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public IEnumerable<(int, int, int)> HotPairs(int n)
    {
        return _draws
            .SelectMany(d =>
                d.Numbers.SelectMany((a, i) =>
                    d.Numbers.Skip(i + 1)
                             .Select(b => (Math.Min(a, b), Math.Max(a, b)))))
            .GroupBy(p => p)
            .Select(g => (g.Key.Item1, g.Key.Item2, g.Count()))
            .OrderByDescending(x => x.Item3)
            .Take(n);
    }

    public Dictionary<string, int> Distribution()
    {
        var ranges = new Dictionary<string, int>
        {
            { "1-10", 0 }, { "11-20", 0 }, { "21-30", 0 }, { "31-40", 0 }, { "41-49", 0 }
        };

        foreach (var num in _draws.SelectMany(d => d.Numbers))
        {
            if (num <= 10) ranges["1-10"]++;
            else if (num <= 20) ranges["11-20"]++;
            else if (num <= 30) ranges["21-30"]++;
            else if (num <= 40) ranges["31-40"]++;
            else ranges["41-49"]++;
        }

        return ranges;
    }

    public Dictionary<int, int> Frequency()
    {
        return _draws
            .SelectMany(d => d.Numbers)
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}