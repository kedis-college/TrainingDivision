using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Contracts;

namespace TrainingDivisionKedis.ViewComponents.Vedomost
{
    public class PrizesTotalViewComponent : ViewComponent
    {
        private readonly IProgressInStudyService _progressInStudyService;

        public PrizesTotalViewComponent(IProgressInStudyService progressInStudyService)
        {
            _progressInStudyService = progressInStudyService ?? throw new ArgumentNullException(nameof(progressInStudyService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var response = await _progressInStudyService.GetTotalsByRaspredelenieAsync(Id);
            if (response.Succedeed)
                return View(response.Entity);
            else
                return Content(response.Error.Message);
        }
    }
}
