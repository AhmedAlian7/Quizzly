using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Instructor;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Implementions
{
    public class InstructorManagementService : IInstructorManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetTotalQuizzesAsync(int InstructorId)
        {
            return await _unitOfWork.Quizzes
                .GetTotalQuizzesPerInstructor(InstructorId);
        }
        public async Task<decimal?> GetAvgScoreAsync(int InstructorId)
        {
            return await _unitOfWork.Quizzes
                .GetAvgScorePerInstructor(InstructorId);
        }
        public async Task<int> GetTotalStudentsCountAsync(int InstructorId)
        {
            return await _unitOfWork.Quizzes
                .GetTotalStudentsCountPerInstructor(InstructorId);

        }
        public async Task<List<InstructorRecentQuizDto>> GetRecentQuizzesAsync(int InstructorId)
        {
            var RecentQuizzes = await _unitOfWork.Quizzes
                .GetRecentQuizzezPerInstructor(InstructorId);

            var recentQuizzesDto = RecentQuizzes.Select(q => new InstructorRecentQuizDto
            {
                Title = q.Title,
                Attempts = q.QuizAttempts.Count(),
                IsPublished = q.IsPublished,
                CreatedAt = q.CreatedAt,
                AvgScore = q.QuizAttempts
                          .Average(qa => qa.Score),
            }).ToList();

            return recentQuizzesDto;
        }

        public async Task<InstructorDashboardDto> GetInstructorDashboardAsync(int InstructorId)
        {
            return (new InstructorDashboardDto
            {
                instructorRecentQuizDtos = await GetRecentQuizzesAsync(InstructorId),
                AvgScore = await GetAvgScoreAsync(InstructorId),
                TotalQuizzes = await GetTotalQuizzesAsync(InstructorId),
                TotalStudents = await GetTotalStudentsCountAsync(InstructorId),
            });

        }

        public async Task<List<InstructorAllQuizzesDto>> GetAllQuizzesAsync(int InstructorId)
        {
            var Quizzes = await _unitOfWork.Quizzes
               .GetAllByInstructorId(InstructorId);

            var QuizzesDto = Quizzes.Select(q => new InstructorAllQuizzesDto
            {
                Title = q.Title,
                Attempts = q.QuizAttempts.Count(),
                Questions = q.Questions.Count(),
                IsPublished = q.IsPublished,
                TimeLimit = q.DurationMintes,
                CreatedAt = q.CreatedAt,
                AvgScore = q.QuizAttempts
                          .Average(qa => qa.Score),
            }).ToList();

            return QuizzesDto;
        }

        public async Task AddQuizAsync(int instructorId, AddQuizDto dto)
        {
            var quiz = new Quiz
            {
                Title = dto.Title,
                Description = dto.Description,
                DurationMintes = dto.TimeLimit,
                ShuffleQuestions = dto.ShuffleQuestions,
                ShuffleChoices = dto.ShuffleChoices,
                InstructorId = instructorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPublished = false 
            };

            
            foreach (var q in dto.addQuestionDtos)
            {
                var question = new Question
                {
                    Text = q.Text,
                    Points = q.Points,
                    QuestionType = q.QuestionType,
                    ImageUrl = q.ImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                foreach (var c in q.Choices)
                {
                    var choice = new Choice
                    {
                        Text = c.Text,
                        IsCorrect = c.IsCorrect,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    question.Choices.Add(choice);
                }

                quiz.Questions.Add(question);
            }

            await _unitOfWork.Quizzes.AddAsync(quiz);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Instructor?> GetInstructorByUserIdAsync(string userId)
        {
            return await _unitOfWork.Instructors
                .GetByUserIdAsync(userId);
        }
    }   
}
