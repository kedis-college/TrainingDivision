using System.Collections.Generic;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.Student.ViewModels.Umk
{
    public class UmkIndexViewModel
    {
        public UmkIndexViewModel()
        {
            Years = new List<Year>();
            Terms = new List<Term>();
        }

        public List<Year> Years { get; set; }
        public List<Term> Terms { get; set; }
    }
}
