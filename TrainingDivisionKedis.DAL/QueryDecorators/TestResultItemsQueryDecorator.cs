using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class TestResultItemsQueryDecorator : ITestResultItemsQuery
    {
        private readonly AppDbContext _context;

        public TestResultItemsQueryDecorator(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TestResultItem> Create(int testResultId, int questionId, int? answerId)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestResultItems_Create] @testResultId, @questionId, @answerId";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@testResultId", testResultId),
                new SqlParameter("@questionId", questionId),
                new SqlParameter("@answerId", answerId ?? (object) DBNull.Value)
            };
            return await _context.TestResultItems.FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }
    }
}
