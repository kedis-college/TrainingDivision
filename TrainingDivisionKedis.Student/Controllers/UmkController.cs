using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Curriculum;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Student.Extensions.Alerts;
using TrainingDivisionKedis.Student.Models;
using TrainingDivisionKedis.Student.ViewModels.Umk;

namespace TrainingDivisionKedis.Student.Controllers
{
    [Authorize]
    public class UmkController : Controller
    {
        private readonly ITermService _termService;
        private readonly ICurriculumService _curriculumService;
        private readonly IUmkFileService _umkFileService;
        private readonly IOptions<UmkFilesConfiguration> _umkFilesConfiguration;

        public UmkController(ITermService termService, ICurriculumService curriculumService, IUmkFileService umkFileService, IOptions<UmkFilesConfiguration> umkFilesConfiguration)
        {
            _termService = termService ?? throw new ArgumentNullException(nameof(termService));
            _curriculumService = curriculumService ?? throw new ArgumentNullException(nameof(curriculumService));
            _umkFileService = umkFileService ?? throw new ArgumentNullException(nameof(umkFileService));
            _umkFilesConfiguration = umkFilesConfiguration ?? throw new ArgumentNullException(nameof(umkFilesConfiguration));
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
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value, out int userId);
            if (!parseResult)
                return BadRequest();

            var request = new GetSubjectsOfStudentRequest(termId, userId);
            var response = await _curriculumService.GetSubjectsOfStudentAsync(request);
            if (response.Succedeed)
                return PartialView("_SubjectsSearch", response.Entity);
            else
                return PartialView("_MessagePartial", response.Error.Message);
        }

        public async Task<IActionResult> Details(int subjectId, byte termId)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value, out int userId);
            if (!parseResult)
                return BadRequest();

            var request = new GetBySubjectAndTermRequest(subjectId, termId) { UserId = userId };
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
    }
}