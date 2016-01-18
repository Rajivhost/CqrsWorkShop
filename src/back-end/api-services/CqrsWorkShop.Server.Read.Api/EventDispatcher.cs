using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Data
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, Func<object, Task>> _routes;

        public EventDispatcher()
        {
            _routes =  new Dictionary<Type, Func<object, Task>>();
        }

        public void RegisterHandler<TEvent>(IHandle<TEvent> handler) where TEvent : class, IEvent
        {
            _routes.Add(typeof(TEvent), async @event => await handler.HandleAsync(@event as TEvent).ConfigureAwait(false));
        }

        public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var eventType = @event.GetType();

            if (!_routes.ContainsKey(eventType))
            {
                throw new ApplicationException("Missing handler for " + eventType.Name);
            }

            await _routes[eventType](@event).ConfigureAwait(false);
        }
    }
}