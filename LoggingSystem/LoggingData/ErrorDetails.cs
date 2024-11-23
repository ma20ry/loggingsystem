using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.HealthExpenseManagement.Helper.LoggingSystem.LoggingData
{
    public class ErrorDetails <T>
    {
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string Endpoint { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int userId { get; set; } = 1;
    }
}
