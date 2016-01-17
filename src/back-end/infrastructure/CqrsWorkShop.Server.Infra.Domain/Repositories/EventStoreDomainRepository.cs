using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Hse.CqrsWorkShop.Domain.Events;
using Hse.CqrsWorkShop.Domain.Exceptions;
using Newtonsoft.Json;

namespace Hse.CqrsWorkShop.Domain.Repositories
{
    public class EventStoreDomainRepository : DomainRepositoryBase
    {
        private readonly IEventStoreConnection _connection;
        private const string Category = "cqrsworkshop";

        public EventStoreDomainRepository(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        private string AggregateToStreamName(Type type, Guid id)
        {
            return string.Format("{0}-{1}-{2}", Category, type.Name, id);
        }

        public override async Task<IEnumerable<IEvent>> SaveAsync<TAggregate>(TAggregate aggregate)
        {
            var events = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, events);
            var eventData = events.Select(CreateEventData);
            var streamName = AggregateToStreamName(aggregate.GetType(), aggregate.Id);
            await _connection.AppendToStreamAsync(streamName, expectedVersion, eventData).ConfigureAwait(false);
            return events;
        }

        public override async Task<TResult> GetByIdAsync<TResult>(Guid id)
        {
            var streamName = AggregateToStreamName(typeof(TResult), id);
            var eventsSlice = await _connection.ReadStreamEventsForwardAsync(streamName, 0, int.MaxValue, false).ConfigureAwait(false);
            if (eventsSlice.Status == SliceReadStatus.StreamNotFound)
            {
                throw new AggregateNotFoundException("Could not found aggregate of type " + typeof(TResult) + " and id " + id);
            }
            var deserializedEvents = eventsSlice.Events.Select(e =>
            {
                var metadata = DeserializeObject<Dictionary<string, string>>(e.OriginalEvent.Metadata);
                var eventData = DeserializeObject(e.OriginalEvent.Data, metadata[EventClrTypeHeader]);
                return eventData as IEvent;
            });
            return BuildAggregate<TResult>(deserializedEvents);
        }

        private T DeserializeObject<T>(byte[] data)
        {
            return (T)(DeserializeObject(data, typeof(T).AssemblyQualifiedName));
        }

        private object DeserializeObject(byte[] data, string typeName)
        {
            var jsonString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName));
        }

        public EventData CreateEventData(object @event)
        {
            var eventHeaders = new Dictionary<string, string>()
            {
                {
                    EventClrTypeHeader, @event.GetType().AssemblyQualifiedName
                },
                {
                    "Domain", "Enheter"
                }
            };
            var eventDataHeaders = SerializeObject(eventHeaders);
            var data = SerializeObject(@event);
            var eventData = new EventData(Guid.NewGuid(), @event.GetType().Name, true, data, eventDataHeaders);
            return eventData;
        }

        private byte[] SerializeObject(object obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(jsonObj);
            return data;
        }

        public string EventClrTypeHeader = "EventClrTypeName";
    }
}