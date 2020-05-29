using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.Core.SPModels.User;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class TeachersQueryDecorator : ITeachersQuery
    {
        private readonly DbContext _context;

        public TeachersQueryDecorator(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPAuthenticateUser>();
            modelBuilder.Query<SPTeacherGetAll>();
            modelBuilder.Query<SPFIOOfActivityOfTeachers>();
        }

        public async Task<List<SPAuthenticateUser>> Authenticate(string login, string password)
        {
            var sqlQuery = "EXEC [dbo].[SP_Teacher_Authenticate] @login, @password";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@login", login),
                        new SqlParameter("@password", password)
                    };
            return await _context.Query<SPAuthenticateUser>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPTeacherGetAll>> GetAll()
        {
            var sqlQuery = "EXEC [dbo].[SP_Teacher_GetAll]";
           return await _context.Query<SPTeacherGetAll>().FromSql(sqlQuery).ToListAsync();
        }

        public async Task<int> ChangePassword(int id, string oldPassword, string newPassword)
        {
            var sqlQuery = "EXEC [dbo].[SP_Teacher_ChangePassword] @id, @oldPassword, @newPassword";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id",id),
                        new SqlParameter("@oldPassword", oldPassword),
                        new SqlParameter("@newPassword", newPassword)
                    };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc);
        }

        public async Task<int> ChangeLogin(int id, string newLogin)
        {
            var sqlQuery = "EXEC [dbo].[SP_Teachers_ChangeLogin] @id, @newLogin";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id",id),
                        new SqlParameter("@newLogin", newLogin)
                    };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc);
        }

        public async Task<List<SPFIOOfActivityOfTeachers>> GetActivityAll()
        {
            var sqlQuery = "EXEC [dbo].[SP_FIOOfActivityOfTeachers]";
            return await _context.Query<SPFIOOfActivityOfTeachers>().FromSql(sqlQuery).ToListAsync();
        }
    }
}
