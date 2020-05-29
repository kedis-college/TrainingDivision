using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Chat;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.Student.Extensions.Alerts;

namespace TrainingDivisionKedis.Student.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService<SPTeacherGetAll> _chatService;

        public ChatController(IChatService<SPTeacherGetAll> chatService)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var response = await _chatService.GetRecipientsAsync();
            if (response.Succedeed)
            {
                if (response.Entity.Count > 0)
                    return View(response.Entity);
                return RedirectToAction("Index").WithWarning("", "Список получателей пуст!");
            }
            else
                return View(new List<SPTeacherGetAll>()).WithDanger("Ошибка!", response.Error.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MessageCreateRequest request)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();
            request.Sender = id;
            var response = await _chatService.CreateAsync(request);
            if (response.Succedeed)
                return RedirectToAction("Index").WithSuccess("", "Сообщение успешно отправлено");
            else
                return RedirectToAction("Create").WithDanger("Ошибка!", response.Error.Message);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAjax(MessageCreateRequest request)
        {
            var parseResult = int.TryParse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int id);
            if (!parseResult)
                return BadRequest();
            request.Sender = id;
            var response = await _chatService.CreateAsync(request);
            if (!response.Succedeed)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            return Json(response);
        }

        public IActionResult GetMessagesByUsers(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                return ViewComponent("Messages", new { firstUser = id, secondUser = userId });
            }
            catch
            {
                return PartialView("_MessagePartial", "Ошибка в авторизации");
            }
        }

        public IActionResult GetContacts()
        {
            try
            {
                var id = int.Parse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                return ViewComponent("Contacts", new { id });
            }
            catch
            {
                return PartialView("_MessagePartial", "Ошибка в авторизации");
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            var response = await _chatService.GetFileByMessageAsync(id);
            if (response.Succedeed)
            {
                return File(response.Entity.FileBytes, response.Entity.FileType, response.Entity.FileName);
            }
            else
                return NotFound();
        }
    }
}