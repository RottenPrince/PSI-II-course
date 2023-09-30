using System.Text.Json.Serialization;


namespace API.Models.Question
{
	[JsonDerivedType(typeof(CheckboxQuestionModel), typeDiscriminator: "CheckboxQuestionModel")]
	[JsonDerivedType(typeof(DecimalNumberQuestionModel), typeDiscriminator: "DecimalNumberQuestionModel")]
	[JsonDerivedType(typeof(IntegerQuestionModel), typeDiscriminator: "IntegerQuestionModel")]
	[JsonDerivedType(typeof(TextCaseInsensitiveQuestionModel), typeDiscriminator: "TextCaseInsensitiveQuestionModel")]
	[JsonDerivedType(typeof(TextCaseSensitiveQuestionModel), typeDiscriminator: "TextCaseSensitiveQuestionModel")]
    public class BaseQuestionModel
    {
		[JsonRequired]
        public string Question { get; set; }
		[JsonRequired]
        public string ImageName { get; set; }

		public virtual bool Validate(string answer) => throw new System.NotImplementedException();
    }
}
