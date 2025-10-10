using Microsoft.EntityFrameworkCore;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Analytics;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services
{
    public class InstructorAnalyticsService : IInstructorAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetTotalQuizzesAuthoredAsync(int instructorId)
        {
            return await _unitOfWork.Quizzes
                .GetTotalQuizzesPerInstructor(instructorId);

        }

        public async Task<decimal?> GetAverageQuizScoreAsync(int instructorId)
        {
            return await _unitOfWork.Quizzes
                .GetAvgScorePerInstructor(instructorId);
        }

        public async Task<List<QuizPerformanceDto>> GetQuizPerformanceAsync(int instructorId)
        {
            var quizzes = await _unitOfWork.Quizzes
                .GetAllByInstructorId(instructorId);

            var performanceList = quizzes.Select(q => new QuizPerformanceDto
            {
                QuizTitle = q.Title,
                AvgScore = q.QuizAttempts?
                 .Where(qa => qa.Quiz?.InstructorId == instructorId
                              && qa.Score.HasValue
                              && qa.Quiz.Questions.Any())
                 .Select(qa =>
                     (qa.Score.Value / (decimal)qa.Quiz.Questions.Sum(q => q.Points)) * 100
                 )
                 .DefaultIfEmpty(0)
                 .Average() ?? 0,

                TotalAttempts = q.QuizAttempts.Count,
                HighestScore = q.QuizAttempts.Any() ? q.QuizAttempts.Max(a => a.Score) : 0,
                LowestScore = q.QuizAttempts.Any() ? q.QuizAttempts.Min(a => a.Score) : 0,
            }).ToList();

            return performanceList;
        }

        public async Task<List<QuestionPerformanceDto>> GetQuestionLevelPerformanceAsync(int quizId)
        {
            // GetQueryable => build query on data coming from database before actually executing
            // select to convert each question to object of QuestionPerformanceDto
            var data = await _unitOfWork.Questions
                .GetQueryable()
                .Where(q => q.QuizId == quizId)
                .Select(q => new QuestionPerformanceDto
                {
                    QuestionId = q.Id,

                    QuestionText = q.Text,

                    TotalAttempts = q.Answers.Count(),

                    PercentCorrect = q.Answers.Count() == 0
                        ? 0
                        : ((double)q.Answers.Count(a => (bool)a.IsCorrect) / q.Answers.Count()) * 100,

                    PercentIncorrect = q.Answers.Count() == 0
                         ? 0
                        : 100 - ((double)q.Answers.Count(a => (bool)a.IsCorrect) / q.Answers.Count()) * 100

                }).ToListAsync();

            return data;

        }

        // Get The most commen incorrect answers in a quiz
        public async Task<List<CommonIncorrectAnswerDto>> GetCommonIncorrectAnswersAsync(int quizId)
        {
            var data = await _unitOfWork.Answers
                .GetQueryable()
                .Where(a => a.Question.QuizId == quizId && a.IsCorrect == false && a.ChoiceId != null)
                .GroupBy(a => new { a.QuestionId, a.ChoiceId, a.Choice.Text })
                .Select(g => new CommonIncorrectAnswerDto
                {
                    QuestionId = g.Key.QuestionId,
                    AnswerId = g.Key.ChoiceId.Value,
                    AnswerText = g.Key.Text,
                    SelectionCount = g.Count()
                })
                .OrderByDescending(x => x.SelectionCount)
                .ToListAsync();

            return data;
        }

        public async Task<TimeSpan> GetAverageQuizTimeAsync(int quizId)
        {
            return await _unitOfWork.Quizzes
                .GetAvgTime(quizId);
        }

        public async Task<List<StudentScoreDistributionDto>> GetStudentPerformanceDistributionAsync(int quizId, int numberOfRanges = 5)
        {
            var attempts = await _unitOfWork.QuizAttempts
                .GetQueryable()
                .Where(q => q.QuizId == quizId && q.Score.HasValue && q.MaxScore > 0 && q.IsCompleted)
                .Select(q => new { q.Score, q.MaxScore })
                .ToListAsync();

            if (!attempts.Any())
                return new List<StudentScoreDistributionDto>();

            var minScore = (double)attempts.Min(a => a.Score.Value);
            var maxScore = (double)attempts.Max(a => a.MaxScore);

            if (Math.Abs(maxScore - minScore) < double.Epsilon)
            {
                return new List<StudentScoreDistributionDto>
                {
                     new StudentScoreDistributionDto
                     {
                         RangeStart = Math.Round(minScore, 2),
                         RangeEnd = Math.Round(maxScore, 2),
                         StudentCount = attempts.Count
                     }
                };
            }

            double rangeSize = (maxScore - minScore) / numberOfRanges;

            var distribution = new List<StudentScoreDistributionDto>();
            for (int i = 0; i < numberOfRanges; i++)
            {
                double start = minScore + i * rangeSize;
                double end = (i == numberOfRanges - 1) ? maxScore : start + rangeSize;

                int count = attempts.Count(a => (double)a.Score.Value >= start && (i == numberOfRanges - 1 ? (double)a.Score.Value <= end : (double)a.Score.Value < end));

                distribution.Add(new StudentScoreDistributionDto
                {
                    RangeStart = Math.Round(start, 2),
                    RangeEnd = Math.Round(end, 2),
                    StudentCount = count
                });
            }
            return distribution;
        }

        public async Task<List<TopPreformingStudentDto>> GetTopPreformingStudentAsync(int InstructorId)
        {
           var students = await _unitOfWork.Students
                .GetTopStudentsByInstructorIdAsync(InstructorId);

            var topStudents = students.Select(s => new TopPreformingStudentDto
            {
               AvgScore = s.QuizAttempts?
                     .Where(qa => qa.Quiz?.InstructorId == InstructorId
                                  && qa.Score.HasValue
                                  && qa.Quiz.Questions.Any())
                     .Select(qa =>
                     {
                         var totalPoints = qa.Quiz.Questions.Sum(q => q.Points);
                         return totalPoints > 0
                             ? (qa.Score.Value / (decimal)totalPoints) * 100
                             : 0;
                     })
                     .DefaultIfEmpty(0)
                     .Average() ?? 0,

                StudentName = s.User.FirstName + s.User.LastName,
                Rank = 0
                

            }).ToList();
            for (int i = 0; i < topStudents.Count; i++)
            {
                topStudents[i].Rank = i + 1;
            }

            return topStudents ;
        }

        public async Task<int> GetTotalCategoriesByInstructorAsync(int instructorId)
        {
           return await _unitOfWork.QuizCategories
                .GetTotalByInstructorIdAsync(instructorId);
        }

    }
}
