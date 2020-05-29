using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class RaspredelenieQueryDecorator : IRaspredelenieQuery
    {
        private readonly DbContext _context;

        public RaspredelenieQueryDecorator(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPSubjectsGetByYearAndTermAndUser>();
            modelBuilder.Query<SPRaspredelenieGetByYearAndTermAndUser>();
            modelBuilder.Query<SPRaspredelenieOfYear>();
            modelBuilder.Query<SPBoolResult>();
        }

        public async Task<List<SPSubjectsGetByYearAndTermAndUser>> GetSubjectsByYearAndTermAndUser(byte yearId, byte termId, int userId)
        {
            var sqlQuery = "EXEC [dbo].[SP_Subjects_GetByYearAndTermAndUser] @yearId, @termId, @userId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@yearId", yearId),
                        new SqlParameter("@termId", termId),
                        new SqlParameter("@userId", userId)
                    };
            return await _context.Query<SPSubjectsGetByYearAndTermAndUser>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPRaspredelenieGetByYearAndTermAndUser>> GetByYearAndTermAndUser(byte yearId, byte termId, int userId)
        {
            var sqlQuery = "EXEC [dbo].[SP_Raspredelenie_GetByYearAndTermAndUser] @yearId, @termId, @userId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@yearId", yearId),
                        new SqlParameter("@termId", termId),
                        new SqlParameter("@userId", userId)
                    };
            return await _context.Query<SPRaspredelenieGetByYearAndTermAndUser>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPRaspredelenieOfYear>> GetByYear(int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_RaspredelenieOfYear] @year";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@year", id)
                    };
            return await _context.Query<SPRaspredelenieOfYear>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<SPBoolResult> CheckTeacherBySubjectAndTerm(short teacherId, int subjectId, byte termId)
        {
            var sqlQuery = "EXEC [dbo].[SP_Raspredelenie_CheckTeacherBySubjectAndTerm] @id, @subjectId, @termId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", teacherId),
                        new SqlParameter("@subjectId", subjectId),
                        new SqlParameter("@termId", termId)
                    };
            return await _context.Query<SPBoolResult>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }
    }
}
