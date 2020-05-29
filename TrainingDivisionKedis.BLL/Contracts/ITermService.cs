using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface ITermService
    {
        Task<OperationDetails<List<Term>>> GetAllAsync();
        Task<OperationDetails<List<TermSeason>>> GetSeasonsAllAsync();
    }
}
