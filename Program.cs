using System.Globalization;
using TransportRequirementAnalyser.Models;
using TransportRequirementAnalyser.Support;

internal class Program
{
    public static List<Interval> Intervals { get; set; }
    public static List<Ride> Rides { get; set; }

    private static void Main(string[] files)
    {
        // initialize intervals using two arbitrary dates tuesday 06:00 -> monday 22:00
        CreateIntervals();

        // read in all rides for all projects
        CreateRides();

        // add info from rides into intervals
        AddRideInfoToInterval();

        // print all stuff
        PrintIntervalInformation();
    }

    private static void CreateIntervals()
    {
        // a tuesday 06:00
        var minDate = new DateTime(2023, 08, 08, 6, 0, 0);

        // next tuesday 06:00
        var maxDate = new DateTime(2023, 08, 15, 6, 0, 0);

        Intervals = DateTimeCalculator.GetIntervals(minDate, maxDate);
    }

    private static void CreateRides()
    {
        // initalize rides
        Rides = new List<Ride>();

        // get all files
        var files = new string[] {
            // add full path to all tsvs files here
        };

        // read all files and create projects
        foreach (string file in files)
        {
            var projectRides = ReadProjectFile(file);
            Rides.AddRange(projectRides);
        }
    }

    private static void AddRideInfoToInterval()
    {
        foreach (var ride in Rides)
        {
            foreach (var rideInterval in ride.AllIntervalsBetweenStartAndEnd)
            {
                var interval = Intervals.Where(interval => 
                    interval.Time == rideInterval.DateTime.ToString("HH:mm") && interval.DayOfWeek == rideInterval.DayOfWeek).First();

                var requirement = interval.Requirements.Where(requirement => requirement.Year == rideInterval.Year).FirstOrDefault();

                if (requirement is null)
                {
                    interval.Requirements.Add(new Requirement() { Amount = 1, Year = rideInterval.Year });
                }
                else
                {
                    requirement.Amount++;
                }
            }
        }
    }

    private static void PrintIntervalInformation()
    {
        string filePath = $""; // Path to the CSV file

        var years = Intervals
            .SelectMany(interval => interval.Requirements.Select(requirement => requirement.Year))
            .Distinct()
            .OrderBy(year => year)
            .ToList();

        // Set the culture to use a dot as the decimal separator
        CultureInfo cultureInfo = new("en-US");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        using StreamWriter writer = new(filePath);

        // Write header row with years
        writer.Write("Day/Time");
        foreach (var year in years)
        {
            writer.Write($"\t{year}");
        }
        writer.Write("\tAVERAGE\tMODE\tMAX\tMIN\tAVERAGE+MARGIN");
        writer.WriteLine();

        // Write data rows
        foreach (var interval in Intervals)
        {
            writer.Write($"{interval.DayOfWeek} {interval.Time}");
            foreach (var year in years)
            {
                int amount = interval.Requirements?.FirstOrDefault(requirement => requirement.Year == year)?.Amount ?? 0;
                writer.Write($"\t{amount}");
            }
            writer.Write($"\t{interval.Average()}\t{interval.Mode()}\t{interval.Max()}\t{interval.Min()}\t{interval.AverageWithMarginAsInt()}");
            writer.WriteLine();
        }
    }

    private static List<Ride> ReadProjectFile(string filePath)
    {
        List<Ride> rides = new();
        try
        {
            using StreamReader reader = new(filePath);

            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();

                if (line is null)
                {
                    continue;
                }

                const char tabCharacter = '\t';
                string[] columns = line.Split(tabCharacter);

                if (columns.Length >= 2) // Ensure at least two columns are present
                {
                    if (DateTimeOffset.TryParse(columns[0], out DateTimeOffset startDateTime) &&
                        DateTimeOffset.TryParse(columns[1], out DateTimeOffset endDateTime))
                    {
                        Ride ride = new(startDateTime, endDateTime);
                        rides.Add(ride);
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format in the line: " + line);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid line format: " + line);
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"An error occurred: {exception.Message}");
        }

        return rides;
    }
}