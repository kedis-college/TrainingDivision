using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface IMessageFileQuery
    {
        Task<MessageFile> Create(string fileName, string fileType);
    }
}
