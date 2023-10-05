using SharedModels.Question.WithAnswer;
using System.Text.Json.Serialization;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(CheckboxQuestionModel), typeDiscriminator: "CheckboxQuestionModel")]
    [JsonDerivedType(typeof(DecimalNumberQuestionModel), typeDiscriminator: "DecimalNumberQuestionModel")]
    [JsonDerivedType(typeof(TextQuestionModel), typeDiscriminator: "TextQuestionModel")]
    public class BaseQuestionModel
    {
        [JsonRequired]
        public string Question { get; set; }
        [JsonRequired]
        public string ImageName { get; set; }
    }
}
