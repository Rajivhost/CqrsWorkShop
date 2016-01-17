using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Domain
{
    public interface IDomainRepository
    {
        Task<IEnumerable<IEvent>> SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : IAggregate, new();
    }
}