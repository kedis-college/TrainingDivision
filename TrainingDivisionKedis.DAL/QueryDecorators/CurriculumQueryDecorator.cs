using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.Core.SPModels.Curriculum;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class CurriculumQueryDecorator : ICurriculumQuery
    {
        private readonly DbContext _context;

        public CurriculumQueryDecorator(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPSubjectsGetByStudent>();
            modelBuilder.Query<SPGetName>();
            modelBuilder.Query<SPBoolResult>();
        }

        public async Task<List<SPSubjectsGetByStudent>> GetByStudentAndTerm(int studentId, byte termId)
        {
            var sqlQuery = "EXEC [dbo].[SP_Subjects_GetByStudentAndTerm] @id, @termId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", studentId),
                        new SqlParameter("@termId", termId)
                    };
            return await _context.Query<SPSubjectsGetByStudent>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<SPGetName> GetNameById(int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_Subjects_GetNameById] @Id";
            return await _context.Query<SPGetName>().FromSql(sqlQuery, new SqlParameter("@Id", id)).FirstOrDefaultAsync();
        }

        public async Task<SPBoolResult> CheckStudentBySubjectAndTerm(int studentId, int subjectId, byte termId)
        {
            var sqlQuery = "EXEC [dbo].[SP_Curriculum_CheckStudentBySubjectAndTerm] @studentId, @subjectId, @termId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@studentId", studentId),
                        new SqlParameter("@subjectId", subjectId),
                        new SqlParameter("@termId", termId)
                    };
            return await _context.Query<SPBoolResult>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }
    }
}
