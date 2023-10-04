using System.Text.Json.Serialization;
using SharedModels.Question.WithoutAnswer;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(DecimalNumberWithAnswerModel), typeDiscriminator: "DecimalNumberWithAnswerModel")]
    public class DecimalNumberWithAnswerModel : DecimalNumberQuestionModel
    {
        [JsonRequired]
        public decimal CorrectAnswer { get; set; }

        public override bool Validate(string answer)
        {
            if (!decimal.TryParse(answer, out decimal decimalAnswer))
            {
                return false;
            }
            return decimalAnswer == CorrectAnswer;
        }
    }
}
