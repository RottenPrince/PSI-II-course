using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CheckAnswerModel
    {
        [Required]
        public string? QuestionName { get; set; }
        [Required]
        public string? Answer { get; set; }
    }
}
