using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Question
{
    public class QuestionModelWithAnswer : QuestionModel
    {
        public QuestionModelWithAnswer() : base() { }

        [Required]
        public int? CorrectAnswerIndex { get; set; }
    }
}
