using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IYearService
    {
        Task<OperationDetails<List<Year>>> GetAllAsync();
    }
}
