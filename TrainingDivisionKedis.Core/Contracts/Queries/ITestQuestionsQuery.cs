using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Core.SPModels.Tests;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface ITestQuestionsQuery
    {
        Task<TestQuestion> Create(string text, int testId);
        Task<List<SPTestQuestionsGetRandomSet>> GetRandomSet(int testId);
        Task<List<SPTestQuestionsGetRandomSet>> GetByTestResultId(int testResultId);
    }
}
