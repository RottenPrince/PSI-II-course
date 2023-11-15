using API.Models;

namespace API.Managers
{
    public interface IQuestionSolveRunJoinRepository : IRepository<QuestionSolveRunJoinModel>
    {
        public Task<int> CreateNewSolveRun(int roomId, int questionAmount);
        public Task<QuestionSolveRunJoinModel?> GetNextQuestionInRun(int runId);
        public Task<List<QuestionSolveRunJoinModel>> GetAllQuestionRunInfo(int runId);
        public Task<QuestionSolveRunJoinModel?> GetNextQuestionInReview(int runId, int currentQuestionIndex);
    }
}
