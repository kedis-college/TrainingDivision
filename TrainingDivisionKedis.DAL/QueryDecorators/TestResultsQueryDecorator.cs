using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class TestResultsQueryDecorator : ITestResultsQuery
    {
        private readonly AppDbContext _context;

        public TestResultsQueryDecorator(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPTestResultsGetWithStudents>();
        }

        public async Task<TestResult> Create(int testId, int studentId)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestResults_Create] @testId, @studentId";
            List<SqlParameter> pc = new List<SqlParameter>
            {                
                new SqlParameter("@testId", testId),
                new SqlParameter("@studentId", studentId)
            };
            return await _context.TestResults.FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }

        public async Task<int> Start(int testResultId, DateTime startedAt)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestResults_Start] @testResultId, @startedAt";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@testResultId", testResultId),
                new SqlParameter("@startedAt", startedAt)
            };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc.ToArray());
        }

        public async Task<int> Finish(int testResultId, DateTime finishedAt)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestResults_Finish] @testResultId, @finishedAt";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@testResultId", testResultId),
                new SqlParameter("@finishedAt", finishedAt)
            };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc.ToArray());
        }

        public async Task<List<Group>> GetGroups(int testId)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestResults_GetGroups] @testId";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@testId", testId)
            };
            return await _context.Groups.FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPTestResultsGetWithStudents>> GetWithStudents(int testId, int groupId)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestResults_GetWithStudents] @testId, @groupId";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@testId", testId),
                new SqlParameter("@groupId", groupId)
            };
            return await _context.Query<SPTestResultsGetWithStudents>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }
    }
}
