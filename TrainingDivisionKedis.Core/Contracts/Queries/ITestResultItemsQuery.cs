using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface ITestResultItemsQuery
    {
        Task<TestResultItem> Create(int testResultId, int questionId, int? answerId);
    }
}
