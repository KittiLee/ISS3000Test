﻿using System;
using System.IO;
using AirlineSendAgent.App;
using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AirlineSendAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets(typeof(Program).Assembly, optional:true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            var config = builder.Build();
            
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>{
                  services.AddSingleton<IAppHost, AppHost>();
                  services.AddSingleton<IWebhookClient, WebhookClient>();
                  services.AddDbContext<SendAgentDbContext>(opt => opt.UseSqlServer ("Server=localhost,1433;Database=AirlineWebDB;User Id=sa;Password=pa55w0rd!"));
                  //(config.GetConnectionString("AirlineConnection")));
                                                                
                  
                    
                  services.AddHttpClient();
                }).Build();

            host.Services.GetService<IAppHost>().Run();
        }
    }
}
