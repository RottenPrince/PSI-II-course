
using System.Text.Json.Serialization;

namespace API.Models.Question
{
	[JsonDerivedType(typeof(IntegerQuestionModel), typeDiscriminator: "IntegerQuestionModel")]
    public class IntegerQuestionModel : BaseQuestionModel
    {
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
