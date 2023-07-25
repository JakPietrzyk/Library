using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BooksLibrary.Middleware
{
    public class LoggerFilterAttribbute: ActionFilterAttribute
    {
        private readonly ILogger<LoggerFilterAttribbute> _logger;
        private Stopwatch _stopwatch;
        public LoggerFilterAttribbute(ILogger<LoggerFilterAttribbute> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch.Start();
            _logger.LogInformation($"Request started: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var response = context.HttpContext.Response.StatusCode;
            _logger.LogInformation($"Request finished: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Response: {response}, Duration: {_stopwatch.ElapsedMilliseconds} ms");

            if(response >= 400)
            {
                var errorMessage = context.Exception?.ToString() ?? "(No Additional error information)";
                _logger.LogError($"Request error: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Response: {response}, Error: {errorMessage}");
            }
        }
    }
}