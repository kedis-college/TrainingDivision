using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.BLL.DTO.TestsAdmin;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.Models.Tests;
using TrainingDivisionKedis.Models.Vedomost;
using TrainingDivisionKedis.ViewModels.Tests;

namespace TrainingDivisionKedis.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly IYearService _yearService;
        private readonly ITermService _termService;
        private readonly IRaspredelenieService _raspredelenieService;
        private readonly ITestAdminService _testService;
        private readonly ICurriculumService _curriculumService;

        public TestsController(IYearService yearService, ITermService termService, IRaspredelenieService raspredelenieService, ITestAdminService testService, ICurriculumService curriculumService)
        {
            _yearService = yearService ?? throw new ArgumentNullException(nameof(yearService));
            _termService = termService ?? throw new ArgumentNullException(nameof(termService));
            _raspredelenieService = raspredelenieService ?? throw new ArgumentNullException(nameof(raspredelenieService));
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
            _curriculumService = curriculumService ?? throw new ArgumentNullException(nameof(curriculumService));
        }

        public async Task<IActionResult> Index()
        {
            var indexViewModel = new TestIndexViewModel();
            var yearsResult = await _yearService.GetAllAsync();            
            var termsResult = await _termService.GetAllAsync();
            if (yearsResult.Succedeed && termsResult.Succedeed)
            {
                indexViewModel.Years = yearsResult.Entity;
                indexViewModel.Terms = termsResult.Entity;
                return View(indexViewModel);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SubjectsSearch([FromBody] GetVedomostListRequest searchParams)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if(!parseResult)
                return PartialView("_MessagePartial", "Ошибка в запросе");

            searchParams.UserId = id;
            var response = await _raspredelenieService.GetSubjectsByUserAndTermAndYearAsync(searchParams);
            if(response.Succedeed)
                return PartialView("_SubjectsSearch", response.Entity);
            else
                return PartialView("_MessagePartial", response.Error.Message);           
        }

        public async Task<IActionResult> List(int subjectId, byte termId)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value, out int userId);
            if (!parseResult)
                return BadRequest();

            var request = new GetBySubjectAndTermRequest(subjectId, termId) { UserId = userId };
            var response = await _testService.GetBySubjectAndTermAsync(request);
            var vm = new TestListViewModel(subjectId, termId);
            if (response.Succedeed)
            {
                vm.Tests = response.Entity.Tests;
                vm.SubjectName = response.Entity.SubjectName;
                return View(vm);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> List(UpdateStateModel model)
        {
            model.Visible = model.Visible != null ? !model.Visible : null;
            var response = await _testService.UpdateVisibilityAsync(new TestStateDto {Id = model.Id, Visible=model.Visible, Draft=model.Draft, Active=model.Active });
            if(response.Succedeed)
                return RedirectToAction("List", new { subjectId = model.SubjectId, termId = model.TermId } );
            else
                return RedirectToAction("List", new { subjectId = model.SubjectId, termId = model.TermId }).WithDanger("Ошибка!",response.Error.Message) ;
        }

        public async Task<IActionResult> Create(int subjectId, byte termId)
        {
            var subjectResult = await _curriculumService.GetSubjectNameById(subjectId);
            if (!subjectResult.Succedeed)
                return NotFound();            
            var model = new TestCreateDto {SubjectId = subjectId, TermId = termId, SubjectName = subjectResult.Entity };
            model.Questions.Add(new TestQuestionDto());
            model.Questions.Add(new TestQuestionDto());
            model.Questions.Add(new TestQuestionDto());
            model.Questions[0].Answers.Add(new TestAnswerDto());
            model.Questions[0].Answers.Add(new TestAnswerDto());
            model.Questions[0].Answers.Add(new TestAnswerDto());
            model.Questions[0].Answers.Add(new TestAnswerDto());
            model.Questions[1].Answers.Add(new TestAnswerDto());
            model.Questions[1].Answers.Add(new TestAnswerDto());
            model.Questions[1].Answers.Add(new TestAnswerDto());
            model.Questions[1].Answers.Add(new TestAnswerDto());
            model.Questions[2].Answers.Add(new TestAnswerDto());
            model.Questions[2].Answers.Add(new TestAnswerDto());
            model.Questions[2].Answers.Add(new TestAnswerDto());
            model.Questions[2].Answers.Add(new TestAnswerDto());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestCreateDto test, int subjectId, byte termId)
        {
            if (ModelState.IsValid)
            {
                if (test.QuestionsPerTest > test.Questions.Count)
                {
                    ModelState.AddModelError("QuestionsPerTest", "Количество вопросов в тесте должно быть меньше или равно общему количеству вопросов");
                    test.QuestionsTotal = (short)test.Questions.Count;
                }
                else
                {
                    test.UserId = int.Parse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var response = await _testService.CreateAsync(test);
                    if (response.Succedeed)
                        return RedirectToAction("List", new { subjectId = test.SubjectId, termId = test.TermId }).WithSuccess("", "Тест успешно добавлен");
                    return View(test).WithDanger("Ошибка!", response.Error.Message);
                }
            }
            return View(test);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _testService.GetByIdAsync(id);
            if (response.Succedeed)
                return View(response.Entity);
            else
                return View(response.Entity).WithDanger("Ошибка!",response.Error.Message);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _testService.GetByIdAsync(id);
            if (response.Succedeed)
            {
                if (!response.Entity.Draft)
                    return NotFound();
                var subjectResult = await _curriculumService.GetSubjectNameById(response.Entity.SubjectId);
                if (!subjectResult.Succedeed)
                    return BadRequest();
                ViewBag.SubjectName = subjectResult.Entity;
                return View(response.Entity);
            }
            else
                return View(response.Entity).WithDanger("Ошибка!", response.Error.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Test test)
        {
            if (ModelState.IsValid)
            {
                var response = await _testService.UpdateAsync(test);
                if (response.Succedeed)
                    return RedirectToAction("List", new { subjectId = test.SubjectId, termId = test.TermId }).WithSuccess("", "Тест успешно изменен");
                return View(test).WithDanger("Ошибка!", response.Error.Message);
            }
            else
                return View(test);
        }

        public async Task<IActionResult> Results(int id)
        {
            var response = await _testService.GetResultsByTestIdAsync(id);
            if (response.Succedeed)
                return View(response.Entity);
            else
                return View(response.Entity).WithDanger("Ошибка!", response.Error.Message);
        }

        public async Task<IActionResult> ResultDetails(int id)
        {
            var response = await _testService.GetResultByIdAsync(id);
            if (response.Succedeed)
                return View(response.Entity);
            else
                return View(response.Entity).WithDanger("Ошибка!", response.Error.Message);
        }

        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var response = await _testService.GetByIdAsync(id);
            if (response.Succedeed)
            {
                return View(response.Entity);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseTest = await _testService.GetByIdAsync(id);
            var response = await _testService.DeleteAsync(id);
            if (response.Succedeed)
            {
                return RedirectToAction("List", new { subjectId = responseTest.Entity.SubjectId, termId = responseTest.Entity.TermId }).WithSuccess("", "Тест успешно удален");
            }
            else
                return RedirectToAction("List", new { subjectId = responseTest.Entity.SubjectId, termId = responseTest.Entity.TermId }).WithDanger("Ошибка!", response.Error.Message);
        }

        public IActionResult ResultsBySubjectAndStudent(int subjectId, int studentId, byte termId)
        {
            return ViewComponent("StudentResults", new { subjectId, studentId, termId });
        }
    }
}
