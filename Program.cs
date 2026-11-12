using System.Text.Json;

try
{
    if (args.Length != 1) throw new ArgumentException("Pass one JSON input file");
    using var data = JsonDocument.Parse(File.ReadAllText(args[0]));
    Console.WriteLine(JsonSerializer.Serialize(Analyze(data.RootElement)));
    return 0;
}
catch (Exception error) when (error is IOException or JsonException or FormatException
                               or ArgumentException or InvalidOperationException
                               or KeyNotFoundException or OverflowException)
{
    Console.Error.WriteLine(error.Message);
    return 2;
}

static object Analyze(JsonElement input)
{
    var now = DateOnly.ParseExact(input.GetProperty("asOf").GetString()!, "yyyy-MM-dd");
    var schedule = new List<(string name, DateOnly date, string trigger)>();
    foreach (var filter in input.GetProperty("filters").EnumerateArray())
    {
        var name = filter.GetProperty("name").GetString();
        var last = DateOnly.ParseExact(filter.GetProperty("lastService").GetString()!, "yyyy-MM-dd");
        var days = filter.GetProperty("days").GetInt32();
        var used = filter.GetProperty("usedHours").GetDecimal();
        var limit = filter.GetProperty("limitHours").GetDecimal();
        var daily = filter.GetProperty("hoursPerDay").GetDecimal();
        if (string.IsNullOrWhiteSpace(name) || last > now || days < 1 || used < 0 || limit <= 0 || daily <= 0)
            throw new ArgumentException("Invalid filter service information");
        var dateLimit = last.AddDays(days);
        var hourLimit = now.AddDays((int)Math.Ceiling(Math.Max(0, limit - used) / daily));
        schedule.Add(dateLimit <= hourLimit ? (name, dateLimit, "calendar") : (name, hourLimit, "operating hours"));
    }
    var sorted = schedule.OrderBy(item => item.date).ThenBy(item => item.name, StringComparer.Ordinal).ToArray();
    return new {
        count = sorted.Length,
        next = sorted.FirstOrDefault().name,
        nextDate = sorted.Length == 0 ? null : sorted[0].date.ToString("yyyy-MM-dd"),
        schedule = sorted.Select(item => new { item.name, item.date, item.trigger, overdue = item.date < now })
    };
}
