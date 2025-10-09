

namespace Quizzly.Business.ViewModels.Analytics
{
    public class StudentScoreDistributionDto
    {
        public double RangeStart { get; set; } // Lower bound of the score range.
        public double RangeEnd { get; set; } // Upper bound of the score range.
        public int StudentCount { get; set; } // Number of students within this score range.
    }
}
