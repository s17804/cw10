using System;
using System.IO;

namespace cw5.Services.impl
{
    public class LoggingService : ILoggingService
    {
        private const string LoggerPath = "requestsLog.txt";
        private const string LoggerDateTimeFormat = "dd.MM.yyyy:hh.mm.ss";

        public void LogToFile(string path, string methodName, string queryString, string bodyString)
        {
            using var streamWriter = new StreamWriter(LoggerPath, true);
            streamWriter.WriteLine("-----------------------------------------------------------------------------");
            streamWriter.WriteLine("Date: " + DateTime.Now.ToString(LoggerDateTimeFormat));
            streamWriter.WriteLine("Path: " + path);
            streamWriter.WriteLine("MethodName: " + methodName);
            streamWriter.WriteLine("QueryString: " + queryString);
            streamWriter.WriteLine("BodyString: " + bodyString);
            streamWriter.WriteLine();
        }
    }
}