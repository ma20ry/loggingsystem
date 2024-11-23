using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

public class LogFileHelper
{
   

    public static void Log(string basePath, string controllerName, string message)
    {
       

        List<string> arrayPaths= basePath.Split("\\").ToList();
      
        string fileName=  arrayPaths.Last();

        arrayPaths.Remove(arrayPaths.Last());

         fileName = arrayPaths.Last()+"\\"+ fileName;
        
        arrayPaths.Remove(arrayPaths.Last());

        string joinedPath = string.Join("\\", arrayPaths);
       
        // Get the current year and month
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;

        // Construct the log directory path with the controller name, year, and month
        string logDirectory = Path.Combine(joinedPath, year.ToString(), month.ToString("D2"), fileName);
        
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        
        // Construct the log file path
        string logFilePath = logDirectory+"log.txt";

        // Format the log entry with a timestamp
        
        string logEntry = $" {message}{Environment.NewLine}";

        // Append the log entry to the file
       System.IO. File.AppendAllText(logFilePath, logEntry);
    }
}