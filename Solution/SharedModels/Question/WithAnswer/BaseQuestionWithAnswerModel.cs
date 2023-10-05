using AutoMapper;
using SharedModels.Question.WithoutAnswer;
using System.Text.Json.Serialization;

namespace SharedModels.Question.WithAnswer
{
    [JsonDerivedType(typeof(CheckboxQuestionWithAnswerModel), typeDiscriminator: "CheckboxQuestionWithAnswerModel")]
    [JsonDerivedType(typeof(DecimalNumberWithAnswerModel), typeDiscriminator: "DecimalNumberWithAnswerModel")]
    [JsonDerivedType(typeof(TextQuestionWithAnswerModel), typeDiscriminator: "TextQuestionWithAnswerModel")]
    public class BaseQuestionWithAnswerModel
    {
        [JsonRequired]
        public string Question { get; set; }
        [JsonRequired]
        public string ImageName { get; set; }
        public virtual bool Validate(string answer) => throw new NotImplementedException();
        public virtual BaseQuestionModel MapToWithoutAnswer(IMapper mapper) => throw new NotImplementedException();
    }
}
