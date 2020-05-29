using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.Curriculum;
using TrainingDivisionKedis.Core.SPModels.Curriculum;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface ICurriculumService
    {
        Task<OperationDetails<List<SPSubjectsGetByStudent>>> GetSubjectsOfStudentAsync(GetSubjectsOfStudentRequest request);
    }
}
