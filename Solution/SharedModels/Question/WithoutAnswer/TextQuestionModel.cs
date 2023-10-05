using System.Text.Json.Serialization;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(TextQuestionModel), typeDiscriminator: "TextQuestionModel")]
    public class TextQuestionModel : BaseQuestionModel
    {
        [JsonRequired]
        public bool CaseSensitive { get; set; }
    }
}
