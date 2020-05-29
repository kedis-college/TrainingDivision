using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using System.Linq;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.BLL.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{

    public class RaspredelenieService : IRaspredelenieService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public RaspredelenieService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }       

        public async Task<OperationDetails<List<SPSubjectsGetByYearAndTermAndUser>>> GetSubjectsByUserAndTermAndYearAsync(GetVedomostListRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var raspredelenie = await context.RaspredelenieQuery().GetSubjectsByYearAndTermAndUser(request.YearId, request.TermId, request.UserId);
                    return OperationDetails<List<SPSubjectsGetByYearAndTermAndUser>>.Success(raspredelenie);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPSubjectsGetByYearAndTermAndUser>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<SPRaspredelenieGetByYearAndTermAndUser>>> GetVedomostListAsync(GetVedomostListRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var raspredelenie = await context.RaspredelenieQuery().GetByYearAndTermAndUser(request.YearId, request.TermId, request.UserId);
                    return OperationDetails<List<SPRaspredelenieGetByYearAndTermAndUser>>.Success(raspredelenie);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPRaspredelenieGetByYearAndTermAndUser>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<SPRaspredelenieOfYear>>> GetByYearAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var raspredelenie = await context.RaspredelenieQuery().GetByYear(id);
                    return OperationDetails<List<SPRaspredelenieOfYear>>.Success(raspredelenie);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPRaspredelenieOfYear>>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }
}
