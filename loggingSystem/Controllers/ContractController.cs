using Commons.HealthExpenseManagement.Data.Entities.Contracts;
using Commons.HealthExpenseManagement.Helper.LoggingSystem.LoggingData;
using Commons.HealthExpenseManagement.savefile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Serilog;
using System.Text.Json;

namespace loggingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly ISaveFile saveFile;

        public ContractController(ISaveFile saveFile)
        {
            this.saveFile = saveFile;
        }
        [HttpGet("get-file/")]
        public async Task<ActionResult> GetFile([FromQuery] string filterMessage = null,
                                    [FromQuery] bool? filterSuccess = null,
                                    [FromQuery] DateTime? filterTimestamp = null,
                                    [FromQuery] string filterEndpoint = null,
                                    [FromQuery] string filterData = null)
        {
            // Set log directory path
            // Assuming you're reading from "logfile.txt"

            // Assign current year and month
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            string filePath = Path.Combine(@"C:\Users\HP\Documents\Growth-tec\ContractDocument\logs",
                                     currentYear.ToString(),
                                     currentMonth.ToString(),
                                     "Contract",
                                       $"Controller-{currentYear}{currentMonth}.txt");
            try
            {
                // Check if the log file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }
                var lines = await System.IO.File.ReadAllLinesAsync(filePath);
                string data = " ";
                List<LogDataFormat<Contract>> logEntries = new List<LogDataFormat<Contract>>();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        // حاول تحليل السطر إلى كائن JSON
                        data += line;

                    }
                    catch (JsonException ex)
                    {
                        // تسجيل الخطأ الذي حدث أثناء التحليل
                        Serilog.Log.Error(ex, "Failed to deserialize line: {Line}", line);
                        continue;
                    }
                }
                data = "[" + data;

                data = data.Remove(data.Count() - 1);
                data = data + "]";

                List<LogDataFormat<Contract>> result = JsonSerializer.Deserialize<List<LogDataFormat<Contract>>>(data);

                var filteredEntries = result.AsQueryable();


                // Apply filtering based on query parameters
                if (!string.IsNullOrEmpty(filterMessage))
                {
                    filteredEntries = filteredEntries.Where(entry => entry.message.Contains(filterMessage, StringComparison.OrdinalIgnoreCase));
                }
                if (filterSuccess.HasValue)
                {
                    filteredEntries = filteredEntries.Where(entry => entry.success == filterSuccess.Value);
                }

                // Filter by current year and month if no timestamp filter is provided
                if (filterTimestamp.HasValue)
                {
                    filteredEntries = filteredEntries.Where(entry => entry.Timestamp.Date == filterTimestamp.Value.Date);
                }
                else
                {
                    // If filterTimestamp is not provided, filter logs by the current year and month
                    filteredEntries = filteredEntries.Where(entry => entry.Timestamp.Year == currentYear && entry.Timestamp.Month == currentMonth);
                }

                // Filter by Endpoint
                if (!string.IsNullOrEmpty(filterEndpoint))
                {
                    filteredEntries = filteredEntries.Where(entry => entry.Endpoint.Equals(filterEndpoint, StringComparison.OrdinalIgnoreCase));
                }

                if (!filteredEntries.Any())
                {
                    return NotFound("No matching log entries found.");
                }

                // Return filtered entries as JSON
                return Ok(filteredEntries.ToList());
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"An error occurred while reading the file: {ex.Message}");
            }
        }
    }
}
