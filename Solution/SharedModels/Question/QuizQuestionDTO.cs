using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Question
{
    public class QuizQuestionDTO
    {
        public QuestionWithAnswerDTO Question { get; set; }
        public int SelectedAnswerOption { get; set; }
    }
}
