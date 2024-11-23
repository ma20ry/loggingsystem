using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Commons.HealthExpenseManagement.Helper.LoggingSystem.LoggingData
{
    public class LoginDataFormat<T> : LogDataFormat<T>
    {

        public string MacAddress { get; set; }
        public string DeviceType { get; set; }
       
        
    }
    
}
