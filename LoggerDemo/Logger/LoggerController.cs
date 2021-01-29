using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace LoggerDemo.Logger
{
    public class LoggerController
    {
        private readonly ILogger<LoggerController> logger;

        public LoggerController(ILogger<LoggerController> logger)
        {
            this.logger = logger;
        }

        [FunctionName("Demo")]
        public async Task<IActionResult> PostConsent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logger")]
            HttpRequest request)
        {
            logger.LogInformation($"Demo logger Information: {DateTime.Now}");
            logger.LogError($"Demo logger Error: {DateTime.Now}");
            logger.LogCritical($"Demo logger Critical: {DateTime.Now}");
            return new OkObjectResult("Logging succesfull");
        }
    }
}