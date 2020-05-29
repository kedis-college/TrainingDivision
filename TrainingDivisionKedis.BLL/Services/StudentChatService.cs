using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.Core.Enums;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.Core.SPModels.Messages;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class StudentChatService : ChatService<SPTeacherGetAll, ChatFilesConfiguration>
    {
        public StudentChatService(IAppDbContextFactory contextFactory, IFileService<ChatFilesConfiguration> fileService, IMapper mapper) : base(contextFactory, fileService , mapper)
        {            
        }

        public override async Task<OperationDetails<List<SPTeacherGetAll>>> GetRecipientsAsync()
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var teachers = await context.TeachersQuery().GetAll();
                    return OperationDetails<List<SPTeacherGetAll>>.Success(teachers);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPTeacherGetAll>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public override async Task<OperationDetails<bool>> CreateAsync(MessageCreateRequest request)
        {
            return await CreateAsync(request, MessageTypeEnum.StudentToTeacher);
        }

        public override async Task<OperationDetails<List<SPMessagesGetContactsByUser>>> GetMessageContactsByUserAsync(int id)
        {
            return await GetMessageContactsByUserAsync(id, MessageTypeEnum.StudentToTeacher);
        }

        protected override void MarkMessagesAsReceived(int sender, int recipient)
        {
            MarkMessagesAsReceived(sender, recipient, MessageTypeEnum.TeacherToStudent);
        }
    }
}
