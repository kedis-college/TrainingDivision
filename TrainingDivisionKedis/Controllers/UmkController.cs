using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.ViewModels.Umk;
using TrainingDivisionKedis.Models.Vedomost;

namespace TrainingDivisionKedis.Controllers
{
    [Authorize]
    public class UmkController : Controller
    {
        private readonly IYearService _yearService;
        private readonly ITermService _termService;
        private readonly IRaspredelenieService _raspredelenieService;
        private readonly IUmkFileService _umkFileService;

        //private IHostingEnvironment _env;       

        public UmkController(IYearService yearService, ITermService termService, IRaspredelenieService raspredelenieService, IUmkFileService umkFileService)
        {
            _yearService = yearService ?? throw new ArgumentNullException(nameof(yearService));
            _termService = termService ?? throw new ArgumentNullException(nameof(termService));
            _raspredelenieService = raspredelenieService ?? throw new ArgumentNullException(nameof(raspredelenieService));
            _umkFileService = umkFileService ?? throw new ArgumentNullException(nameof(umkFileService));
        }

        public async Task<IActionResult> Index()
        {
            var indexViewModel = new UmkIndexViewModel();
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
            if (response.Succedeed)
                return PartialView("_SubjectsSearch", response.Entity);
            else
                return PartialView("_MessagePartial", response.Error.Message);
        }

        public async Task<IActionResult> Details(int subjectId, byte termId )
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();

            var request = new GetBySubjectAndTermRequest(subjectId, termId) { UserId = id };
            var response = await _umkFileService.GetBySubjectAndTermAsync(request);
            var vm = new UmkDetailsViewModel(subjectId, termId);
            if (response.Succedeed)
            {
                vm.UmkFiles = response.Entity.UmkFiles;
                vm.SubjectName = response.Entity.SubjectName;
                return View(vm);
            }
            else
                return View(vm).WithDanger("Ошибка!", response.Error.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Details(CreateRequest request)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();

            request.UserId = id;
            var response = await _umkFileService.CreateAsync(request);
            if (response.Succedeed)
                return RedirectToAction("Details",new { subjectId = request.SubjectId, termId = request.TermId }).WithSuccess("", "Файл успешно добавлен");
            else
                return RedirectToAction("Details", new { subjectId = request.SubjectId, termId = request.TermId }).WithDanger("Ошибка!", response.Error.Message);
        }

        public IActionResult Download(int id)
        {
            var response = _umkFileService.GetFileById(id);
            if (response.Succedeed)
            {
                return File(response.Entity.FileBytes, response.Entity.FileType, response.Entity.FileName);
            }
            else
                return NotFound();
        }

        public IActionResult Edit(int id)
        {
            var response = _umkFileService.GetById(id);
            if (response.Succedeed)
            {
                return View(response.Entity);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateRequest request)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();

            request.UserId = id;
            var response = await _umkFileService.UpdateAsync(request);
            if (response.Succedeed)
            {
                return RedirectToAction("Details", new { subjectId = response.Entity.SubjectId, termId = response.Entity.TermId }).WithSuccess("","Файл успешно изменен");
            }
            else
                return RedirectToAction("Edit", new { id = request.Id }).WithDanger("Ошибка!", response.Error.Message);
        }

        [ActionName("Delete")]
        public IActionResult ConfirmDelete(int id)
        {
            var response = _umkFileService.GetById(id);
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
            var responseUmk = _umkFileService.GetById(id);
            if (!responseUmk.Succedeed)
                return NotFound();

            var response = await _umkFileService.DeleteAsync(id);
            if (response.Succedeed)
            {
                return RedirectToAction("Details", new { subjectId = responseUmk.Entity?.SubjectId, termId = responseUmk.Entity?.TermId }).WithSuccess("", "Файл успешно удален");
            }
            else
                return RedirectToAction("Details", new { subjectId = responseUmk.Entity?.SubjectId, termId = responseUmk.Entity?.TermId }).WithDanger("Ошибка!", response.Error.Message);
        }
    }
}