using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Data
{
    public interface IEventDispatcher
    {
        void RegisterHandler<TEvent>(IHandle<TEvent> handler) where TEvent : class, IEvent;
        Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}