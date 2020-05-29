using System.Collections.Generic;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.ViewModels.ControlSchedule
{
    public class CSchIndexViewModel
    {
        public List<Year> Years { get; set; }
        public List<TermSeason> TermSeasons { get; set; }
    }
}
