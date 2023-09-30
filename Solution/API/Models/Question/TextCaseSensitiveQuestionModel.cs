using System.Text.Json.Serialization;

namespace API.Models.Question
{
	[JsonDerivedType(typeof(TextCaseSensitiveQuestionModel), typeDiscriminator: "TextCaseSensitiveQuestionModel")]
    public class TextCaseSensitiveQuestionModel : BaseQuestionModel
    {
		[JsonRequired]
		public string CorrectAnswer { get; set; }

		public override bool Validate(string answer)
		{
			return answer == CorrectAnswer;
		}
    }
}
