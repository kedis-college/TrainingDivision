using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.Tests
{
    class ComparableObject 
    {
        public static string Convert(object objectToCompare)
        {
            return JsonConvert.SerializeObject(objectToCompare);
        }
    }
}
