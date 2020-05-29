using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;

namespace TrainingDivisionKedis.ViewComponents.Tests
{
    public class StudentResultsViewComponent : ViewComponent
    {
        private readonly ITestAdminService _testService;

        public StudentResultsViewComponent(ITestAdminService testService)
        {
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int subjectId, int studentId, byte termId)
        {
            var response = await _testService.GetResultsBySubjectAndTermAndStudentAsync(new GetBySubjectAndTermRequest {SubjectId = subjectId, TermId = termId, UserId = studentId });
            if (response.Succedeed)
            {
                return View(response.Entity);
            }
            else
            {
                return Content(response.Error.Message);
            }
        }
    }
}
