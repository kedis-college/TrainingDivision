using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingDivisionKedis.ViewModels.Account
{
    public class ChangeLoginViewModel
    {
        public int Id { get; set; }
        public string OldLogin { get; set; }
        public string NewLogin { get; set; }
    }
}
