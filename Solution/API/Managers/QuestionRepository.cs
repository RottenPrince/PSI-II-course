using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedModels.Lobby;
using SharedModels.Question;
using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace API.Managers
{
    public class QuestionRepository : Repository<QuestionModel>
    {
        public override IQueryable<QuestionModel> Query => _context.Questions.Include(q => q.AnswerOptions);

        public QuestionRepository(AppDbContext context) : base(context) { }

        public override Task<QuestionModel?> GetById(int id)
        {
            return Query.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<RoomModel?> GetRoomContent(AppDbContext db, int roomId)
        {
            var roomModel = await db.Rooms
                            .Include(q => q.Questions)
                            .Include(q => q.SolveRuns)
                            .Where(q => q.Id == roomId)
                            .FirstOrDefaultAsync();
            if(roomModel == null)
            {
                return null;
            }

            return roomModel;
        }

        public async Task<QuestionSolveRunJoinModel> GetNextQuestionInRun(AppDbContext db, int runId)
        {
            var questions = await db.QuestionSolveRunJoinModels
                .Include(srm => srm.Question)
                .ThenInclude(m => m.AnswerOptions)
                .Where(srm => srm.SolveRunModelID == runId)
                .ToListAsync();
            try
            {
                return questions.First(m => m.SelectedAnswerOption == null);
            } catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<List<QuestionSolveRunJoinModel>> GetAllQuestionRunInfo(AppDbContext db, int runId)
        {
            var questions = await db.QuestionSolveRunJoinModels
                .Include(srm => srm.Question)
                .ThenInclude(m => m.AnswerOptions)
                .Where(srm => srm.SolveRunModelID == runId)
                .ToListAsync();
            try
            {
                return questions;
            } catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<int> CreateNewSolveRun(AppDbContext db, Random rng, int roomId)
        {
            var roomModel = await db.Rooms
                            .Include(room => room.Questions)
                            .Where(room => room.Id == roomId)
                            .FirstAsync();
            var questions = roomModel.Questions;
            lock (rng)
            {
                questions = questions.OrderBy(x => rng.Next()).ToList();
            }
            db.Rooms.Attach(roomModel);
            db.Questions.AttachRange(questions);
            var newModel = new SolveRunModel
            {
                StartTime = DateTime.UtcNow,
                Room = roomModel,
            };
            foreach (var q in questions)
            {
                db.QuestionSolveRunJoinModels.Add(new QuestionSolveRunJoinModel
                {
                    SolveRun = newModel,
                    Question = q,
                });
            }
            db.SolveRunModels.Add(newModel);
            db.SaveChanges();
            return newModel.Id;
        }

        private async Task<QuestionModel?> GetQuestionFromDatabase(AppDbContext db, int questionId)
        {
            return await db.Questions
                                .Include(q => q.AnswerOptions)
                                .Where(q => q.Id == questionId)
                                .FirstOrDefaultAsync();
        }

    }
}
