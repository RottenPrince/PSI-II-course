using System.Text.Json.Serialization;


namespace SharedModels.Question.WithoutAnswer
{
    public class BaseQuestionWithAnswerModel : BaseQuestionModel
    {
        public virtual bool Validate(string answer) => throw new NotImplementedException();
    }
}
