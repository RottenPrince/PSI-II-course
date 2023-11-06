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
        private static Random _rng = new Random();

        public async static Task<QuestionWithAnswerTransferModel?> GetQuestionWithAnswer(int questionId)
        {
            return await GetQuestionFromDatabase(questionId);
        }

        public static int[] GetAllQuestionIds(int roomId)
        {
            using(var db = new AppDbContext())
            {
                var roomModel = db.Rooms.Include(q => q.Questions).First(q => q.Id == roomId);
                return roomModel?.Questions.Select(q => q.Id).ToArray();
            }
        }

        public static List<QuestionWithAnswerTransferModel?> GetAllQuestions(int roomId)
        {
            using(var db = new AppDbContext())
            {
                var roomModel = db.Rooms.Include(q => q.Questions).First(q => q.Id == roomId);
                return roomModel.Questions.Select(q => new QuestionWithAnswerTransferModel
                {
                    Title = q.Title,
                    AnswerOptions = q.AnswerOptions.Select(o => o.OptionText).ToList(),
                    CorrectAnswerIndex = q.AnswerOptions.FindIndex(o => o.IsCorrect),
                    ImageName = q.ImageSource
                }).ToList();
            }
        }

        public static int GetQuestionCountInRoom(int roomId)
        {
            return GetAllQuestionIds(roomId).Length;
        }

        public static bool CreateNewQuestion(int roomId, QuestionWithAnswerTransferModel model)
        {
            using(var db = new AppDbContext())
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
                        OptionText = o,
                        IsCorrect = false,
                    }).ToList(),
                };

                newQuestionModel.AnswerOptions[model.CorrectAnswerIndex].IsCorrect = true;

                db.Add(newQuestionModel);
                db.SaveChanges();
                return true;
            }
        }

        public static string? GetRoomName(int roomId)
        {
            using(var db = new AppDbContext())
            {
                return db.Rooms.Find(roomId)?.Name;
            }
        }

        public static IEnumerable<RoomTransferModel> GetAllRooms()
        {
            using (var db = new AppDbContext())
            {
                return db.Rooms
                         .Select(r => new RoomTransferModel
                         {
                             Id = r.Id,
                             Name = r.Name,
                         })
                         .ToList();
            }
        }

        public async static Task<RoomContentStruct?> GetRoomContent(int roomId)
        {
            using(var db = new AppDbContext())
            {
                var roomModel = await db.Rooms.Include(q => q.Questions).Where(q => q.Id == roomId).FirstAsync();
                if(roomModel == null)
                {
                    return null;
                }

                return new RoomContentStruct
                {
                    RoomName = roomModel.Name,
                    QuestionAmount = roomModel.Questions.Count,
                };
            }
        }

        public static List<QuestionModel> GetQuestionsForRun(int roomId)
        {
            var questionIds = GetAllQuestionIds(roomId);
            using(var db = new AppDbContext())
            {
                var questions = db.Rooms.Include(room => room.Questions).Where(room => room.Id == roomId).First().Questions;
                lock(_rng)
                {
                    return questions.OrderBy(x => _rng.Next()).ToList();
                }
            }
        }

        private async static Task<QuestionWithAnswerTransferModel> GetQuestionFromDatabase(int questionId)
        {
            using(var db = new AppDbContext())
            {
                var questionModel = await db.Questions.Include(q => q.AnswerOptions).Where(q => q.Id == questionId).FirstAsync();
                var result = new QuestionWithAnswerTransferModel
                {
                    Title = questionModel.Title,
                    AnswerOptions = questionModel.AnswerOptions.Select(o => o.OptionText).ToList(),
                    CorrectAnswerIndex = questionModel.AnswerOptions.FindIndex(o => o.IsCorrect),
                    ImageName = questionModel.ImageSource
                };
                return result;
            }
        }
    }
}
