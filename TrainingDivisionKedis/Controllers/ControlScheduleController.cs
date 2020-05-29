using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.ControlSchedule;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.ViewModels.ControlSchedule;

namespace TrainingDivisionKedis.Controllers
{
    [Authorize(Roles = "директор, начальник учебного управления")]
    public class ControlScheduleController : Controller
    {
        private readonly IYearService _yearService;
        private readonly ITermService _termService;
        private readonly IControlScheduleService _controlScheduleService;

        public ControlScheduleController(IYearService yearService, ITermService termService, IControlScheduleService controlScheduleService)
        {
            _yearService = yearService ?? throw new ArgumentNullException(nameof(yearService));
            _termService = termService ?? throw new ArgumentNullException(nameof(termService));
            _controlScheduleService = controlScheduleService ?? throw new ArgumentNullException(nameof(controlScheduleService));
        }

        public async Task<IActionResult> Index()
        {
            var yearsResponse = await _yearService.GetAllAsync();
            var termsResponse = await _termService.GetSeasonsAllAsync();
            var viewModel = new CSchIndexViewModel();
            if (yearsResponse.Succedeed && termsResponse.Succedeed)
            {
                viewModel.Years = yearsResponse.Entity;
                viewModel.TermSeasons = termsResponse.Entity;
                return View(viewModel);
            }
            else
                return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ControlScheduleSearch([FromBody] GetByYearAndSeasonRequest searchParams)
        {
            var response = await _controlScheduleService.GetByYearAndSeasonAsync(searchParams);
            if (response.Succedeed)
                return PartialView("_ControlScheduleSearch", response.Entity);
            else
                return PartialView("_MessagePartial", response.Error.Message);
        }

        public async Task<IActionResult> Create()
        {
            var yearsResponse = await _yearService.GetAllAsync();
            var termsResponse = await _termService.GetSeasonsAllAsync();
            var viewModel = new CSchCreateViewModel();
            if (yearsResponse.Succedeed && termsResponse.Succedeed)
            {
                viewModel.Years = new SelectList(yearsResponse.Entity, "Id", "Name");
                viewModel.TermSeasons = new SelectList(termsResponse.Entity, "Id", "Name");
                return View(viewModel);
            }
            else
                return RedirectToAction("Error", "Home");     
        }

        [HttpPost]
        public async Task<IActionResult> Create(CSchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var yearsResponse = await _yearService.GetAllAsync();
                var termsResponse = await _termService.GetSeasonsAllAsync();
                if (yearsResponse.Succedeed && termsResponse.Succedeed)
                {
                    model.Years = new SelectList(yearsResponse.Entity, "Id", "Name");
                    model.TermSeasons = new SelectList(termsResponse.Entity, "Id", "Name");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }               
            }
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();

            model.ControlSchedule.UserId = (short)id;
            var response = await _controlScheduleService.CreateAsync(model.ControlSchedule);
            if (response.Succedeed)
                return RedirectToAction("Index").WithSuccess("", "Запись успешно добавлена");
            else
                return View(model).WithDanger("Ошибка!", response.Error.Message);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _controlScheduleService.GetByIdAsync(id);
            if (response.Succedeed)
                return View(response.Entity);
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ControlScheduleDto dto)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();

            dto.UserId = (short)id;
            var response = await _controlScheduleService.UpdateAsync(dto);
            if (response.Succedeed)
            {
                dto = response.Entity;
                return View(dto).WithSuccess("", "Запись успешно обновлена");
            }
            else
                return RedirectToAction("Edit",new { id = dto.Id }).WithDanger("Ошибка!",response.Error.Message);
        }
    }
}