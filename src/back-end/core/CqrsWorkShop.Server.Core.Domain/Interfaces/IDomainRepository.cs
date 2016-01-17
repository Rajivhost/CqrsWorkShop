using System;
using System.Collections.Generic;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Domain
{
    public interface IDomainRepository
    {
        IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : IAggregate, new();
    }
}