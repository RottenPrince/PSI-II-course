using System.Text.Json.Serialization;

namespace API.Models.Question
{
	[JsonDerivedType(typeof(CheckboxQuestionModel), typeDiscriminator: "CheckboxQuestionModel")]
    public class CheckboxQuestionModel : BaseQuestionModel
    {
		[JsonRequired]
		public List<string> AnswerOptions { get; set; }
		[JsonRequired]
		public int CorrectAnswer { get; set; }

		public override bool Validate(string answer)
		{
			if(!int.TryParse(answer, out int intAnswer))
			{
				return false;
			}
			return intAnswer == CorrectAnswer;
		}
    }
}
