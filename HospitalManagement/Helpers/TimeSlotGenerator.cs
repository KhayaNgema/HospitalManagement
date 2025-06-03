public static class TimeSlotGenerator
{
    public static List<(TimeSpan From, TimeSpan To)> GenerateDefaultSlots(DateTime date)
    {
        var slots = new List<(TimeSpan, TimeSpan)>();
        var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

        TimeSpan start = isWeekend ? new TimeSpan(9, 0, 0) : new TimeSpan(8, 0, 0);
        TimeSpan end = isWeekend ? new TimeSpan(14, 0, 0) : new TimeSpan(16, 0, 0);

        while (start < end)
        {
            var next = start.Add(TimeSpan.FromHours(1));
            slots.Add((start, next));
            start = next;
        }

        return slots;
    }
}