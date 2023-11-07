using API.Models;
using SharedModels.Question;
using AutoMapper;

namespace API.Data
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<AnswerOptionModel, AnswerOptionTransferModel>();
            CreateMap<QuestionModel, QuestionTransferModel>();
        }
    }
}
