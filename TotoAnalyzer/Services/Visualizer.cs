public static class Visualizer
{
    public static void PrintBarChart(Dictionary<int, int> data)
    {
        if (data.Count == 0)
        {
            Console.WriteLine("Няма данни за показване.");
            return;
        }

        Console.WriteLine("\nТоп числа:\n");

        int max = data.Values.Max();

        foreach (var kvp in data)
        {
            int len = max > 0 ? (int)(kvp.Value / (double)max * 40) : 0;
            Console.WriteLine($"{kvp.Key,3} | {new string('#', len),-40} {kvp.Value}");
        }
    }

    public static void PrintHeatMap(Dictionary<int, int> freq)
    {
        Console.WriteLine("\nHeat Map (Червено=горещо, Жълто=неутрално, Циан=студено):\n");

        var ordered = freq.OrderBy(x => x.Value).ToList();
        int count = ordered.Count;

        int coldLimit = count * 30 / 100;
        int hotStart = count * 70 / 100;

        var cold = ordered.Take(coldLimit).Select(x => x.Key).ToHashSet();
        var hot = ordered.Skip(hotStart).Select(x => x.Key).ToHashSet();

        for (int i = 1; i <= 49; i++)
        {
            if (hot.Contains(i))
                Console.ForegroundColor = ConsoleColor.Red;
            else if (cold.Contains(i))
                Console.ForegroundColor = ConsoleColor.Cyan;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write($"{i,3} ");
            Console.ResetColor();

            if (i % 7 == 0)
                Console.WriteLine();
        }

        Console.WriteLine();
    }
}