using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IActivityOfTeacherService
    {
        Task<OperationDetails<List<SPFIOOfActivityOfTeachers>>> GetTeachersWithPostAsync();
    }
}
