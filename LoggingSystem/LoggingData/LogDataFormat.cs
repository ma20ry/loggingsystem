using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Commons.HealthExpenseManagement.Helper.LoggingSystem.LoggingData
{ 
    public  class LogDataFormat <T>
    {
        public bool success { get; set; } = true;
        public string message { get; set; }
        public T Data { get; set; }
        public string Endpoint { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int userId { get; set; } = 1;
        public CodeType code { get; set; }
    }
}
