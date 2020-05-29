using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Student.ViewModels.Umk
{
    public class UmkDetailsViewModel
    {
        public UmkDetailsViewModel()
        {
            UmkFiles = new List<UmkFile>();
        }

        public UmkDetailsViewModel(int subjectId, byte termId)
        {
            SubjectId = subjectId;
            SubjectName = "";
            TermId = termId;
            UmkFiles = new List<UmkFile>();
        }

        public List<UmkFile> UmkFiles { get; set; }
        public int SubjectId { get; set; }
        public byte TermId { get; set; }
        public string SubjectName { get; set; }
    }
}
