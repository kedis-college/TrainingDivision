using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class ProgressInStudyQueryDecorator : IProgressInStudyQuery
    {
        private readonly DbContext _context;

        public ProgressInStudyQueryDecorator(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPProgressInStudyGetByRaspredelenieAndUser>();
            modelBuilder.Query<SPProgressInStudyGetByStudent>();
            modelBuilder.Query<SPProgressInStudyGetTotalsByStudent>();
            modelBuilder.Query<SPProgressInStudyGetTotals>();
        }

        public async Task<List<SPProgressInStudyGetByRaspredelenieAndUser>> GetByRaspredelenieAndUser(int raspredelenieId, int userId)
        {
            var sqlQuery = "EXEC [dbo].[SP_ProgressInStudy_GetByRaspredelenieAndUser] @raspredelenieId, @userId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@raspredelenieId", raspredelenieId),
                        new SqlParameter("@userId", userId)
                    };
            return await _context.Query<SPProgressInStudyGetByRaspredelenieAndUser>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPProgressInStudyGetByRaspredelenieAndUser>> GetByRaspredelenie(int raspredelenieId)
        {
            var sqlQuery = "EXEC [dbo].[SP_GetProgressInStudyByRaspredelenie] @raspredelenieId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@raspredelenieId", raspredelenieId)
                    };
            return await _context.Query<SPProgressInStudyGetByRaspredelenieAndUser>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPProgressInStudyGetByStudent>> GetByStudentAndTerm(int id, byte term)
        {
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", id),
                        new SqlParameter("@term", term)
                    };
            var sqlQuery = "EXEC [dbo].[SP_ProgressInStudy_GetByStudent] @id, @term";
            return await _context.Query<SPProgressInStudyGetByStudent>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPProgressInStudyGetTotals>> GetTotalsByRaspredelenie(int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_GetProgressInStudyTotals] @RaspId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@RaspId", id)
                    };
            return await _context.Query<SPProgressInStudyGetTotals>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<SPProgressInStudyGetByRaspredelenieAndUser> Update(int id, byte mod1, byte mod2, byte dop, byte itog, short userId, DateTime? date)
        {
            var sqlQuery = "EXEC [dbo].[SP_ProgressInStudy_Update] @NumRec, @Mod1, @Mod2, @Itog, @Dop, @Date, @UserId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@NumRec", id),
                        new SqlParameter("@Mod1", mod1),
                        new SqlParameter("@Mod2", mod2),
                        new SqlParameter("@Dop", dop),
                        new SqlParameter("@Itog", itog),
                        new SqlParameter("@Date", date ?? (object)DBNull.Value),
                        new SqlParameter("@UserId", userId)
                    };
            return await _context.Query<SPProgressInStudyGetByRaspredelenieAndUser>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }

        public async Task<SPProgressInStudyGetTotalsByStudent> GetTotalsByStudent(int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_ProgressInStudy_GetTotalsByStudent] @id";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", id)
                    };
            return await _context.Query<SPProgressInStudyGetTotalsByStudent>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }
    }
}
