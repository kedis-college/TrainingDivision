using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingDivisionKedis.Core.SPModels.ControlSchedule;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;

namespace TrainingDivisionKedis.BLL.DTO.ProgressInStudy
{
    public class ProgressInStudyListDto
    {   
        public ProgressInStudyListDto()
        {
        }

        public ProgressInStudyListDto(IEnumerable<SPProgressInStudyGetByRaspredelenieAndUser> collection, SPControlScheduleGetEditable controlSchedule)
        {
            ProgressInStudy = collection.ToList();
            ControlSchedule = controlSchedule;
            if (controlSchedule != null)
            {
                ControlSchedule.Mod1 = !ControlSchedule.Mod1;
                ControlSchedule.Mod2 = !ControlSchedule.Mod2;
                ControlSchedule.Itog = !ControlSchedule.Itog;
            }
        }

        public ProgressInStudyListDto(IEnumerable<SPProgressInStudyGetByRaspredelenieAndUser> collection)
        {
            ProgressInStudy = collection.ToList();
        }

        public List<SPProgressInStudyGetByRaspredelenieAndUser> ProgressInStudy { get; set; }
        public SPControlScheduleGetEditable ControlSchedule { get; set; }
    }
}
