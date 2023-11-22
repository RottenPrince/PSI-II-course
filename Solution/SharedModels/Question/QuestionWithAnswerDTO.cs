using System.ComponentModel.DataAnnotations;

namespace SharedModels.Question
{
    public class QuestionWithAnswerDTO : QuestionDTO
    {
        public QuestionWithAnswerDTO() : base() { }

        [Required]
        public int CorrectAnswerIndex { get; set; }
    }
}
