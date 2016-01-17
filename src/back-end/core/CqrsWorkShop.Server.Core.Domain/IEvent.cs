using System;

namespace Hse.CqrsWorkShop.Domain
{
    public interface IEvent
    {
        Guid Id { get; }
    }
}
