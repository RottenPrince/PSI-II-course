using System.ComponentModel.DataAnnotations;

namespace SharedModels.Question
{
    public class QuestionDTO
    {
        public QuestionDTO()
        {
            AnswerOptions = new List<AnswerOptionDTO>();
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public List<AnswerOptionDTO>? AnswerOptions { get; set; }
        public string? ImageName { get; set; }
    }
}
