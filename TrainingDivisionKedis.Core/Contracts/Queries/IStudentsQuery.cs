using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels.Students;
using TrainingDivisionKedis.Core.SPModels.User;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface IStudentsQuery
    {
        Task<SPAuthenticateUser> Authenticate(string login, string password);

        Task<List<SPStudentsGetWithSpeciality>> GetWithSpeciality();

        Task<int> ChangePassword(int id, string oldPassword, string newPassword);

        Task<SPStudentsGetWithSpeciality> GetById(int id);
    }
}
