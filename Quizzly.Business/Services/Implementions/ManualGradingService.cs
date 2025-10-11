using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Enums;
using Quizzly.DataAccess.Repositories.Interfaces;

public class ManualGradingService : IManualGradingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public ManualGradingService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId, string include = "Answers.Question,Student.User,Quiz")
    {
        return await _unitOfWork.QuizAttempts.GetAttemptByIdAsync(attemptId, include);
    }

    // Get answers that need manual grading => ShortAnswer & Essay
    public async Task<List<Answer>> GetAnswersNeedingManualGradingAsync(int attemptId)
    {
        var attempt = await _unitOfWork.QuizAttempts
            .GetAttemptByIdAsync(attemptId, "Answers.Question,Student.User,Quiz");

        if (attempt == null)
            throw new Exception("Attempt not found.");

        var manualAnswers = attempt.Answers
            .Where(a =>
                (a.Question.QuestionType == QuestionType.ShortAnswer ||
                 a.Question.QuestionType == QuestionType.Essay) &&
                a.PointsAwarded == null)
            .ToList();

        return manualAnswers;
    }

    // Grade a single answer manually
    public async Task ManualGradeAnswerAsync(int answerId, decimal pointsAwarded)
    {
        var answer = await _unitOfWork.Answers.GetByIdAsync(answerId, "Question");

        if (answer == null)
            throw new Exception("Answer not found.");

        // Get max points from Question, not from Answer.MaxPoints
        decimal maxAllowed = answer.Question?.Points ?? 0;

        // If maxAllowed is still 0, something is wrong
        if (maxAllowed <= 0)
            throw new Exception("Question points not found or invalid.");

        if (pointsAwarded < 0 || pointsAwarded > maxAllowed)
            throw new Exception($"Points must be between 0 and {maxAllowed}");

        answer.PointsAwarded = pointsAwarded;
        answer.IsGraded = true;
        answer.GradedAt = DateTime.UtcNow;

        await _unitOfWork.SaveAsync();
    }

    // Recalculate total score for a quiz attempt
    public async Task UpdateAttemptTotalScoreAsync(int attemptId)
    {
        var attempt = await _unitOfWork.QuizAttempts.GetAttemptByIdAsync(attemptId, "Answers.Question");

        if (attempt == null)
            throw new Exception("Attempt not found.");

        var totalPoints = attempt.Answers.Sum(a => a.PointsAwarded ?? 0);

        // Calculate max score from Questions, not from Answer.MaxPoints
        var maxScore = attempt.Answers.Sum(a => a.Question?.Points ?? 0);

        if (maxScore <= 0)
        {
            throw new Exception("Cannot calculate max score - question points missing.");
        }

        attempt.Score = totalPoints;
        attempt.MaxScore = maxScore;
        attempt.Percentage = maxScore > 0 ? (totalPoints / maxScore) * 100 : 0;
        attempt.IsCompleted = true;
        attempt.IsAutoGraded = false;

        await _unitOfWork.SaveAsync();
    }

    // Check if all answers are graded
    public async Task<bool> AreAllAnswersGradedAsync(int attemptId)
    {
        var attempt = await _unitOfWork.QuizAttempts.GetAttemptByIdAsync(attemptId, "Answers.Question");

        if (attempt == null)
            throw new Exception("Attempt not found.");

        return !attempt.Answers.Any(a =>
            (a.Question.QuestionType == QuestionType.ShortAnswer ||
             a.Question.QuestionType == QuestionType.Essay) &&
            a.PointsAwarded == null);
    }

    // Get pending attempts (those that have at least one ShortAnswer/Essay with null PointsAwarded)
    public async Task<List<QuizAttempt>> GetPendingAttemptsAsync()
    {
        var query = _unitOfWork.QuizAttempts.GetQueryable("Student.User,Quiz,Answers.Question");

        query = query.Where(a => a.Answers.Any(ans =>
            (ans.Question.QuestionType == QuestionType.ShortAnswer ||
             ans.Question.QuestionType == QuestionType.Essay) &&
            ans.PointsAwarded == null));

        return query.ToList();
    }

    // Complete manual grading and send email notification to student
    public async Task CompleteManualGradingAsync(int attemptId)
    {
        // First update the attempt total score
        await UpdateAttemptTotalScoreAsync(attemptId);

        // Get the attempt with student and quiz details for email
        var attempt = await _unitOfWork.QuizAttempts
            .GetAttemptByIdAsync(attemptId, "Student.User,Quiz");

        if (attempt?.Student?.User == null || attempt.Quiz == null)
        {
            throw new Exception("Attempt, student, or quiz information not found.");
        }

        // Send email notification to student
        try
        {
            await SendGradingCompletionEmailAsync(attempt);
        }
        catch (Exception ex)
        {
            // Log the email error but don't fail the grading process
            // You might want to log this to a logging service
            Console.WriteLine($"Failed to send email notification: {ex.Message}");
        }
    }

    private async Task SendGradingCompletionEmailAsync(QuizAttempt attempt)
    {
        var studentEmail = attempt.Student.User.Email;
        var studentName = $"{attempt.Student.User.FirstName} {attempt.Student.User.LastName}";
        var quizTitle = attempt.Quiz.Title;
        var score = attempt.Score ?? 0m;
        var maxScore = attempt.MaxScore;
        var percentage = attempt.Percentage ?? 0m;

        var subject = $"Quiz Grading Complete - {quizTitle}";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
        .score-box {{ background-color: white; border: 2px solid #4CAF50; border-radius: 8px; padding: 20px; margin: 20px 0; text-align: center; }}
        .score {{ font-size: 2em; font-weight: bold; color: #4CAF50; }}
        .details {{ margin: 15px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 0.9em; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Quiz Grading Complete</h1>
        </div>
        <div class='content'>
            <p>Dear {studentName},</p>
            
            <p>We are pleased to inform you that the manual grading for your quiz <strong>""{quizTitle}""</strong> has been completed.</p>
            
            <div class='score-box'>
                <div class='score'>{score}/{maxScore}</div>
                <div>Your Score</div>
                <div style='margin-top: 10px; font-size: 1.2em; color: #666;'>{percentage:F1}%</div>
            </div>
            
            <div class='details'>
                <p><strong>Quiz:</strong> {quizTitle}</p>
                <p><strong>Score:</strong> {score} out of {maxScore} points</p>
                <p><strong>Percentage:</strong> {percentage:F1}%</p>
                <p><strong>Graded on:</strong> {DateTime.UtcNow.ToString("MMMM dd, yyyy 'at' HH:mm UTC")}</p>
            </div>
            
            <p>You can now view your detailed results and feedback in your student dashboard.</p>
            
            <p>Thank you for your participation!</p>
            
            <div class='footer'>
                <p>Best regards,<br>Quizzly Team</p>
            </div>
        </div>
    </div>
</body>
</html>";

        await _emailService.SendEmailAsync(studentEmail ?? "", subject, body, true);
    }
}