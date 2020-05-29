using AutoMapper;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.BLL.DTO.ControlSchedule;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.Core.SPModels.Messages;
using TrainingDivisionKedis.BLL.DTO.TestsClient;
using TrainingDivisionKedis.BLL.DTO.TestsAdmin;
using TrainingDivisionKedis.Core.SPModels;

namespace TrainingDivisionKedis.BLL.Common
{
    public class MappingProfileBLL : Profile
    {
        public MappingProfileBLL()
        {
            ProgressInStudyMapping();
            UmkFileMapping();
            ControlScheduleMapping();
            MessageMapping();
            TestMapping();
        }

        private void ProgressInStudyMapping()
        {
            CreateMap<SPProgressInStudyGetByRaspredelenieAndUser, ProgressInStudyDto>();
            CreateMap<ProgressInStudyDto, SPProgressInStudyGetByRaspredelenieAndUser>();
            CreateMap<SPProgressInStudyGetTotals, TotalsDto>();
        }

        private void UmkFileMapping()
        {
            CreateMap<CreateRequest, UmkFile>();
            CreateMap<UmkFile, CreateRequest>();
        }

        private void ControlScheduleMapping()
        {
            CreateMap<ControlSchedule, ControlScheduleDto>();
            CreateMap<ControlScheduleDto, ControlSchedule>();
            CreateMap<ControlSchedule, ControlScheduleListDto>();
        }

        private void MessageMapping()
        {
            CreateMap<MessageCreateRequest, Message>();
        }

        private void TestMapping()
        {
            CreateMap<Test, TestDto>();
            CreateMap<Test, TestCreateDto>();
            CreateMap<TestQuestion, TestQuestionDto>();
            CreateMap<TestAnswer, TestAnswerDto>();
        }
    }
}
