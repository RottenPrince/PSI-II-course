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
            CreateMap<QuestionModel, QuestionWithAnswerTransferModel>()
                .AfterMap((a, b) =>
                {
                    b.CorrectAnswerIndex = a.AnswerOptions.FindIndex(x => x.IsCorrect);
                });
            CreateMap<QuestionSolveRunJoinModel, QuestionRunTransferModel>()
                .ForMember(x => x.SelectedAnswerOption, opt => opt.Ignore())
                .AfterMap((a, b) =>
                {
                    b.SelectedAnswerOption = a.Question.AnswerOptions.FindIndex(x => x == a.SelectedAnswerOption);
                });
            CreateMap<RoomModel, RoomTransferModel>();
        }
    }
}
