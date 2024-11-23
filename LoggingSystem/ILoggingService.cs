using Commons.HealthExpenseManagement.Helper.LoggingSystem.LoggingData;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel;


namespace Commons.HealthExpenseManagement.Helper.LoggingSystem
{
    public interface ILoggingService<T>
    {
        LogDataFormat<T> Loginfo<T>(T data, string endpointName);
     Task < LogDataFormat<T> >LogCreationSuccess<T>(T genriclogger, string endpointName);
       UpdateInformation<T> LogUpdateSuccess<T>(T data, string endpointName, T olddata);
        LogDataFormat<T> LogDeleteSuccess<T>(string endpointName, T olddata);
        LogDataFormat<T> LogGetSuccess<T>(string endpointName);
        bool LogGeneralError<T>(Exception ex, T genriclogger, string endpointName);
        bool LogWarning<T>(WarningException warningException, string endpointName);

    }
}