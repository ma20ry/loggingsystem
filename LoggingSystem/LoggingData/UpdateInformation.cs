using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.HealthExpenseManagement.Helper.LoggingSystem.LoggingData
{
    public class UpdateInformation<T> :LogDataFormat <T>
    {
        public T olddata { get; set; }
        
    }
}
