using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.Core.SPModels.User;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface ITeachersQuery
    {
        Task<List<SPAuthenticateUser>> Authenticate(string login, string password);

        Task<List<SPTeacherGetAll>> GetAll();

        Task<int> ChangePassword(int id, string oldPassword, string newPassword);

        Task<int> ChangeLogin(int id, string newLogin);

        Task<List<SPFIOOfActivityOfTeachers>> GetActivityAll();
    }
}
