using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;

namespace TrainingDivisionKedis.BLL.DTO.ProgressInStudy
{
    public class ProgressInStudyOfStudent
    {
        public ProgressInStudyOfStudent(byte term, List<SPProgressInStudyGetByStudent> progressInStudy)
        {
            Term = term;
            ProgressInStudy = progressInStudy ?? throw new ArgumentNullException(nameof(progressInStudy));
        }

        public byte Term { get; set; }
        public List<SPProgressInStudyGetByStudent> ProgressInStudy { get; set; }
    }
}
