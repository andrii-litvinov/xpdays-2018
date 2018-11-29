﻿using System;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Payments
{
    public class OrderPlacedHandler : IEventHandler<OrderPlaced>
    {
        private readonly IMongoCollection<Payment> payments;
        public OrderPlacedHandler(IMongoDatabase database) => payments = database.GetCollection<Payment>("payments");

        public async Task Handle(OrderPlaced @event)
        {
            var payment = new Payment(ObjectId.GenerateNewId().ToString(), @event.SourceId, @event.TotalAmount);

            if (new Random().Next() % 2 == 0) payment.Accept();
            else payment.Reject();

            await payments.InsertOneAsync(payment);
        }
    }
}