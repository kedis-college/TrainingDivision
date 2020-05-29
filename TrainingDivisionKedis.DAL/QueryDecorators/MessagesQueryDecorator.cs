using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.Core.SPModels.Messages;
using TrainingDivisionKedis.Core.Contracts.Queries;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class MessagesQueryDecorator : IMessagesQuery
    {
        private readonly DbContext _context;

        public MessagesQueryDecorator(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<SPMessagesGetContactsByUser>();
            modelBuilder.Query<SPMessagesGetByUsers>();
        }

        public async Task<List<SPMessagesGetByUsers>> GetMessagesByUsers(int firstUser, int secondUser)
        {
            var sqlQuery = "EXEC [dbo].[SP_Messages_GetByUsers] @firstUser, @secondUser";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@firstUser", firstUser),
                        new SqlParameter("@secondUser", secondUser)
                    };
            return await _context.Query<SPMessagesGetByUsers>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public int UpdateReceivedByUsers(int sender, int recipient, byte messageType)
        {
            var sqlQuery = "EXEC [dbo].[SP_Messages_UpdateReceivedByUsers] @sender, @recipient, @type";
            List<SqlParameter> pc = new List<SqlParameter>
                {
                    new SqlParameter("@sender", sender),
                    new SqlParameter("@recipient", recipient),
                    new SqlParameter("@type", messageType)
                };
            return _context.Database.ExecuteSqlCommand(sqlQuery, pc.ToArray());
        }

        public async Task<int> Create(int sender, int recipient, string text, byte messageType, int? messageFileId)
        {
            var sqlQuery = "EXEC [dbo].[SP_Messages_Create] @sender, @recipient, @text, @type, @messageFile";
            List<SqlParameter> pc = new List<SqlParameter>
            {
                new SqlParameter("@sender", sender),
                new SqlParameter("@recipient", recipient),
                new SqlParameter("@text", text ?? (object)DBNull.Value),
                new SqlParameter("@type", messageType),
                new SqlParameter("@messageFile", messageFileId ?? (object)DBNull.Value)
            };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc.ToArray());
        }

        public async Task<List<SPMessagesGetContactsByUser>> GetContactsByUser(int userId, byte messageType)
        {
            var sqlQuery = "EXEC [dbo].[SP_Messages_GetContactsByUser] @id, @type";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", userId),
                        new SqlParameter("@type", messageType)
                    };
            return await _context.Query<SPMessagesGetContactsByUser>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }
    }
}
