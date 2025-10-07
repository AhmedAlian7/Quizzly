using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Instructor;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.Business.ViewModels.QuizCategories;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Implementions
{
    public class InstructorManagementService : IInstructorManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;

        public InstructorManagementService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
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
                IsPublished = false,
                QuizCategoryId = dto.CaregoryId,
                AllowMultipleAttempts = dto.AllowMultipleAttempts,
                IsAutoGraded = dto.IsAutoGraded,
                PassingScore = dto.PassingScore,
                MaxAttempts = dto.MaxAttempts,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                AccessToken = Guid.NewGuid().ToString("N") // N produces a 32-digit hexadecimal string without hyphens
            };


            foreach (var q in dto.addQuestionDtos)
            {
                try
                {
                    q.ImageUrl = await _fileUploadService.UploadAsync(q.ImageFile, "Questions");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error uploading main image: " + ex.Message);
                }

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

        public async Task AddQuizCategoryAsync(int instructorId, AddQuizCategoryDto dto)
        {
            var category = new QuizCategory
            {
                Name = dto.Name,
                Description = dto.Description,
                InstructorId = instructorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.QuizCategories.AddAsync(category);
            await _unitOfWork.SaveAsync();
        }
        public async Task<Instructor?> GetInstructorByUserIdAsync(string userId)
        {
            return await _unitOfWork.Instructors
                .GetByUserIdAsync(userId);
        }
    }
}
