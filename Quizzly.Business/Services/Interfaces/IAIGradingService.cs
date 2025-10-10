using Quizzly.Business.ViewModels.AI;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IAIGradingService
    {
        Task<GradingResponse> AiGradeAnswerAsync(string questionText, string studentAnswer, string modelAnswer, int maxPoint);
    }
}
    