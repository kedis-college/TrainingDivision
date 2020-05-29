using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.ApplicationDbContext;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class TestAnswersQueryDecorator : ITestAnswersQuery
    {
        private readonly AppDbContext _context;

        public TestAnswersQueryDecorator(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TestAnswer> Create(string text, int questionId, bool isCorrect)
        {
            var sqlQuery = "EXEC [dbo].[SP_TestAnswers_Create] @text, @questionId, @isCorrect";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@text", text),
                new SqlParameter("@questionId", questionId),
                new SqlParameter("@isCorrect", isCorrect)
            };
            return await _context.TestAnswers.FromSql(sqlQuery, pc.ToArray()).FirstAsync();
        }
    }
}
