namespace TransportRequirementAnalyser.Models;

public class Interval
{
    public DayOfWeek DayOfWeek { get; set; }
    public string Time { get; set; }
    public List<Requirement> Requirements { get; set; }

    public int Max()
    {
        if (Requirements == null || Requirements.Count == 0)
        {
            return 0;
        }

        return Requirements.Max(requiremnt => requiremnt.Amount);
    }

    public int Min()
    {
        if (Requirements == null || Requirements.Count == 0)
        {
            return 0;
        }

        return Requirements.Min(requiremnt => requiremnt.Amount);
    }

    public double Average()
    {
        if (Requirements == null || Requirements.Count == 0)
        {
            return 0.0;
        }

        var average = Requirements.Average(requirement => requirement.Amount);
        return Math.Round(average, 2); // Round to two decimal places
    }

    public int AverageWithMarginAsInt()
    {
        if (Requirements == null || Requirements.Count == 0)
        {
            return 0;
        }

        var average = Requirements.Average(requirement => requirement.Amount);

        // Round the average up to the nearest whole number and add 3
        const int MarginFromAverageToMax = 3;
        int roundedAverage = (int)Math.Ceiling(average) + MarginFromAverageToMax;

        return roundedAverage;
    }

    public int Mode()
    {
        if (Requirements == null || Requirements.Count == 0)
        {
            return 0;
        }

        // Sort the requirements by Amount
        var sortedRequirements = Requirements.OrderBy(requirement => requirement.Amount).ToList();
        int middleIndex = sortedRequirements.Count / 2;

        // If there is an odd number of elements, there is a single mode
        if (sortedRequirements.Count % 2 != 0)
        {
            return sortedRequirements[middleIndex].Amount;
        }
        else
        {
            // If there is an even number of elements, return the first middle value
            return sortedRequirements[middleIndex - 1].Amount;
        }
    }
}