using AutoMapper;
using SharedModels.Question.WithoutAnswer;
using System.Text.Json.Serialization;

namespace SharedModels.Question.WithAnswer
{
    [JsonDerivedType(typeof(TextQuestionWithAnswerModel), typeDiscriminator: "TextQuestionWithAnswerModel")]
    public class TextQuestionWithAnswerModel : BaseQuestionWithAnswerModel
    {
        [JsonRequired]
        public string CorrectAnswer { get; set; }
        [JsonRequired]
        public bool CaseSensitive { get; set; }

        public override bool Validate(string answer)
        {
            if (CaseSensitive) { return answer == CorrectAnswer; }
            else { return answer.ToLower() == CorrectAnswer.ToLower(); }
        }

        public override BaseQuestionModel MapToWithoutAnswer(IMapper mapper)
        {
            return mapper.Map<TextQuestionModel>(this);
        }
    }
}
