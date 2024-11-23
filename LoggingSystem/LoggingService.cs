using Commons.HealthExpenseManagement.Helper.LoggingSystem.LoggingData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.ComponentModel;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Commons.HealthExpenseManagement.Helper.LoggingSystem
{
    public class LoggingService<T> : ILoggingService<T>
    {
        private IConfiguration _configuration;
        private String _path;
        private readonly ILogger<T> _logger;
        public LoggingService(ILogger<T> logger , IConfiguration configuration )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           
            _configuration = configuration;
            _path= _configuration["Loging_Path"]+ typeof(T).Name;
        }

        public LogDataFormat<T> Loginfo<T>(T data, string endpointName)
        {
            // Initialize variables to hold mac address and device type for the first available network interface
            string macAddress = string.Empty;
            string deviceName = string.Empty;
            deviceName = System.Net.Dns.GetHostName();
            // Iterate through all network interfaces, but only log the first one (or pick your preferred logic)
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            if (networkInterface != null)
            {
                macAddress = networkInterface.GetPhysicalAddress().ToString();
             
            }

            var loginInformation = new LoginDataFormat<T>
            {
                // Store only the first device's MAC address and device type
                MacAddress = macAddress,
                DeviceType = deviceName,
                success = true,
                message = "Successfully logged in!",
                Endpoint = endpointName,
                Data = data,
                Timestamp = DateTime.Now,
             code = CodeType.Login
            };

            string controllerName = typeof(T).Name;
            // Log the information (using Serilog or other logging system)
            LogFileHelper.Log(_path, controllerName, JsonSerializer.Serialize(loginInformation, new JsonSerializerOptions
            {
                WriteIndented = true
            }).ToString() + ",");


            return loginInformation;
        }
        public async Task<LogDataFormat<T>> LogCreationSuccess<T>(T data, string endpointName)
        {

            var createInformation = new LogDataFormat <T>
            {
                success = true,
                message = "successfully created!",
                Endpoint = endpointName,
                Data = data,
                Timestamp = DateTime.Now,
                code = CodeType.Create
            };

            /* Serilog.Log.Information(JsonSerializer.Serialize(createInformation, new JsonSerializerOptions
             {
                 WriteIndented = true
             }).ToString() + ",");*/
            string controllerName = typeof(T).Name;
            //var path = _configuration["Loging_Path"] + typeof(T).Name;
            LogFileHelper.Log(_path, controllerName, JsonSerializer.Serialize(createInformation, new JsonSerializerOptions
            {
                WriteIndented = true
            }).ToString() + ",");
          
            return createInformation;

        }
        public UpdateInformation<T> LogUpdateSuccess<T>(T data, string endpointName, T olddata)
        {

            var updateInformation = new UpdateInformation<T>
            {
                success = true,
                message = "successfully Update!",
                Endpoint = endpointName,
                Data = data,
                olddata = olddata,
                Timestamp = DateTime.Now,
                code = CodeType.Update
            };
            string controllerName = typeof(T).Name;
            LogFileHelper.Log(_path, controllerName, JsonSerializer.Serialize(updateInformation, new JsonSerializerOptions
            {
                WriteIndented = true
            }).ToString() + ",");

           


            return updateInformation;
        }
        public LogDataFormat<T> LogDeleteSuccess<T>(string endpointName, T olddata)
        {

               var deleteInformation = new LogDataFormat<T>
               {
                success = true,
                message = "successfully deleted!",
                Endpoint = endpointName,
                Data = olddata,
                Timestamp = DateTime.Now,
                code = CodeType.Delete
            };

            string controllerName = typeof(T).Name;
            LogFileHelper.Log(_path, controllerName, JsonSerializer.Serialize(deleteInformation, new JsonSerializerOptions
            {
                WriteIndented = true // Proper syntax for indented JSON

            }).ToString() + ",");

            return deleteInformation;
        }
        public LogDataFormat<T> LogGetSuccess<T>(string endpointName)
        {

            var createInformation = new LogDataFormat<T>
            {
                success = true,
                message = "successfully Get",
                Endpoint = endpointName,
                Timestamp = DateTime.Now,
                code = CodeType.Get
            };
            string controllerName = typeof(T).Name;

            LogFileHelper.Log(_path, controllerName, JsonSerializer.Serialize(createInformation, new JsonSerializerOptions
            {
                WriteIndented = true
            }).ToString() + ",");
            return createInformation;

        }
        
        public bool LogGeneralError<T>(Exception ex, T data, string endpointName)
        {
            var errorDetails = new ErrorDetails<T>
            {
                ErrorType = ex.GetType().Name,
                Message = ex.Message,
                Data = data,
                Endpoint = endpointName,
                Timestamp = DateTime.Now,
                userId = 1 // Replace with actual user ID if available
            };
            string controllerName = typeof(T).Name;
            LogFileHelper.Log(_path, controllerName, JsonSerializer.Serialize(errorDetails, new JsonSerializerOptions
            {
                WriteIndented = true
            }).ToString() + ",");

            return false;
        }

        public bool LogWarning<T>(WarningException warningException, string endpointName)
        {
            var errorDetails = new ErrorDetails<T>
            {
                ErrorType = warningException.GetType().Name,
                Message = warningException.Message,
                Endpoint = endpointName,
                Timestamp = DateTime.Now, // Use UTC for consistency
                userId = 1 // Replace with actual user ID if available
            };
            string controllerName = typeof(T).Name;
            // Serialize error details
            LogFileHelper.Log(_path, controllerName, JsonSerializer.Serialize(errorDetails, new JsonSerializerOptions
            {
                WriteIndented = true
            }).ToString() + ",");



            // Await any necessary asynchronous operations here if needed
            return false; // Placeholder for async operations
        }

    }


}