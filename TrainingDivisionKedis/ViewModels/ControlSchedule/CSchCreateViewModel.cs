using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingDivisionKedis.BLL.DTO.ControlSchedule;

namespace TrainingDivisionKedis.ViewModels.ControlSchedule
{
    public class CSchCreateViewModel
    {
        public SelectList Years { get; set; }
        public SelectList TermSeasons { get; set; }
        public ControlScheduleDto ControlSchedule { get; set; }
    }
}
