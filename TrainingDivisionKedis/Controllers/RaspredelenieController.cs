using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.ViewModels.Raspredelenie;

namespace TrainingDivisionKedis.Controllers
{
    [Authorize]
    public class RaspredelenieController : Controller
    {
        private readonly IYearService _yearService;
        private readonly IRaspredelenieService _raspredelenieService;
        private readonly IActivityOfTeacherService _activityOfTeacherService;

        public RaspredelenieController(IYearService yearService, IRaspredelenieService raspredelenieService, IActivityOfTeacherService activityOfTeacherService)
        {
            _yearService = yearService ?? throw new ArgumentNullException(nameof(yearService));
            _raspredelenieService = raspredelenieService ?? throw new ArgumentNullException(nameof(raspredelenieService));
            _activityOfTeacherService = activityOfTeacherService ?? throw new ArgumentNullException(nameof(activityOfTeacherService));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _yearService.GetAllAsync();
            if (result.Succedeed)
                return View(result.Entity);
            else
                return View(result.Entity).WithDanger("Ошибка!", result.Error.Message);
        }

        public async Task<IActionResult> RaspredelenieSearch(int id)
        {
            var raspredelenie = await _raspredelenieService.GetByYearAsync(id);
            var teachers = await _activityOfTeacherService.GetTeachersWithPostAsync();
            var viewModel = new IndexViewModel();
            viewModel.Raspredelenie = raspredelenie.Entity;
            viewModel.Teachers = teachers.Entity;
            return PartialView("_RaspredelenieSearch", viewModel);
        }
    }
}