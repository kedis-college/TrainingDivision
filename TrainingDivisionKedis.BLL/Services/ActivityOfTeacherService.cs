using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class ActivityOfTeacherService : IActivityOfTeacherService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public ActivityOfTeacherService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<OperationDetails<List<SPFIOOfActivityOfTeachers>>> GetTeachersWithPostAsync()
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var teachers = await context.TeachersQuery().GetActivityAll();
                    return OperationDetails<List<SPFIOOfActivityOfTeachers>>.Success(teachers);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPFIOOfActivityOfTeachers>>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }
}
