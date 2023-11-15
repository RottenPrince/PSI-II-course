using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Managers
{
    public class QuestionSolveRunJoinRepository : Repository<QuestionSolveRunJoinModel>, IQuestionSolveRunJoinRepository
    {
        Random _rng = new Random();
        IRepository<RoomModel> _room;
        public override IQueryable<QuestionSolveRunJoinModel> Query => _context.QuestionSolveRunJoinModels
                                    .Include(srm => srm.Question)
                                    .ThenInclude(m => m.AnswerOptions);
        public QuestionSolveRunJoinRepository(AppDbContext context, IRepository<RoomModel> room) : base(context)
        {
            _room = room;
        }
        public override Task<QuestionSolveRunJoinModel?> GetById(int id)
        {
            return Query
                    .FirstOrDefaultAsync(q => q.Id == id);
        }
        public Task<List<QuestionSolveRunJoinModel>> GetSolveRun(int id)
        {
            return Query
                .Where(srm => srm.SolveRunModelID == id)
                .ToListAsync();
        }
        public async Task<QuestionSolveRunJoinModel?> GetNextQuestionInRun(int runId)
        {
            var questions = await GetSolveRun(runId);
            try
            {
                return questions.First(m => m.SelectedAnswerOption == null);
            } catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<List<QuestionSolveRunJoinModel>> GetAllQuestionRunInfo(int runId)
        {
            var questions = await GetSolveRun(runId);
            try
            {
                return questions;
            } catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<int> CreateNewSolveRun(int roomId)
        {
            var roomModel = await _room.GetById(roomId);
            if(roomModel == null)
            {
                return -1;
            }
            var questions = roomModel.Questions;
            lock (_rng)
            {
                questions = questions.OrderBy(x => _rng.Next()).ToList();
            }
            var newModel = new SolveRunModel
            {
                StartTime = DateTime.UtcNow,
                Room = roomModel,
            };
            foreach (var q in questions)
            {
                Add(new QuestionSolveRunJoinModel
                {
                    SolveRun = newModel,
                    Question = q,
                });
            }
            _context.SolveRunModels.Add(newModel);
            Save();
            return newModel.Id;
        }
    }
}
