using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.Core.Enums;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.Messages;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public abstract class ChatService<T, U> : IChatService<T> where U : FilesConfiguration
    {
        protected readonly IAppDbContextFactory _contextFactory;
        protected readonly IFileService<U> _fileService;
        protected readonly IMapper _mapper;

        protected ChatService(IAppDbContextFactory contextFactory, IFileService<U> fileService, IMapper mapper)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService)) ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }      

        public async Task<OperationDetails<FileDto>> GetFileByMessageAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var message = await context.Messages.Include(m => m.MessageFile).ThenInclude(m => m.FileType).FirstOrDefaultAsync(m => m.Id == id);
                    if (message == null)
                    {
                        throw new Exception("Сообщение не найдено");
                    }
                    if (message.MessageFile == null)
                    {
                        throw new Exception("Файл отсутствует в данном сообщении");
                    }
                    var mas = _fileService.GetFileBytes(message.MessageFile.Name);
                    var dto = new FileDto()
                    {
                        FileBytes = mas,
                        FileType = message.MessageFile.FileType.Type,
                        FileName = message.MessageFile.Name
                    };
                    return OperationDetails<FileDto>.Success(dto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<FileDto>.Failure(ex.Message, ex.Source);
                }
            }

        }

        public async Task<OperationDetails<List<SPMessagesGetByUsers>>> GetMessagesByUsersAsync(int firstUser, int secondUser)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var messages = await context.MessagesQuery().GetMessagesByUsers(firstUser, secondUser);
                    MarkMessagesAsReceived(firstUser, secondUser);
                    return OperationDetails<List<SPMessagesGetByUsers>>.Success(messages);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPMessagesGetByUsers>>.Failure(ex.Message, ex.Source);
                }
            }
        }
       
        public abstract Task<OperationDetails<List<T>>> GetRecipientsAsync();
        public abstract Task<OperationDetails<bool>> CreateAsync(MessageCreateRequest request);
        public abstract Task<OperationDetails<List<SPMessagesGetContactsByUser>>> GetMessageContactsByUserAsync(int id);
        protected abstract void MarkMessagesAsReceived(int sender, int recipient);

        protected void MarkMessagesAsReceived(int sender, int recipient, MessageTypeEnum messageType)
        {
            using (var context = _contextFactory.Create())
            {
                context.MessagesQuery().UpdateReceivedByUsers(sender, recipient, (byte)messageType);
            }
        }

        protected async Task<OperationDetails<bool>> CreateAsync(MessageCreateRequest request, MessageTypeEnum messageType)
        {
            using (var context = _contextFactory.Create())
            {
                var transaction = context.Database.BeginTransaction();
                try
                {
                    request.Validate();
                    int? messageFileId = null;
                    if (request.AppliedFile != null)
                    {
                        var fileType = request.AppliedFile.ContentType;
                        if (await FileTypeExists(fileType))
                        {
                            var fileName = await _fileService.UploadAsync(request.AppliedFile);
                            var messageFile = await context.MessageFileQuery().Create(fileName, fileType);
                            messageFileId = messageFile.Id;
                        }
                        else
                            throw new Exception("Файл \"" + request.AppliedFile.FileName + "\" не может быть загружен");
                    }
                    
                    foreach (var recipient in request.Recipients)
                    {
                        await context.MessagesQuery().Create(request.Sender, recipient, request.Text, (byte)messageType, messageFileId);
                    }

                    transaction.Commit();
                    transaction.Dispose();

                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.InnerException == null)
                        return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                    else
                        return OperationDetails<bool>.Failure(ex.InnerException.Message, ex.Source);
                }
            }
        }

        protected async Task<OperationDetails<List<SPMessagesGetContactsByUser>>> GetMessageContactsByUserAsync(int id, MessageTypeEnum messageType)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var contacts = await context.MessagesQuery().GetContactsByUser(id, (byte)messageType);
                    return OperationDetails<List<SPMessagesGetContactsByUser>>.Success(contacts);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPMessagesGetContactsByUser>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        protected async Task<bool> FileTypeExists(string contentType)
        {
            using (var context = _contextFactory.Create())
            {
                var fileType = await context.FileTypes.FirstOrDefaultAsync(f => f.Type == contentType);
                if (fileType == null)
                    return false;
                else
                    return true;
            }
        }
    }
}
