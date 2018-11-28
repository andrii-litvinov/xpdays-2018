﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Serilog;
using SimpleInjector;

namespace Payments
{
    public static class Bootstrapper
    {
        public static Container ConfigureContainer(IConfiguration configuration, ILogger logger)
        {
            BsonConfig.RegisterConventionPacks();

            var container = new Container();

            container.RegisterInstance(configuration);
            container.RegisterInstance(logger);

            var mongoUrl = configuration["mongo:url"];
            var url = new MongoUrl(mongoUrl);
            var client = new MongoClient(url);
            var database = client.GetDatabase(url.DatabaseName);
            container.RegisterInstance(database);

            container.Register(typeof(IEventHandler<>), typeof(Bootstrapper).Assembly);
            container.RegisterDecorator(typeof(IEventHandler<>), typeof(LoggerEventHandlerDecorator<>));

            container.Collection.Append(
                typeof(IHostedService),
                Lifestyle.Singleton.CreateRegistration(() =>
                {
                    var consumer = new EventConsumer("payments", database, new Dictionary<string, Func<DomainEvent, Task>>
                    {
                        {nameof(OrderCreated), @event => container.GetInstance<IEventHandler<OrderCreated>>().Handle((OrderCreated) @event)}
                    }, logger);
                    return consumer;
                }, container));

            return container;
        }
    }
}