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
    public class UmkFilesQueryDecorator : IUmkFilesQuery
    {
        private readonly AppDbContext _context;

        public UmkFilesQueryDecorator(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UmkFile> Create(string name, int subjectId, byte termId, string fileName, double fileSize, string fileType)
        {
            var sqlQuery = "EXEC [dbo].[SP_UmkFiles_Create] @subjectId, @termId, " +
                        "@name, @fileName, @fileSize, @fileType";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@subjectId", subjectId),
                        new SqlParameter("@termId", termId),
                        new SqlParameter("@name", name),
                        new SqlParameter("@fileName", fileName),
                        new SqlParameter("@fileSize", fileSize),
                        new SqlParameter("@fileType", fileType)
                    };

            return await _context.UmkFiles.FromSql(sqlQuery, pc.ToArray()).FirstAsync();
        }

        public async Task<int> Delete(int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_UmkFiles_Update] @id, @name, @fileName, @fileSize, @fileType, @active";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", id),
                        new SqlParameter("@name", DBNull.Value),
                        new SqlParameter("@fileName", DBNull.Value),
                        new SqlParameter("@fileSize", DBNull.Value),
                        new SqlParameter("@fileType", DBNull.Value),
                        new SqlParameter("@active", false)
                    };
            return await _context.Database.ExecuteSqlCommandAsync(sqlQuery, pc.ToArray());
        }

        public async Task<UmkFile> Update(int id, string name, string fileName, double? fileSize, string fileType)
        {
            var sqlQuery = "EXEC [dbo].[SP_UmkFiles_Update] @id, @name, @fileName, @fileSize, @fileType, @active";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", id),
                        new SqlParameter("@name", name ?? (object)DBNull.Value),
                        new SqlParameter("@fileName", fileName ?? (object)DBNull.Value),
                        new SqlParameter("@fileSize", fileSize ?? (object)DBNull.Value),
                        new SqlParameter("@fileType", fileType ?? (object)DBNull.Value),
                        new SqlParameter("@active", DBNull.Value)
                    };
            return await _context.UmkFiles.FromSql(sqlQuery, pc.ToArray()).FirstAsync();
        }
    }
}
