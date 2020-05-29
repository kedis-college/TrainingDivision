using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.ControlSchedule;

namespace TrainingDivisionKedis.DAL.QueryDecorators
{
    public class ControlScheduleQueryDecorator : IControlScheduleQuery
    {
        private readonly DbContext _context;

        public ControlScheduleQueryDecorator(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<ControlSchedule>();
            modelBuilder.Query<SPControlScheduleGetEditable>();
        }

        public async Task<ControlSchedule> Create(byte yearId, byte seasonId, short userId, 
            DateTime dateStart, DateTime? dateEnd, DateTime? mod1DateStart, DateTime? mod1DateEnd, 
            DateTime? mod2DateStart, DateTime? mod2DateEnd, DateTime? itogDateStart, DateTime? itogDateEnd)
        {
            var sqlQuery = "EXEC [dbo].[SP_ControlSchedules_Create] @yearId, @seasonId, @dateStart, @dateEnd, @userId, " +
                        "@mod1DateStart, @mod1DateEnd, @mod2DateStart, @mod2DateEnd, @itogDateStart, @itogDateEnd";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@yearId", yearId),
                        new SqlParameter("@seasonId", seasonId),
                        new SqlParameter("@dateStart", dateStart),
                        new SqlParameter("@dateEnd", dateEnd ?? (object)DBNull.Value),
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@mod1DateStart", mod1DateStart ?? (object)DBNull.Value),
                        new SqlParameter("@mod1DateEnd", mod1DateEnd ?? (object)DBNull.Value),
                        new SqlParameter("@mod2DateStart", mod2DateStart ?? (object)DBNull.Value),
                        new SqlParameter("@mod2DateEnd", mod2DateEnd ?? (object)DBNull.Value),
                        new SqlParameter("@itogDateStart", itogDateStart ?? (object)DBNull.Value),
                        new SqlParameter("@itogDateEnd", itogDateEnd ?? (object)DBNull.Value)
                    };
            return await _context.Query<ControlSchedule>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }

        public async Task<List<ControlSchedule>> GetByYearAndSeason(byte yearId, byte seasonId)
        {
            var sqlQuery = "EXEC [dbo].[SP_ControlSchedules_GetByYearAndSeason] @yearId, @seasonId";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@yearId", yearId),
                        new SqlParameter("@seasonId", seasonId)
                    };
            return await _context.Query<ControlSchedule>().FromSql(sqlQuery, pc.ToArray()).ToListAsync();
        }

        public async Task<ControlSchedule> GetById(int id)
        {
            var sqlQuery = "EXEC [dbo].[SP_ControlSchedules_GetById] @id";
            var sqlParameter = new SqlParameter("@id", id);
            return await _context.Query<ControlSchedule>().FromSql(sqlQuery, sqlParameter).FirstOrDefaultAsync();
        }

        public async Task<ControlSchedule> Update(int id, short userId,
            DateTime dateStart, DateTime? dateEnd, DateTime? mod1DateStart, DateTime? mod1DateEnd,
            DateTime? mod2DateStart, DateTime? mod2DateEnd, DateTime? itogDateStart, DateTime? itogDateEnd)
        {
            var sqlQuery = "EXEC [dbo].[SP_ControlSchedules_Update] @id, @dateStart, @dateEnd, @userId, " +
                        "@mod1DateStart, @mod1DateEnd, @mod2DateStart, @mod2DateEnd, @itogDateStart, @itogDateEnd";
            List<SqlParameter> pc = new List<SqlParameter>
                    {
                        new SqlParameter("@id", id),
                        new SqlParameter("@dateStart", dateStart),
                        new SqlParameter("@dateEnd", dateEnd ?? (object)DBNull.Value),
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@mod1DateStart", mod1DateStart ?? (object)DBNull.Value),
                        new SqlParameter("@mod1DateEnd", mod1DateEnd ?? (object)DBNull.Value),
                        new SqlParameter("@mod2DateStart", mod2DateStart ?? (object)DBNull.Value),
                        new SqlParameter("@mod2DateEnd", mod2DateEnd ?? (object)DBNull.Value),
                        new SqlParameter("@itogDateStart", itogDateStart ?? (object)DBNull.Value),
                        new SqlParameter("@itogDateEnd", itogDateEnd ?? (object)DBNull.Value),
                    };
            return await _context.Query<ControlSchedule>().FromSql(sqlQuery, pc.ToArray()).FirstOrDefaultAsync();
        }

        public async Task<SPControlScheduleGetEditable> GetEditable(int raspredelenieId)
        {
            var sqlQuery = "EXEC [dbo].[SP_ControlSchedule_GetEditable] @raspredelenieId";
            var sqlParameter = new SqlParameter("@raspredelenieId", raspredelenieId);
            return await _context.Query<SPControlScheduleGetEditable>().FromSql(sqlQuery, sqlParameter).FirstOrDefaultAsync();
        }
    }
}
