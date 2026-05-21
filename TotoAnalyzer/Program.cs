class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Зареждане на данни...");

        var loader = new DataLoader();
        IEnumerable<Draw> allDraws;

        try
        {
            allDraws = await loader.LoadAllAsync();
            Console.WriteLine($"Заредени {allDraws.Count()} тиража.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Грешка при зареждане: {ex.Message}");
            Console.ReadKey();
            return;
        }

        IEnumerable<Draw>? filtered = null;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("============================================");
            Console.WriteLine("            ТОТО АНАЛИЗАТОР");
            Console.WriteLine("============================================");
            Console.WriteLine(" [1]  Избери период (от година - до година)");
            Console.WriteLine(" [2]  Топ N най-чести числа");
            Console.WriteLine(" [3]  Горещи двойки");
            Console.WriteLine(" [4]  Разпределение по десетици");
            Console.WriteLine(" [5]  Heat Map");
            Console.WriteLine(" [0]  Изход");
            Console.WriteLine("============================================");
            Console.Write(" Избор: ");

            var input = Console.ReadLine()?.Trim();

            if (input == "0") break;

            if (input == "1")
            {
                Console.Write("От година: ");
                if (!int.TryParse(Console.ReadLine(), out int from))
                {
                    Console.WriteLine("Невалидна година.");
                    Console.ReadKey();
                    continue;
                }

                Console.Write("До година: ");
                if (!int.TryParse(Console.ReadLine(), out int to))
                {
                    Console.WriteLine("Невалидна година.");
                    Console.ReadKey();
                    continue;
                }

                filtered = allDraws.Where(d =>
                    d.Date == DateTime.MinValue ||
                    (d.Date.Year >= from && d.Date.Year <= to)).ToList();

                Console.WriteLine($"Периодът е зададен: {from}-{to}. Намерени {((List<Draw>)filtered).Count} тиража.");
                Console.ReadKey();
                continue;
            }

            if (filtered == null)
            {
                Console.WriteLine("\n⚠ Първо избери период (опция 1)!");
                Console.ReadKey();
                continue;
            }

            var stats = new Statistics(filtered);

            switch (input)
            {
                case "2":
                    Console.Write("Брой числа N = ");
                    if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
                    {
                        Console.WriteLine("Невалидна стойност.");
                        break;
                    }
                    Console.WriteLine($"\nПоказване на топ {n} най-чести числа от избрания период:");
                    var topNums = stats.TopNumbers(n);
                    Visualizer.PrintBarChart(topNums);
                    break;

                case "3":
                    Console.WriteLine("\nТоп 10 двойки числа, които най-често се появяват заедно:");
                    var pairs = stats.HotPairs(10);
                    int rank = 1;
                    foreach (var p in pairs)
                        Console.WriteLine($"  {rank++,2}. {p.Item1,2} и {p.Item2,2}  =>  {p.Item3} пъти");
                    break;

                case "4":
                    Console.WriteLine("\nРазпределение на изтеглените числа по диапазони:");
                    var dist = stats.Distribution();
                    foreach (var d in dist)
                        Console.WriteLine($"  {d.Key,5}: {d.Value} изтегляния");
                    break;

                case "5":
                    Console.WriteLine("\nТоплинна карта на числата 1-49:");
                    var freq = stats.Frequency();
                    Visualizer.PrintHeatMap(freq);
                    break;

                default:
                    Console.WriteLine("Невалиден избор.");
                    break;
            }

            Console.WriteLine("\nНатисни клавиш за връщане към менюто...");
            Console.ReadKey();
        }
    }
}