using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IVedomostReportService
    {
        Task<OperationDetails<FileDto>> GetReportAsync(int raspredelenieId);
    }
}
