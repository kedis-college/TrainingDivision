using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.Core.SPModels.Raspredelenie;

namespace TrainingDivisionKedis.ViewModels.Raspredelenie
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Raspredelenie = new List<SPRaspredelenieOfYear>();
            Teachers = new List<SPFIOOfActivityOfTeachers>();
        }

        public List<SPRaspredelenieOfYear> Raspredelenie { get; set; }
        public List<SPFIOOfActivityOfTeachers> Teachers { get; set; }
    }
}
