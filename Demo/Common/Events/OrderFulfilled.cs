﻿namespace Common
{
    public class OrderFulfilled : DomainEvent
    {
        public OrderFulfilled(string sourceId) : base(sourceId)
        {
        }
    }
}