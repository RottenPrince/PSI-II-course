using System.ComponentModel.DataAnnotations;

namespace SharedModels.Question
{
    public class QuestionTransferModel
    {
        public QuestionTransferModel()
        {
            AnswerOptions = new List<string>();
        }

        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public List<string>? AnswerOptions { get; set; }
        public string? ImageName { get; set; }
    }
}
