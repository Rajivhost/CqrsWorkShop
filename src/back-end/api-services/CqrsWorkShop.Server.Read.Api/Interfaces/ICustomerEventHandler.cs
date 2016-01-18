using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Data.EventHandlers
{
    public interface ICustomerEventHandler : IHandle<CustomerCreated>, IHandle<CustomerMarkedAsPreferred>
    {
    }
}