using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.Core.Enums;
using TrainingDivisionKedis.Core.SPModels.Messages;
using TrainingDivisionKedis.Core.SPModels.Students;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class TeacherChatService : ChatService<SPStudentsGetWithSpeciality, ChatFilesConfiguration>
    {
        public TeacherChatService(IAppDbContextFactory contextFactory, IFileService<ChatFilesConfiguration> fileService, IMapper mapper) : base(contextFactory,fileService,mapper)
        {            
        }

        public override async Task<OperationDetails<List<SPStudentsGetWithSpeciality>>> GetRecipientsAsync()
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var recipients = await context.StudentsQuery().GetWithSpeciality();
                    return OperationDetails<List<SPStudentsGetWithSpeciality>>.Success(recipients);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPStudentsGetWithSpeciality>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public override async Task<OperationDetails<bool>> CreateAsync(MessageCreateRequest request)
        {
            return await CreateAsync(request, MessageTypeEnum.TeacherToStudent);
        }

        public override async Task<OperationDetails<List<SPMessagesGetContactsByUser>>> GetMessageContactsByUserAsync(int id)
        {
            return await GetMessageContactsByUserAsync(id, MessageTypeEnum.TeacherToStudent);
        }

        protected override void MarkMessagesAsReceived(int sender, int recipient)
        {
            MarkMessagesAsReceived(sender, recipient, MessageTypeEnum.StudentToTeacher);
        }
    }
}
