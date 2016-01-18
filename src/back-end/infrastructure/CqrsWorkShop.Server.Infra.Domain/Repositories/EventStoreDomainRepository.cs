using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Hse.CqrsWorkShop.Domain.Events;
using Hse.CqrsWorkShop.Domain.Exceptions;
using Newtonsoft.Json;
// ReSharper disable ExceptionNotDocumented

namespace Hse.CqrsWorkShop.Domain.Repositories
{
    public class EventStoreDomainRepository : DomainRepositoryBase
    {
        private const int PageSize = 512;
        private IEventStoreConnection _connection;
        private readonly IGuidIdProvider _guidIdProvider;
        private const string Category = "HseShop";

        public string EventClrTypeHeader = "EventClrTypeName";

        public EventStoreDomainRepository(IEventStoreConnectionProvider eventStoreConnectionProvider, IGuidIdProvider guidIdProvider)
        {
            InitializeEventStoreConnection(eventStoreConnectionProvider);
            _guidIdProvider = guidIdProvider;
        }

        private async void InitializeEventStoreConnection(IEventStoreConnectionProvider eventStoreConnectionProvider)
        {
            _connection = eventStoreConnectionProvider.GetConnection();

            await _connection.ConnectAsync().ConfigureAwait(false);
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

        public override async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id)
        {
            var streamName = AggregateToStreamName(typeof(TAggregate), id);

            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.Start;

            var streamEvents = new List<ResolvedEvent>();

            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, PageSize, false).ConfigureAwait(false);
                if (currentSlice.Status == SliceReadStatus.StreamDeleted || currentSlice.Status == SliceReadStatus.StreamNotFound)
                {
                    var message = string.Format("Could not found aggregate of type '{0}' and id '{1}'", typeof(TAggregate), id);

                    throw new AggregateNotFoundException(message);
                }


                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);

            } while (!currentSlice.IsEndOfStream);

            var deserializedEvents = streamEvents.Select(e =>
            {
                var metadata = DeserializeObject<Dictionary<string, string>>(e.OriginalEvent.Metadata);
                var eventData = DeserializeObject(e.OriginalEvent.Data, metadata[EventClrTypeHeader]);
                return eventData as IEvent;
            });

            return BuildAggregate<TAggregate>(deserializedEvents);
        }

        public EventData CreateEventData(object @event)
        {
            var eventHeaders = new Dictionary<string, string>
            {
                {
                    EventClrTypeHeader, @event.GetType().AssemblyQualifiedName
                },
                {
                    "Domain", "Hse"
                }
            };
            var eventDataHeaders = SerializeObject(eventHeaders);
            var data = SerializeObject(@event);
            var eventData = new EventData(_guidIdProvider.GenerateId(), @event.GetType().Name, true, data, eventDataHeaders);
            return eventData;
        }

        private static string AggregateToStreamName(Type type, Guid id)
        {
            return string.Format("{0}-{1}-{2}", Category, type.Name, id);
        }

        private static T DeserializeObject<T>(byte[] data)
        {
            return (T)(DeserializeObject(data, typeof(T).AssemblyQualifiedName));
        }

        private static object DeserializeObject(byte[] data, string typeName)
        {
            var jsonString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName));
        }

        private static byte[] SerializeObject(object obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(jsonObj);
            return data;
        }
    }
}