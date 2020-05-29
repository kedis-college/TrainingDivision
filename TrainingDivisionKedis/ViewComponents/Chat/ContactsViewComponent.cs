using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.Core.SPModels.Students;

namespace TrainingDivisionKedis.ViewComponents.Chat
{
    public class ContactsViewComponent : ViewComponent
    {
        private readonly IChatService<SPStudentsGetWithSpeciality> _chatService;

        public ContactsViewComponent(IChatService<SPStudentsGetWithSpeciality> chatService)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var response = await _chatService.GetMessageContactsByUserAsync(id);
            if (response.Succedeed)
                return View(response.Entity);
            else
                return Content(response.Error.Message);
        }
    }
}
