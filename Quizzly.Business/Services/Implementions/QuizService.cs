using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Choice;
using Quizzly.Business.ViewModels.Question;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;
namespace Quizzly.Business.Services.Implementions
{
    public class QuizService : IQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInstructorAnalyticsService _instructorAnalyticsService;
        public QuizService(IUnitOfWork unitOfWork, IInstructorAnalyticsService instructorAnalyticsService)
        {
            _unitOfWork = unitOfWork;
            _instructorAnalyticsService = instructorAnalyticsService;
        }

        public async Task<QuizDetailsDto> GetQuizByIdAsync(int quizId)
        {
            var quiz = await _unitOfWork.Quizzes
                .GetByIdAsync(quizId, "Questions,QuizCategory,Questions.Choices");

            if (quiz == null) return null;

            var dto = new QuizDetailsDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                TimeLimit = quiz.DurationMintes,
                IsPublished = quiz.IsPublished,
                CategoryId = quiz.QuizCategoryId,
                AllowMultipleAttempts = quiz.AllowMultipleAttempts,
                MaxAttempts = quiz.MaxAttempts,
                IsAutoGraded = quiz.IsAutoGraded,
                PassingScore = quiz.PassingScore,
                ShowCorrectAnswers = quiz.ShowCorrectAnswers,
                ShowScoreImmediatlely = quiz.ShowScoreImmediatlely,
                ShuffleChoices = quiz.ShuffleChoices,
                ShuffleQuestions = quiz.ShuffleQuestions,
                StartAt = quiz.StartAt,
                EndAt = quiz.EndAt,
                AccessCode = quiz.AccessToken,
                QuestionPerformances = await _instructorAnalyticsService.GetQuestionLevelPerformanceAsync(quizId),
                CommonIncorrectAnswers = await _instructorAnalyticsService.GetCommonIncorrectAnswersAsync(quizId),
                AverageQuizTime = await _instructorAnalyticsService.GetAverageQuizTimeAsync(quizId),
                StudentScoreDistributions = await _instructorAnalyticsService.GetStudentPerformanceDistributionAsync(quizId, 5),

                Questions = quiz.Questions.Select(q => new QuestionDetailsDto
                {
                    Id = q.Id, 
                    Text = q.Text,
                    QuestionType = q.QuestionType,
                    Points = q.Points,
                    ImageUrl = q.ImageUrl, 
                    Choices = q.Choices.Select(c => new AddChoiceDto
                    {
                        Id = c.Id, 
                        Text = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                }).ToList()
            };

            return dto;
        }

        public async Task<string> PublishQuizAsync(int quizId)
        {
            var quiz = await _unitOfWork.Quizzes
                .GetByIdAsync(quizId);

            if (quiz == null)
                throw new Exception("Quiz not found.");

            quiz.IsPublished = true;
            quiz.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Quizzes.Update(quiz);
            await _unitOfWork.SaveAsync();

            return quiz.AccessToken;
        }

        public async Task UpdateQuizAsync(QuizDetailsDto dto)
        {
            var quiz = await _unitOfWork.Quizzes
                .GetByIdAsync(dto.Id, "Questions,Questions.Choices");

            if (quiz == null)
                throw new KeyNotFoundException("Quiz not found");

            quiz.Title = dto.Title;
            quiz.Description = dto.Description;
            quiz.DurationMintes = dto.TimeLimit;
            quiz.ShuffleQuestions = dto.ShuffleQuestions;
            quiz.ShuffleChoices = dto.ShuffleChoices;
            quiz.ShowCorrectAnswers = dto.ShowCorrectAnswers;
            quiz.ShowScoreImmediatlely = dto.ShowScoreImmediatlely;
            quiz.UpdatedAt = DateTime.UtcNow;
            quiz.QuizCategoryId = dto.CategoryId;
            quiz.AllowMultipleAttempts = dto.AllowMultipleAttempts;
            quiz.IsAutoGraded = dto.IsAutoGraded;
            quiz.IsPublished = dto.IsPublished;
            quiz.PassingScore = dto.PassingScore;
            quiz.MaxAttempts = dto.MaxAttempts;
            quiz.StartAt = dto.StartAt;
            quiz.EndAt = dto.EndAt;

            var existingQuestions = quiz.Questions.ToList();

            foreach (var existing in existingQuestions)
            {
                if (!dto.Questions.Any(q => q.Id == existing.Id))
                    existing.IsDeleted = true;
            }

            foreach (var questionDto in dto.Questions)
            {
                var question = existingQuestions.FirstOrDefault(q => q.Id == questionDto.Id);
                if (question == null)
                {
                    // new question
                    question = new Question
                    {
                        Text = questionDto.Text,
                        Points = questionDto.Points,
                        QuestionType = questionDto.QuestionType,
                        QuizId = quiz.Id,
                        Choices = questionDto.Choices.Select(c => new Choice
                        {
                            Text = c.Text,
                            IsCorrect = c.IsCorrect
                        }).ToList()
                    };
                    quiz.Questions.Add(question);
                }
                else
                {
                    question.Text = questionDto.Text;
                    question.Points = questionDto.Points;
                    question.QuestionType = questionDto.QuestionType;

                    var existingChoices = question.Choices.ToList();

                    foreach (var existingChoice in existingChoices)
                    {
                        if (!questionDto.Choices.Any(c => c.Id == existingChoice.Id))
                            existingChoice.IsDeleted = true;
                    }

                    foreach (var choiceDto in questionDto.Choices)
                    {
                        var choice = existingChoices.FirstOrDefault(c => c.Id == choiceDto.Id);
                        if (choice == null)
                        {
                            question.Choices.Add(new Choice
                            {
                                Text = choiceDto.Text,
                                IsCorrect = choiceDto.IsCorrect
                            });
                        }
                        else
                        {
                            choice.Text = choiceDto.Text;
                            choice.IsCorrect = choiceDto.IsCorrect;
                        }
                    }
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            var quiz = await _unitOfWork.Quizzes
                .GetByIdAsync(quizId);

            quiz.IsDeleted = true;

            await _unitOfWork.SaveAsync();

        }

    }
}
