using System.Text.Json.Serialization;

namespace API.Models.Question
{
	[JsonDerivedType(typeof(DecimalNumberQuestionModel), typeDiscriminator: "DecimalNumberQuestionModel")]
    public class DecimalNumberQuestionModel : BaseQuestionModel
    {
		[JsonRequired]
		public decimal CorrectAnswer { get; set; }

		public override bool Validate(string answer)
		{
			if(!decimal.TryParse(answer, out decimal decimalAnswer))
			{
				return false;
			}
			return decimalAnswer == CorrectAnswer;
		}
    }
}
