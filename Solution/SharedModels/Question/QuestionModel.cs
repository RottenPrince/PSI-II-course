using System.ComponentModel.DataAnnotations;

namespace SharedModels.Question
{
    public class QuestionModel
    {
        public QuestionModel()
        {
            AnswerOptions = new List<string>();
        }

        [Required]
        public string? Title { get; set; }
        [Required]
        public List<string>? AnswerOptions { get; set; }
        public string? ImageName { get; set; }
    }
}
