using BrainBoxAPI.Models;
using SharedModels.Question;
using AutoMapper;

namespace BrainBoxAPI.Data
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<AnswerOptionModel, AnswerOptionDTO>();
            CreateMap<AnswerOptionDTO, AnswerOptionModel>();
            CreateMap<QuestionModel, QuestionDTO>();
            CreateMap<QuestionModel, QuestionWithAnswerDTO>()
                .AfterMap((a, b) =>
                {
                    b.CorrectAnswerIndex = a.AnswerOptions.FindIndex(x => x.IsCorrect);
                });
            CreateMap<QuestionWithAnswerDTO, QuestionModel>()
                .AfterMap((a, b) =>
                {
                    b.AnswerOptions[a.CorrectAnswerIndex].IsCorrect = true;
                    b.AnswerOptions.ForEach(o => o.Question = b);
                });
            CreateMap<QuizQuestionRelationModel, QuizQuestionDTO>()
                .ForMember(x => x.SelectedAnswerOption, opt => opt.Ignore())
                .AfterMap((a, b) =>
                {
                    b.SelectedAnswerOption = a.Question.AnswerOptions.FindIndex(x => x == a.SelectedAnswerOption);
                });
            CreateMap<RoomModel, RoomDTO>();
        }
    }
}
