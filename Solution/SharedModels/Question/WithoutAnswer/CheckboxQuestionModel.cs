using System.Text.Json.Serialization;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(CheckboxQuestionModel), typeDiscriminator: "CheckboxQuestionModel")]
    public class CheckboxQuestionModel : BaseQuestionModel
    {
        [JsonRequired]
        public List<string> AnswerOptions { get; set; }
    }
}
