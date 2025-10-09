using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Enums;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Implementions
{
    public class ManualGradingService : IManualGradingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManualGradingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get answers that need manual grading => ShortAnswer & Essay
        public async Task<List<Answer>> GetAnswersNeedingManualGradingAsync(int attemptId)
        {
            var attempt = await _unitOfWork.QuizAttempts.GetAttemptByIdAsync(attemptId, "Answers.Question");
            if (attempt == null)
                throw new Exception("Attempt not found.");

            var manualAnswers = attempt.Answers
                .Where(a =>
                    (a.Question.QuestionType == QuestionType.ShortAnswer ||
                     a.Question.QuestionType == QuestionType.Essay) &&
                    !a.IsGraded)
                .ToList();

            return manualAnswers;
        }

        // Grade a single answer manually
        public async Task ManualGradeAnswerAsync(int answerId, decimal pointsAwarded)
        {
            var answer = await _unitOfWork.Answers.GetByIdAsync(answerId);
            if (answer == null)
                throw new Exception("Answer not found.");

            if (pointsAwarded < 0 || pointsAwarded > answer.MaxPoints)
                throw new Exception($"Points must be between 0 and {answer.MaxPoints}");

            answer.PointsAwarded = pointsAwarded;
            answer.IsGraded = true;
            answer.GradedAt = DateTime.UtcNow;

            _unitOfWork.Answers.Update(answer);
            await _unitOfWork.SaveAsync();
        }

        // Recalculate total score for a quiz attempt
        public async Task UpdateAttemptTotalScoreAsync(int attemptId)
        {
            var attempt = await _unitOfWork.QuizAttempts.GetAttemptByIdAsync(attemptId, "Answers");
            if (attempt == null)
                throw new Exception("Attempt not found.");

            var totalPoints = attempt.Answers.Sum(a => a.PointsAwarded ?? 0);

            attempt.Score = totalPoints;
            attempt.Percentage = attempt.MaxScore > 0 ? (attempt.Score / attempt.MaxScore) * 100 : 0;
            attempt.IsCompleted = true;
            attempt.IsAutoGraded = false;

            _unitOfWork.QuizAttempts.Update(attempt);
            await _unitOfWork.SaveAsync();
        }

        // Check if all answers are graded
        public async Task<bool> AreAllAnswersGradedAsync(int attemptId)
        {
            var attempt = await _unitOfWork.QuizAttempts.GetAttemptByIdAsync(attemptId, "Answers");
            if (attempt == null)
                throw new Exception("Attempt not found.");

            return attempt.Answers.All(a => a.IsGraded);
        }
    }
}