using System;
using System.Collections.Generic;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Domain.Aggrates
{
    public class AggregateBase : IAggregate
    {
        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        private readonly Dictionary<Type, Action<IEvent>> _routes = new Dictionary<Type, Action<IEvent>>();

        public AggregateBase()
        {
            Version = -1;
        }

        public int Version { get; protected set; }

        public Guid Id { get; protected set; }

        public void RaiseEvent(IEvent @event)
        {
            ApplyEvent(@event);
            _uncommitedEvents.Add(@event);
        }

        protected void RegisterTransition<T>(Action<T> transition) where T : class
        {
            _routes.Add(typeof(T), o => transition(o as T));
        }

        public void ApplyEvent(IEvent @event)
        {
            var eventType = @event.GetType();
            if (_routes.ContainsKey(eventType))
            {
                _routes[eventType](@event);
            }
            Version++;
        }

        public IEnumerable<IEvent> UncommitedEvents()
        {
            return _uncommitedEvents;
        }

        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        }
    }
}