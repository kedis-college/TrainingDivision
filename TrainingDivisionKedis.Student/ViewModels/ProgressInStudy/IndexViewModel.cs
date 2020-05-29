using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;

namespace TrainingDivisionKedis.Student.ViewModels.ProgressInStudy
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            ProgressInStudy = new List<ProgressInStudyOfStudent>();
        }

        public List<ProgressInStudyOfStudent> ProgressInStudy { get; set; }
        public SPProgressInStudyGetTotalsByStudent Totals { get; set; }
    }
}
