using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HistoryRental.Loggers
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
            if(context.HttpContext.Items["X-Request-ID"] is null)
            {
                List<string> listToAdd = new List<string>();
                context.HttpContext.Items.Add("X-Request-ID", listToAdd);
            }
            if(context.HttpContext.Request.Headers.TryGetValue("X-Request-ID", out var requestId))
            {
                var listToUpdate = (List<string>?)context.HttpContext.Items["X-Request-ID"];
                listToUpdate.Add(requestId);
                _logger.LogInformation("Request {status}: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Request ID: {requestId}","started",context.HttpContext.Request.Method,context.HttpContext.Request.Path,requestId.ToString() );
            }
            else
            {
                string generetedRequestId = Guid.NewGuid().ToString();
                var listToUpdate = (List<string>?)context.HttpContext.Items["X-Request-ID"];
                listToUpdate.Add(generetedRequestId);
                _logger.LogInformation("Request {status}: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Request ID: {requestId}","started",context.HttpContext.Request.Method,context.HttpContext.Request.Path,generetedRequestId.ToString());
            }
           
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var response = context.HttpContext.Response.StatusCode;

            if(context.HttpContext.Items["X-Request-ID"] is null)
            {
                List<string> listToAdd = new List<string>();
                context.HttpContext.Items.Add("X-Request-ID", listToAdd);
            }
            if(context.HttpContext.Request.Headers.TryGetValue("X-Request-ID", out var requestId))
            {
                _logger.LogInformation("Request {status}: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Request ID: {requestId}, Response: {response}, Duration: {_stopwatch.ElapsedMilliseconds} ms","finished",context.HttpContext.Request.Method,context.HttpContext.Request.Path,requestId.ToString(),response,_stopwatch.ElapsedMilliseconds );
            }
            else
            {
                
                var listToUpdate = (List<string>?)context.HttpContext.Items["X-Request-ID"];
                string generetedRequestId = listToUpdate.Last();
                _logger.LogInformation("Request {status}: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Request ID: {requestId}, Response: {response}, Duration: {_stopwatch.ElapsedMilliseconds} ms","finished",context.HttpContext.Request.Method,context.HttpContext.Request.Path,generetedRequestId.ToString(),response,_stopwatch.ElapsedMilliseconds );
            }
            

            if(response >= 400)
            {
                var errorMessage = context.Exception?.ToString() ?? "(No Additional error information)";
                _logger.LogError($"Request error: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Response: {response}, Error: {errorMessage}");
            }
        }
    }
}