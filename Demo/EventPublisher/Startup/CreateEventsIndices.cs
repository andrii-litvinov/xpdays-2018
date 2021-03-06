﻿using System.Threading.Tasks;
using Framework;
using MongoDB.Driver;

namespace EventPublisher
{
    public class CreateEventsIndices : IStartupOperation
    {
        private readonly IMongoCollection<EventEnvelope> events;
        private readonly IndexKeysDefinitionBuilder<EventEnvelope> indexKeys = Builders<EventEnvelope>.IndexKeys;
        private readonly CreateIndexOptions options = new CreateIndexOptions {Background = true};

        public CreateEventsIndices(IMongoDatabase database) => events = database.GetCollection<EventEnvelope>("events");

        public async Task Execute() =>
            await events.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<EventEnvelope>(indexKeys.Ascending(e => e.Timestamp), options),
                new CreateIndexModel<EventEnvelope>(indexKeys.Ascending("event._t").Ascending(e => e.Timestamp),
                    options),
                new CreateIndexModel<EventEnvelope>(indexKeys.Ascending($"event.{PrivateField.SourceId}"), options)
            });
    }
}