using TransportRequirementAnalyser.Models;

namespace TransportRequirementAnalyser.Support;

public static class DateTimeCalculator
{
    public static DateTimeOffset RoundDownToQuarterHour(DateTimeOffset dateTime)
    {
        int minutes = dateTime.Minute;
        int roundedMinutes = (minutes / 15) * 15; // Round down to the nearest multiple of 15

        DateTimeOffset roundedDateTime = new(
            dateTime.Year, dateTime.Month, dateTime.Day,
            dateTime.Hour, roundedMinutes, 0,
            dateTime.Offset);

        return roundedDateTime;
    }

    public static DateTimeOffset RoundUpToQuarterHour(DateTimeOffset dateTime)
    {
        int minutes = dateTime.Minute;
        int roundedMinutes = ((minutes + 14) / 15) * 15; // Round up to the nearest multiple of 15

        const int FullHour = 60;
        if (roundedMinutes == FullHour)
        {
            // Handle the case when rounding up crosses an hour boundary
            var newDate = dateTime.AddHours(1);
            return new DateTimeOffset(new DateTime(newDate.Year, newDate.Month, newDate.Day, newDate.Hour, 0, newDate.Second), dateTime.Offset);
        }

        DateTimeOffset roundedDateTime = new(
            dateTime.Year, dateTime.Month, dateTime.Day,
            dateTime.Hour, roundedMinutes, 0,
            dateTime.Offset);

        return roundedDateTime;
    }

    public static List<Interval> GetIntervals(DateTimeOffset startDateTime, DateTimeOffset endDateTime)
    {
        List<Interval> intervals = new();

        DateTimeOffset currentInterval = startDateTime;

        while (currentInterval < endDateTime)
        {
            var newInterval = new Interval() { DayOfWeek = currentInterval.DayOfWeek, Time = currentInterval.DateTime.ToString("HH:mm"), Requirements = new List<Requirement>() };
            intervals.Add(newInterval);
            const int FifteenMinutes = 15;
            currentInterval = currentInterval.AddMinutes(FifteenMinutes);
        }

        return intervals;
    }

    public static List<DateTimeOffset> Get15MinuteIntervalsBetweenDates(DateTimeOffset startDateTime, DateTimeOffset endDateTime)
    {
        List<DateTimeOffset> intervals = new();

        const int FifteenMinutes = 15;

        DateTimeOffset currentInterval = startDateTime.Subtract(TimeSpan.FromMinutes(FifteenMinutes));

        while (currentInterval < endDateTime)
        {
            intervals.Add(currentInterval);
            currentInterval = currentInterval.AddMinutes(FifteenMinutes);
        }

        return intervals;
    }
}