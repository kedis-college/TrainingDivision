using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.DAL;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.BLL.Contracts;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.Services
{ 
    public class TermService : ITermService
    {

        private readonly IAppDbContextFactory _contextFactory;

        public TermService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<OperationDetails<List<Term>>> GetAllAsync()
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var terms = await context.Terms.ToListAsync();
                    return OperationDetails<List<Term>>.Success(terms);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<Term>>.Failure(ex.Message, "");
                }
            }
        }

        public async Task<OperationDetails<List<TermSeason>>> GetSeasonsAllAsync()
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var terms = await context.TermSeasons.ToListAsync();
                    return OperationDetails<List<TermSeason>>.Success(terms);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<TermSeason>>.Failure(ex.Message, "");
                }
            }
        }
    }
}
