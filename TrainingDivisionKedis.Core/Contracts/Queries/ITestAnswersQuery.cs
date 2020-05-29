using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Core.Contracts.Queries
{
    public interface ITestAnswersQuery
    {
        Task<TestAnswer> Create(string text, int questionId, bool isCorrect);
    }
}
