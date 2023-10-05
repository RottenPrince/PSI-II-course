using System.Text.Json.Serialization;

namespace SharedModels.Question.WithoutAnswer
{
    [JsonDerivedType(typeof(DecimalNumberQuestionModel), typeDiscriminator: "DecimalNumberQuestionModel")]
    public class DecimalNumberQuestionModel : BaseQuestionModel
    {

    }
}
