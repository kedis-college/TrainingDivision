using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingDivisionKedis.Core.Models;

namespace TrainingDivisionKedis.ViewModels.Tests
{
    public class TestIndexViewModel
    {
        public TestIndexViewModel()
        {
            Years = new List<Year>();
            Terms = new List<Term>();
        }

        public List<Year> Years { get; set; }
        public List<Term> Terms { get; set; }
    }
}
