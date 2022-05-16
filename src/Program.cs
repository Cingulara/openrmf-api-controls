// Copyright (c) Cingulara LLC 2020 and Tutela LLC 2020. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace openrmf_api_controls
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOGLEVEL"))) // default
                NLog.LogManager.Configuration.Variables["logLevel"] = "Warn";
            else {
                switch (Environment.GetEnvironmentVariable("LOGLEVEL"))
                {
                    case "5":
                        NLog.LogManager.Configuration.Variables["logLevel"] = "Critical";
                        break;
                    case "4":
                        NLog.LogManager.Configuration.Variables["logLevel"] = "Error";
                        break;
                    case "3":
                        NLog.LogManager.Configuration.Variables["logLevel"] = "Warn";
                        break;
                    case "2":
                        NLog.LogManager.Configuration.Variables["logLevel"] = "Info";
                        break;
                    case "1":
                        NLog.LogManager.Configuration.Variables["logLevel"] = "Debug";
                        break;
                    case "0":
                        NLog.LogManager.Configuration.Variables["logLevel"] = "Trace";
                        break;
                    default:
                        NLog.LogManager.Configuration.Variables["logLevel"] = "Warn";
                        break;
                }
            }
            NLog.LogManager.ReconfigExistingLoggers();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run(); 
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception with logging. Please investigate.");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                        // make the timeout 6 minutes for longer running processes
                        serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(6);
                    })                
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    })
                    .UseNLog()  // NLog: setup NLog for Dependency injection
                    .UseStartup<Startup>();
                });
    }
}
