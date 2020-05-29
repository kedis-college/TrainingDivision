using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class MessageFileQueryDecorator : IMessageFileQuery
    {
        private readonly AppDbContext _context;

        public MessageFileQueryDecorator(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<MessageFile> Create(string fileName, string fileType)
        {
            var sqlQuery = "EXEC [dbo].[SP_MessageFiles_Create] @fileName, @fileType";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@fileName", fileName),
                new SqlParameter("@fileType", fileType)
            };
            return await _context.MessageFiles.FromSql(sqlQuery, pc.ToArray()).FirstAsync();
        }
    }
}
