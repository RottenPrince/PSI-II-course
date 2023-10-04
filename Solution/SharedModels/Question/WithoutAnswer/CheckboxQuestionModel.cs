using System.Text.Json.Serialization;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(CheckboxQuestionModel), typeDiscriminator: "CheckboxQuestionModel")]
    public class CheckboxQuestionModel : BaseQuestionWithAnswerModel
    {
        [JsonRequired]
        public List<string> AnswerOptions { get; set; }
        [JsonRequired]
        public int CorrectAnswerIndex { get; set; }
    }
}
