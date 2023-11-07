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
    public static class QuestionManager
    {
        public static async Task<QuestionModel?> GetQuestionWithAnswer(AppDbContext db, int questionId)
        {
            return await GetQuestionFromDatabase(db, questionId);
        }

        public static bool CreateNewQuestion(AppDbContext db, int roomId, QuestionWithAnswerTransferModel model)
        {
            var roomModel = db.Rooms.Find(roomId);

            if(roomModel == null)
            {
                return false;
            }

            var newQuestionModel = new QuestionModel
            {
                Title = model.Title,
                ImageSource = model.ImageName,
                Room = roomModel,
                AnswerOptions = model.AnswerOptions.Select(o => new AnswerOptionModel
                {
                    OptionText = o.OptionText,
                    IsCorrect = false,
                }).ToList(),
            };

            newQuestionModel.AnswerOptions[model.CorrectAnswerIndex].IsCorrect = true;

            db.Add(newQuestionModel);
            db.SaveChanges();
            return true;
        }

        public static async Task<List<RoomModel>> GetAllRooms(AppDbContext db)
        {
            return await db.Rooms.ToListAsync();
        }

        public static async Task<RoomModel?> GetRoomContent(AppDbContext db, int roomId)
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

        public static async Task<QuestionSolveRunJoinModel> GetNextQuestionInRun(AppDbContext db, int runId)
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

        public static async Task<int> CreateNewSolveRun(AppDbContext db, Random rng, int roomId)
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

        private static async Task<QuestionModel?> GetQuestionFromDatabase(AppDbContext db, int questionId)
        {
            return await db.Questions
                                .Include(q => q.AnswerOptions)
                                .Where(q => q.Id == questionId)
                                .FirstOrDefaultAsync();
        }
    }
}
