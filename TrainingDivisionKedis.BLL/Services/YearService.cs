using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.BLL.Contracts;
using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.Services
{
    public class YearService : IYearService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public YearService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<OperationDetails<List<Year>>> GetAllAsync()
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var years = await context.Years.ToListAsync();
                    return OperationDetails<List<Year>>.Success(years);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<Year>>.Failure(ex.Message, "");
                }
            }
        }
    }
}
