using System.Text.Json.Serialization;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(TextQuestionModel), typeDiscriminator: "TextQuestionModel")]
    public class TextQuestionModel : BaseQuestionWithAnswerModel
    {
        [JsonRequired]
        public bool CaseSensitive { get; set; }
    }
}
