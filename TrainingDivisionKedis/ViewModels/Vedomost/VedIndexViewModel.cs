using System.Collections.Generic;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.ViewModels.Vedomost
{
    public class VedIndexViewModel
    {
        public VedIndexViewModel()
        {
            Years = new List<Year>();
            Terms = new List<Term>();
        }

        public List<Year> Years { get; set; }
        public List<Term> Terms { get; set; }
    }
}
