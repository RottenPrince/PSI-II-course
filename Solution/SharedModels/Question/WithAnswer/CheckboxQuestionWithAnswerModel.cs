using System.Text.Json.Serialization;
using SharedModels.Question.WithoutAnswer;

namespace SharedModels.Question.WithAnswer
{
    [JsonDerivedType(typeof(CheckboxQuestionWithAnswerModel), typeDiscriminator: "CheckboxQuestionWithAnswerModel")]
    public class CheckboxQuestionWithAnswerModel : BaseQuestionWithAnswerModel
    {
        [JsonRequired]
        public int CorrectAnswer { get; set; }

        public override bool Validate(string answer)
        {
            if (!int.TryParse(answer, out int intAnswer))
            {
                return false;
            }
            return intAnswer == CorrectAnswer;
        }
    }
}
