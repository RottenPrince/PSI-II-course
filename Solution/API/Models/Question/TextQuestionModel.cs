using System.Text.Json.Serialization;

namespace API.Models.Question
{
	[JsonDerivedType(typeof(TextQuestionModel), typeDiscriminator: "TextCaseInsensitiveQuestionModel")]
    public class TextQuestionModel : BaseQuestionModel
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
