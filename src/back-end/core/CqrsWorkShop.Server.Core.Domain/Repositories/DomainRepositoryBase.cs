using System;
using System.Collections.Generic;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Domain.Repositories
{
    public abstract class DomainRepositoryBase : IDomainRepository
    {
        public abstract IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        public abstract TAggregate GetById<TAggregate>(Guid id) where TAggregate : IAggregate, new();

        protected int CalculateExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            var expectedVersion = aggregate.Version - events.Count;
            return expectedVersion;
        }

        protected TAggregate BuildAggregate<TAggregate>(IEnumerable<IEvent> events) where TAggregate : IAggregate, new()
        {
            var result = new TAggregate();

            foreach (var @event in events)
            {
                result.ApplyEvent(@event);
            }
            return result;
        }
    }
}