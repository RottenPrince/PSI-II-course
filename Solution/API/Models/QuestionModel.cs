using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class QuestionModel
    {
        public string? Question { get; set; }
        public List<string>? AnswerOptions { get; set; }
        public AnswerValidationStrategy? ValidationStrategy { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? ImageName { get; set; }
    }
}
