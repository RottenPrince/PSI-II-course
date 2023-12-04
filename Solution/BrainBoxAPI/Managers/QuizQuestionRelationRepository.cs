using BrainBoxAPI.Data;
using BrainBoxAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainBoxAPI.Managers
{
    public class QuizQuestionRelationRepository : Repository<QuizQuestionRelationModel>, IQuizQuestionRelationRepository
    {
        Random _rng = new Random();
        IRepository<RoomModel> _room;
        public override IQueryable<QuizQuestionRelationModel> Query => _context.QuizQuestionRelationModels
                                    .Include(srm => srm.Question)
                                    .ThenInclude(m => m.AnswerOptions);
        public QuizQuestionRelationRepository(AppDbContext context, IRepository<RoomModel> room) : base(context)
        {
            _room = room;
        }
        public override Task<QuizQuestionRelationModel?> GetById(int id)
        {
            return Query
                    .FirstOrDefaultAsync(q => q.Id == id);
        }
        public Task<List<QuizQuestionRelationModel>> GetQuiz(int id)
        {
            return Query
                .Where(srm => srm.QuizModelID == id)
                .ToListAsync();
        }
        public async Task<QuizQuestionRelationModel?> GetNextQuestionInQuiz(int runId)
        {
            var questions = await GetQuiz(runId);
            try
            {
                return questions.First(m => m.SelectedAnswerOption == null);
            } catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<List<QuizQuestionRelationModel>> GetAllQuizQuestionsInfo(int runId)
        {
            var questions = await GetQuiz(runId);
            try
            {
                return questions;
            } catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<int> CreateNewQuiz(int roomId, int questionAmount, string userId, ApplicationUser user)
        {
            var roomModel = await _room.GetById(roomId);
            if(roomModel == null)
            {
                return -1;
            }
            var questions = roomModel.Questions;
            lock (_rng)
            {
                questions = questions.OrderBy(x => _rng.Next()).Take(questionAmount).ToList();
            }
            var newModel = new QuizModel
            {
                StartTime = DateTime.UtcNow,
                Room = roomModel,
                UserId = userId,
                User = user,
            };
            foreach (var q in questions)
            {
                Add(new QuizQuestionRelationModel
                {
                    Quiz = newModel,
                    Question = q,
                });
            }
            _context.QuizModels.Add(newModel);

            Save();
            return newModel.Id;
        }

        public async Task<QuizQuestionRelationModel?> GetNextQuestionInReview(int runId, int currentQuestionIndex)
        {
            var questions = await Query
                        .Where(srm => srm.QuizModelID == runId)
                        .OrderBy(q => q.Id)
                        .ToListAsync();

            if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
            {
                return questions[currentQuestionIndex];
            }
            else
            {
                return null;
            }
        }
    }
}
