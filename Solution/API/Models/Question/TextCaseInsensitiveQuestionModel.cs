using System.Text.Json.Serialization;

namespace API.Models.Question
{
	[JsonDerivedType(typeof(TextCaseInsensitiveQuestionModel), typeDiscriminator: "TextCaseInsensitiveQuestionModel")]
    public class TextCaseInsensitiveQuestionModel : BaseQuestionModel
    {
		[JsonRequired]
		public string CorrectAnswer { get; set; }

		public override bool Validate(string answer)
		{
			return answer.ToLower() == CorrectAnswer.ToLower();
		}
    }
}
