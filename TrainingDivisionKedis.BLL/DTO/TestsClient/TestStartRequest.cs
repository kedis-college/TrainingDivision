using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.DTO.TestsClient
{
    public class TestStartRequest
    {
        public TestStartRequest(int testResultId, DateTime startedAt)
        {
            TestResultId = testResultId;
            StartedAt = startedAt;
        }

        public int TestResultId { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
