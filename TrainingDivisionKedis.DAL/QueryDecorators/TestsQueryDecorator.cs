using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.Tests;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class TestsQueryDecorator : ITestsQuery
    {
        private readonly AppDbContext _context;

        public TestsQueryDecorator(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPTestGetByStudentAndSubjectAndTerm>();
        }

        public async Task<Test> Create(string name, int subjectId, byte termId, short questionsPerTest, short timeLimit)
        {
            var sqlQuery = "EXEC [dbo].[SP_Tests_Create] @name, @subjectId, @termId, @questionsPerTest, @timeLimit";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@name", name),
                new SqlParameter("@subjectId", subjectId),
                new SqlParameter("@termId", termId),
                new SqlParameter("@questionsPerTest", questionsPerTest),
                new SqlParameter("@timeLimit", timeLimit)
            };
            return await _context.Tests.FromSql(sqlQuery, pc.ToArray()).FirstAsync();
        }

        public async Task<int> UpdateState(int id, bool? visibility, bool? draft, bool? active)
        {
            var sqlQuery = "EXEC [dbo].[SP_Tests_UpdateState] @id, @isVisible, @isDraft, @isActive";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@id", id),
                new SqlParameter("@isVisible", visibility ?? (object) DBNull.Value),
                new SqlParameter("@isDraft", draft ?? (object) DBNull.Value),
                new SqlParameter("@isActive", active ?? (object) DBNull.Value)
            };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery,pc.ToArray());
        }

        public async Task<List<SPTestGetByStudentAndSubjectAndTerm>> GetByStudentAndSubjectAndTerm(int studentId, int subjectId, byte termId)
        {
            var sqlQuery = "EXEC [dbo].[SP_Tests_GetByStudentAndSubjectAndTerm] @studentId, @subjectId, @termId";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@studentId", studentId),
                new SqlParameter("@subjectId", subjectId),
                new SqlParameter("@termId", termId)
            };
            return await _context.Query<SPTestGetByStudentAndSubjectAndTerm>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<int> Delete (int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_Tests_Delete] @id";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@id", id)
            };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc.ToArray());
        }
    }
}
