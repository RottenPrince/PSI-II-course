using BrainBoxAPI.Data;
using BrainBoxAPI.Models;

namespace BrainBoxAPI.Managers
{
    public interface IQuizQuestionRelationRepository : IRepository<QuizQuestionRelationModel>
    {
        public Task<int> CreateNewQuiz(int roomId, int questionAmount, string userId, ApplicationUser user);
        public Task<QuizQuestionRelationModel?> GetNextQuestionInQuiz(int runId);
        public Task<List<QuizQuestionRelationModel>> GetAllQuizQuestionsInfo(int runId);
        public Task<QuizQuestionRelationModel?> GetNextQuestionInReview(int runId, int currentQuestionIndex);
    }
}
