namespace cw5.Services
{
    public interface ILoggingService
    {
        void LogToFile(string path, string methodName, string queryString, string bodyString);
    }
}