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
        public QuizService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<QuizDetailsDto> GetQuizByIdAsync(int quizId)
        {
            var quiz = await _unitOfWork.Quizzes
                .GetByIdAsync(quizId, "Questions,QuizCategory,Questions.Choices");

            var dto = new QuizDetailsDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                TimeLimit = quiz.DurationMintes,
                IsPublished = quiz.IsPublished,
                CaregoryId = quiz.QuizCategoryId,
                AllowMultipleAttempts = quiz.AllowMultipleAttempts,
                MaxAttempts = quiz.MaxAttempts,
                IsAutoGraded = quiz.IsAutoGraded,
                PassingScore = quiz.PassingScore,
                ShowCorrectAnswers = quiz.ShowCorrectAnswers,
                ShowScoreImmediatlely = quiz.ShowScoreImmediatlely,
                StartAt = quiz.StartAt,
                EndAt = quiz.EndAt,
                Questions = quiz.Questions.Select(q => new QuestionDetailsDto
                {
                    Text = q.Text,
                    QuestionType = q.QuestionType,
                    Points = q.Points,
                    Choices = q.Choices.Select(c => new AddChoiceDto
                    {
                        Text = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                }).ToList()

            };

            return dto;
        }

        public async Task UpdateQuizAsync(QuizDetailsDto dto)
        {
            var quiz = await _unitOfWork.Quizzes
                .GetByIdAsync(dto.Id , "Questions,QuizCategory,Questions.Choices");

           
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            quiz.Title = dto.Title;
            quiz.Description = dto.Description;
            quiz.DurationMintes = dto.TimeLimit;
            quiz.ShuffleQuestions = dto.ShuffleQuestions;
            quiz.ShuffleChoices = dto.ShuffleChoices;
            quiz.UpdatedAt = DateTime.UtcNow;
            quiz.QuizCategoryId = dto.CaregoryId;
            quiz.AllowMultipleAttempts = dto.AllowMultipleAttempts;
            quiz.IsAutoGraded = dto.IsAutoGraded;
            quiz.PassingScore = dto.PassingScore;
            quiz.MaxAttempts = dto.MaxAttempts;
            quiz.StartAt = dto.StartAt;
            quiz.EndAt = dto.EndAt;

            quiz.Questions = dto.Questions.Select(q => new Question
            {
                Text = q.Text,
                QuestionType = q.QuestionType,
                Points = q.Points,
                Choices = q.Choices.Select(c => new Choice
                {
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            }).ToList();

            _unitOfWork.Quizzes.Update(quiz);
            await _unitOfWork.SaveAsync();

        }
    }
}
