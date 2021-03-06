﻿using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace Framework
{
    public static class LoggerFactory
    {
        public static Logger Create(IConfiguration configuration)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Async(c => c.LiterateConsole())
                .Enrich.WithDemystifiedStackTraces()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("OsVersion", Environment.OSVersion)
                .Enrich.WithProperty("Version", assemblyName.Version)
                .Enrich.WithProperty("UserName", Environment.UserName)
                .Enrich.WithProperty("Application", $"{assemblyName.Name}")
                .Enrich.WithProperty("Environment", configuration["environment"])
                .Enrich.With(new TraceContextEnricher());

            return loggerConfiguration.CreateLogger();
        }
    }
}