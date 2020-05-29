using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels.Messages;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface IMessagesQuery
    {
        Task<List<SPMessagesGetByUsers>> GetMessagesByUsers(int firstUser, int secondUser);

        int UpdateReceivedByUsers(int sender, int recipient, byte messageType);

        Task<int> Create(int sender, int recipient, string text, byte messageType, int? messageFileId);

        Task<List<SPMessagesGetContactsByUser>> GetContactsByUser(int userId, byte messageType);
    }
}
