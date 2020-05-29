using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Curriculum;
using TrainingDivisionKedis.BLL.DTO.TestsClient;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Student.Extensions.Alerts;
using TrainingDivisionKedis.Student.ViewModels.Tests;
using TrainingDivisionKedis.Student.ViewModels.Umk;

namespace TrainingDivisionKedis.Student.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITermService _termService;
        private readonly ICurriculumService _curriculumService;
        private readonly ITestClientService _testService;

        public TestsController(ITermService termService, ICurriculumService curriculumService, ITestClientService testService)
        {
            _termService = termService ?? throw new ArgumentNullException(nameof(termService));
            _curriculumService = curriculumService ?? throw new ArgumentNullException(nameof(curriculumService));
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _termService.GetAllAsync();
            if (result.Succedeed)
            {
                return View(result.Entity);
            }
            else
            {
                return View(result.Entity).WithDanger("Ошибка!", result.Error.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubjectsSearch([FromBody] byte termId)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            if (!parseResult)
                return BadRequest();

            var request = new GetSubjectsOfStudentRequest(termId, userId);
            var response = await _curriculumService.GetSubjectsOfStudentAsync(request);
            if (response.Succedeed)
                return PartialView("_SubjectsSearch", response.Entity);
            else
                return PartialView("_MessagePartial", response.Error.Message);
        }

        public async Task<IActionResult> List(int subjectId, byte termId)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value, out int userId);
            if (!parseResult)
                return BadRequest();

            var request = new GetBySubjectAndTermRequest() { SubjectId = subjectId, UserId = userId, TermId = termId };
            var response = await _testService.GetBySubjectAndTermAsync(request);
            var vm = new TestsListViewModel(subjectId);
            if (response.Succedeed)
            {
                vm.Tests = response.Entity.Tests;
                vm.SubjectName = response.Entity.SubjectName;
                return View(vm);
            }
            else
                return View(vm).WithDanger("Ошибка!", response.Error.Message);
        }

        public async Task<IActionResult> Details(int id)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value, out int userId);
            if (!parseResult)
                return BadRequest();

            var response = await _testService.GetByIdAsync(id,userId);
            if (response.Succedeed)
                return View(response.Entity);
            else
                return NotFound();
        }

        public async Task<IActionResult> Start(int id)
        {
            var dateNow = DateTime.Now;
            var response = await _testService.Start(new TestStartRequest(id, dateNow));
            if (!response.Succedeed)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResultItem([FromBody]TestResultItem testResultItem)
        {
            var response = await _testService.CreateResultItem(testResultItem);
            if (!response.Succedeed)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Finish(int TestResultId)
        {
            var response = await _testService.Finish(TestResultId);
            if (response.Succedeed && response.Entity.Test != null)
                return RedirectToAction("List", new { response.Entity.Test.SubjectId, response.Entity.Test.TermId })
                    .WithSuccess("", "Тест пройден с результатом " + response.Entity.Ball + "  из " + response.Entity.Test.QuestionsPerTest);
            else
                return BadRequest();

        }
    }
}