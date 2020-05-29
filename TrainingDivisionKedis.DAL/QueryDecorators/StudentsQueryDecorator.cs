using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.Core.SPModels.Students;
using TrainingDivisionKedis.Core.SPModels.User;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.Core.SPModels;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class StudentsQueryDecorator : IStudentsQuery
    {
        private readonly DbContext _context;

        public StudentsQueryDecorator(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPAuthenticateUser>();
            modelBuilder.Query<SPStudentsGetWithSpeciality>();
        }

        public async Task<SPAuthenticateUser> Authenticate(string login, string password)
        {
            var sqlQuery = "EXEC [dbo].[SP_Student_Authenticate] @login, @password";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@login", login),
                        new SqlParameter("@password", password)
                    };
            return await _context.Query<SPAuthenticateUser>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }

        public async Task<List<SPStudentsGetWithSpeciality>> GetWithSpeciality()
        {
            var sqlQuery = "EXEC [dbo].[SP_Students_GetWithSpeciality]";
           return await _context.Query<SPStudentsGetWithSpeciality>().FromSql(sqlQuery).ToListAsync();
        }

        public async Task<int> ChangePassword(int id, string oldPassword, string newPassword)
        {
            var sqlQuery = "EXEC [dbo].[SP_Students_ChangePassword] @id, @oldPassword, @newPassword";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id",id),
                        new SqlParameter("@oldPassword", oldPassword),
                        new SqlParameter("@newPassword", newPassword)
                    };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc);
        }

        public async Task<SPStudentsGetWithSpeciality> GetById(int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_Students_GetById] @id";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", id)
                    };
            return await _context.Query<SPStudentsGetWithSpeciality>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }
    }
}
