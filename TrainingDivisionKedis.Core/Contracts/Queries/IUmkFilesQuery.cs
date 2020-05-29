using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface IUmkFilesQuery 
    {
        Task<UmkFile> Create(string name, int subjectId, byte termId, string fileName, double fileSize, string fileType);

        Task<int> Delete(int id);

        Task<UmkFile> Update(int id, string name, string fileName, double? fileSize, string fileType);
    }
}
