using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quizzly.Business.ViewModels.Analytics
{
    public class QuizPerformanceDto
    {
        public string QuizTitle { get; set; }
        public decimal AvgScore { get; set; }
        public int TotalAttempts { get; set; }
        public decimal? HighestScore { get; set; }
        public decimal? LowestScore { get; set; }
    }
}
