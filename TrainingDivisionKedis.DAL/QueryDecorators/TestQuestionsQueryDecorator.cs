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
    public class TestQuestionsQueryDecorator : ITestQuestionsQuery
    {
        private readonly AppDbContext _context;

        public TestQuestionsQueryDecorator(ApplicationDbContext.AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPTestQuestionsGetRandomSet>();
        }

        public async Task<TestQuestion> Create(string text, int testId)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestQuestions_Create] @text, @testId";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@text", text),
                new SqlParameter("@testId", testId)
            };
            return await _context.TestQuestions.FromSql(sqlQuery, pc.ToArray()).FirstAsync();
        }

        public async Task<List<SPTestQuestionsGetRandomSet>> GetRandomSet(int testId)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestQuestions_GetRandomSet] @testId";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@testId", testId)
            };
            return await _context.Query<SPTestQuestionsGetRandomSet>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<List<SPTestQuestionsGetRandomSet>> GetByTestResultId(int testResultId)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestQuestions_GetByTestResultId] @testResultId";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@testResultId", testResultId)
            };
            return await _context.Query<SPTestQuestionsGetRandomSet>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }
    }
}
