using AutoMapper;
using SharedModels.Question.WithoutAnswer;
using System.Text.Json.Serialization;

namespace SharedModels.Question.WithAnswer
{
    [JsonDerivedType(typeof(CheckboxQuestionWithAnswerModel), typeDiscriminator: "CheckboxQuestionWithAnswerModel")]
    public class CheckboxQuestionWithAnswerModel : BaseQuestionWithAnswerModel
    {
        [JsonRequired]
        public List<string> AnswerOptions { get; set; }
        [JsonRequired]
        public int CorrectAnswerIndex { get; set; }

        public override bool Validate(string answer)
        {
            if (!int.TryParse(answer, out int intAnswer))
            {
                return false;
            }
            return intAnswer == CorrectAnswerIndex;
        }

        public override BaseQuestionModel MapToWithoutAnswer(IMapper mapper)
        {
            return mapper.Map<CheckboxQuestionModel>(this);
        } 
    }
}
