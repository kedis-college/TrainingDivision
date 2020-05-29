using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrainingDivisionKedis.ViewModels.Vedomost;
using TrainingDivisionKedis.Models.Vedomost;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Raspredelenie;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using TrainingDivisionKedis.Extensions.Alerts;
using System.Security.Claims;

namespace TrainingDivisionKedis.Controllers
{
    [Authorize]
    public class VedomostController : Controller
    {
        private readonly IYearService _yearService;
        private readonly ITermService _termService;
        private readonly IRaspredelenieService _raspredelenieService;
        private readonly IProgressInStudyService _progressInStudyService;
        private readonly IVedomostReportService _vedomostReportService;

        public VedomostController(IYearService yearService, ITermService termService, IRaspredelenieService raspredelenieService, IProgressInStudyService progressInStudyService, IVedomostReportService vedomostReportService)
        {
            _yearService = yearService ?? throw new ArgumentNullException(nameof(yearService));
            _termService = termService ?? throw new ArgumentNullException(nameof(termService));
            _raspredelenieService = raspredelenieService ?? throw new ArgumentNullException(nameof(raspredelenieService));
            _progressInStudyService = progressInStudyService ?? throw new ArgumentNullException(nameof(progressInStudyService));
            _vedomostReportService = vedomostReportService ?? throw new ArgumentNullException(nameof(vedomostReportService));
        }

        public async Task<IActionResult> Index()
        {            
            var yearsResult = await _yearService.GetAllAsync();
            var termsResult = await _termService.GetAllAsync();
            if (yearsResult.Succedeed && termsResult.Succedeed)
            {
                var indexViewModel = new VedIndexViewModel();
                indexViewModel.Years = yearsResult.Entity;
                indexViewModel.Terms = termsResult.Entity;
                return View(indexViewModel);
            }
            else
                return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> VedomostSearch([FromBody] GetVedomostListRequest searchParams)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return PartialView("_MessagePartial", "Ошибка в авторизации");

            searchParams.UserId = id;
            var response = await _raspredelenieService.GetVedomostListAsync(searchParams);
            if(response.Succedeed)
                return PartialView("_VedomostSearch", response.Entity);
            else
                return PartialView("_MessagePartial", response.Error.Message);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            if (!parseResult)
                return BadRequest();

            var response = await _progressInStudyService.GetByRaspredelenieAndUserAsync(new ProgressInStudyRequest(id, userId));
            if (response.Succedeed && response.Entity.ProgressInStudy?.Count > 0)
            {
                var model = new VedDetailsViewModel(response.Entity, id);
                return View(model);
            }
            else
                return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] ProgressInStudyUpdateRequest request)
        {
            var parseResult = short.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out short userId);
            if (!parseResult)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Ошибка в авторизации").WithDanger("Ошибка!", "Ошибка в авторизации");
            }

            request.UserId = userId;
            var response = await _progressInStudyService.UpdateAsync(request);
            if (response.Succedeed)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(response);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(response).WithDanger("Ошибка!", response.Error.Message);
            }
        }

        public IActionResult GetPrizesTotal(int id)
        {
            return ViewComponent("PrizesTotal", id);
        }

        public async Task<IActionResult> GetReport(int id)
        {
            var response = await _vedomostReportService.GetReportAsync(id);
            if (response.Succedeed)
                return File(response.Entity.FileBytes, response.Entity.FileType, response.Entity.FileName);
            else
                return BadRequest();
        }
    }
}