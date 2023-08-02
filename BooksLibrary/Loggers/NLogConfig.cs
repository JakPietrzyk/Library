using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace BooksLibrary.Loggers
{
    public static class NLogJsonConfiguration
    {
        public static void Configure()
        {
            var config = new LoggingConfiguration();
            var jsonFileTarget = new FileTarget
            {
                Name = "jsonFile",
                FileName = "C:/Users/Jakub/Documents/C#/1 task/BooksLibrary/Logs/${shortdate}_log-file.json",
            };
            var jsonLayout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("time", "${longdate}"),
                    new JsonAttribute("X-Request-ID", "${event-properties:requestId}"),
                    new JsonAttribute("requestMethod", "${event-properties:context.HttpContext.Request.Method}"),
                    new JsonAttribute("requestPath", "${event-properties:context.HttpContext.Request.Path}"),
                    new JsonAttribute("status", "${event-properties:status}"),
                    new JsonAttribute("responseTime", "${event-properties:response}"),
                    new JsonAttribute("level", "${level:upperCase=true}"),
                    new JsonAttribute("message", "${message}"),
                }
            };
            var rule = new LoggingRule("BooksLibrary.*", NLog.LogLevel.Info, jsonFileTarget);
            config.LoggingRules.Add(rule);
            jsonFileTarget.Layout = jsonLayout;

            LogManager.Configuration = config;

            var logger = LogManager.GetCurrentClassLogger();


        }
    }
}