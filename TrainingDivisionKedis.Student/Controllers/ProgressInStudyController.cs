using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.Student.Extensions.Alerts;
using TrainingDivisionKedis.Student.ViewModels.ProgressInStudy;

namespace TrainingDivisionKedis.Student.Controllers
{
    [Authorize]
    public class ProgressInStudyController : Controller
    {
        private readonly IProgressInStudyService _progressInStudyService;

        public ProgressInStudyController(IProgressInStudyService progressInStudyService)
        {
            _progressInStudyService = progressInStudyService ?? throw new ArgumentNullException(nameof(progressInStudyService));
        }

        public async Task<IActionResult> Index()
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();

            var prInStudy = await _progressInStudyService.GetByStudentAsync(id);
            var totals = await _progressInStudyService.GetTotalsOfStudentAsync(id);
            var viewModel = new IndexViewModel();
            if (prInStudy.Succedeed)
            {
                if (totals.Succedeed)
                {
                    viewModel.ProgressInStudy = prInStudy.Entity;
                    viewModel.Totals = totals.Entity;
                    return View(viewModel);
                }
                else
                    return View(viewModel).WithDanger("Ошибка!", totals.Error.Message);
            }
            else
                return View(viewModel).WithDanger("Ошибка!", prInStudy.Error.Message);
        }
    }
}