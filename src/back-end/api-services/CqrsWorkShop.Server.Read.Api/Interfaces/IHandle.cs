using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Data
{
    public interface IHandle<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}