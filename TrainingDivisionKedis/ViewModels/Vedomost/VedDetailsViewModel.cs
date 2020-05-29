using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;

namespace TrainingDivisionKedis.ViewModels.Vedomost
{
    public class VedDetailsViewModel
    {
        public VedDetailsViewModel()
        {
            ProgressInStudy = new ProgressInStudyListDto();
            Date = DateTime.Today;
        }

        public VedDetailsViewModel(ProgressInStudyListDto progressInStudy, int raspredelenieId)
        {
            ProgressInStudy = progressInStudy ?? throw new ArgumentNullException(nameof(progressInStudy));
            SpecialityName = ProgressInStudy.ProgressInStudy.FirstOrDefault().SpecialityName;
            GroupName = ProgressInStudy.ProgressInStudy.FirstOrDefault().GroupName;
            SubjectName = ProgressInStudy.ProgressInStudy.FirstOrDefault().SubjectName;
            Date = DateTime.Today;
            RaspredelenieId = raspredelenieId;
        }

        public ProgressInStudyListDto ProgressInStudy { get; set; }
        public string SpecialityName { get; set; }
        public string GroupName { get; set; }
        public string SubjectName { get; set; }
        public DateTime Date { get; set; }
        public int RaspredelenieId { get; set; }
    }
}
