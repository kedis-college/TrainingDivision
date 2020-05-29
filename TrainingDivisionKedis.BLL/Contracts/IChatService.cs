using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.Messages;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IChatService<T>
    {
        Task<OperationDetails<bool>> CreateAsync(MessageCreateRequest request);
        Task<OperationDetails<List<T>>> GetRecipientsAsync();
        Task<OperationDetails<List<SPMessagesGetByUsers>>> GetMessagesByUsersAsync(int firstUser, int secondUser);
        Task<OperationDetails<List<SPMessagesGetContactsByUser>>> GetMessageContactsByUserAsync(int id);
        Task<OperationDetails<FileDto>> GetFileByMessageAsync(int id);
    }
}
