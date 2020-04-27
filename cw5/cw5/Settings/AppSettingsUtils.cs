﻿using System;
using System.Configuration;

namespace cw5.Settings
{
    public static class AppSettingsUtils
    {
        public static string GetConnectionString()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return $"server={appSettings["Server"]};" +
                       $"database={appSettings["Database"]};" +
                       $"User={appSettings["User"]};" +
                       $"Password={appSettings["Password"]}";
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

            return string.Empty;
        }
    }
}