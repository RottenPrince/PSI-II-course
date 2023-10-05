using AutoMapper;
using SharedModels.Question.WithAnswer;
using SharedModels.Question.WithoutAnswer;

namespace API.Models
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<BaseQuestionWithAnswerModel, BaseQuestionModel>();
            CreateMap<CheckboxQuestionWithAnswerModel, CheckboxQuestionModel>();
            CreateMap<DecimalNumberWithAnswerModel, DecimalNumberQuestionModel>();
            CreateMap<TextQuestionWithAnswerModel, TextQuestionModel>();
        }
    }
}
