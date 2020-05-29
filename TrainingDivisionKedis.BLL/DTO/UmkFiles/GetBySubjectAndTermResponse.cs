using System;
using System.Collections.Generic;
using System.Text;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.BLL.DTO.UmkFiles
{
    public class GetBySubjectAndTermResponse
    {
        public GetBySubjectAndTermResponse(List<UmkFile> umkFiles, string subjectName)
        {
            UmkFiles = umkFiles ?? throw new ArgumentNullException(nameof(umkFiles));
            SubjectName = subjectName ?? throw new ArgumentNullException(nameof(subjectName));
        }

        public List<UmkFile> UmkFiles { get; set; }
        public string SubjectName { get; set; }
    }
}
