using SharedModels.Question.WithAnswer;
using System.Text.Json.Serialization;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(CheckboxQuestionModel), typeDiscriminator: "CheckboxQuestionModel")]
    [JsonDerivedType(typeof(CheckboxQuestionWithAnswerModel), typeDiscriminator: "CheckboxQuestionWithAnswerModel")]
    [JsonDerivedType(typeof(DecimalNumberQuestionModel), typeDiscriminator: "DecimalNumberQuestionModel")]
    [JsonDerivedType(typeof(DecimalNumberWithAnswerModel), typeDiscriminator: "DecimalNumberWithAnswerModel")]
    [JsonDerivedType(typeof(TextQuestionModel), typeDiscriminator: "TextQuestionModel")]
    [JsonDerivedType(typeof(TextQuestionWithAnswerModel), typeDiscriminator: "TextQuestionWithAnswerModel")]
    public class BaseQuestionModel
    {
        [JsonRequired]
        public string Question { get; set; }
        [JsonRequired]
        public string ImageName { get; set; }
    }
}
