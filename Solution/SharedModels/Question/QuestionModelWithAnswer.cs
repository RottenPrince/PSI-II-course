using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Question
{
    public class QuestionModelWithAnswer : QuestionModel
    {
        public QuestionModelWithAnswer() : base() { }

        public int? CorrectAnswerIndex { get; set; }
    }
}
