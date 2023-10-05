using AutoMapper;
using SharedModels.Question.WithoutAnswer;
using System.Text.Json.Serialization;

namespace SharedModels.Question.WithAnswer
{
    [JsonDerivedType(typeof(DecimalNumberWithAnswerModel), typeDiscriminator: "DecimalNumberWithAnswerModel")]
    public class DecimalNumberWithAnswerModel : BaseQuestionWithAnswerModel
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

        
        public override BaseQuestionModel MapToWithoutAnswer(IMapper mapper)
        {
            return mapper.Map<DecimalNumberQuestionModel>(this);
        }
    }
}
