using System.ComponentModel.DataAnnotations;

namespace SharedModels.Question
{
    public class QuestionTransferModel : IComparable<QuestionTransferModel>
    {
        public QuestionTransferModel()
        {
            AnswerOptions = new List<string>();
        }

        [Required]
        public string? Title { get; set; }
        [Required]
        public List<string>? AnswerOptions { get; set; }
        [Required]
        public int QuestionIndex { get; set; }
        public string? ImageName { get; set; }

        public int CompareTo(QuestionTransferModel? other)
        {
            if(other == null)
            {
                return 1; // always make sure null < non-null
            }
            else
            {
                return QuestionIndex.CompareTo(other.QuestionIndex);
            }
        }
    }
}
