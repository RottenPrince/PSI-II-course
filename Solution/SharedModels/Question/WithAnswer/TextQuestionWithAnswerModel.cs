using System.Text.Json.Serialization;
using SharedModels.Question.WithoutAnswer;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(TextQuestionWithAnswerModel), typeDiscriminator: "TextQuestionWithAnswerModel")]
    public class TextQuestionWithAnswerModel : TextQuestionModel
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
    }
}
