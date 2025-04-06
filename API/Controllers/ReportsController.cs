using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ReportApiProject.Controllers
{
    [Authorize(Roles = "prothetic_user")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        [HttpGet]        
        public IActionResult GetReport()
        {
            // Генерация произвольного отчета
            var reportBytes = GenerateReport();

            // Возврат файла        
            return File(reportBytes, "application/json", "report.json");
        }

        public record JsonReport(int Year, int Month, int Day, string Title);

        private byte[] GenerateReport()
        {
            var today = DateTime.Today;

            JsonReport r = new(today.Year, today.Month, today.Day, "Test report");
            string jsonString = JsonSerializer.Serialize(r);

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            return byteArray;
        }
    }
}
