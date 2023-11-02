using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Database
{
    public class Room
    {
        public int RoomId { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }

        [Required]
        public string Title { get; set; }

        public string ImageSouce { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public List<AnswerOption> AnswerOptions { get; set; }
    }

    public class AnswerOption
    {
        public int AnswerOptionId { get; set; }

        [Required]
        public string OptionText { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
