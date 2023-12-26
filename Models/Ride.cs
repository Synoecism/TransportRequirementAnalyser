using TransportRequirementAnalyser.Support;

namespace TransportRequirementAnalyser.Models;

/*
 * a ride is represented as a single vehicle transfer in a transport
 */

public class Ride
{
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }
    public List<DateTimeOffset> AllIntervalsBetweenStartAndEnd { get; set; }

    public Ride(DateTimeOffset startDateTime, DateTimeOffset endDateTime)
    {
        StartDateTime = DateTimeCalculator.RoundDownToQuarterHour(startDateTime);
        EndDateTime = DateTimeCalculator.RoundUpToQuarterHour(endDateTime);
        AllIntervalsBetweenStartAndEnd = DateTimeCalculator.Get15MinuteIntervalsBetweenDates(StartDateTime, EndDateTime);
    }
}