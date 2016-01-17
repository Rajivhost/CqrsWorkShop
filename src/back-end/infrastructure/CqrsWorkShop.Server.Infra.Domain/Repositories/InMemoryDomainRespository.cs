using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI.Exceptions;
using Hse.CqrsWorkShop.Domain.Events;
using Hse.CqrsWorkShop.Domain.Exceptions;
using Newtonsoft.Json;

namespace Hse.CqrsWorkShop.Domain.Repositories
{
    public class InMemoryDomainRespository : DomainRepositoryBase
    {
        public Dictionary<Guid, List<string>> EventStore = new Dictionary<Guid, List<string>>();
        private readonly List<IEvent> _latestEvents = new List<IEvent>();
        private readonly JsonSerializerSettings _serializationSettings;

        public InMemoryDomainRespository()
        {
            _serializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public override Task<IEnumerable<IEvent>> SaveAsync<TAggregate>(TAggregate aggregate)
        {
            return Task.Run(() =>
            {
                var eventsToSave = aggregate.UncommitedEvents().ToList();
                var serializedEvents = eventsToSave.Select(Serialize).ToList();
                var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);
                if (expectedVersion < 0)
                {
                    EventStore.Add(aggregate.Id, serializedEvents);
                }
                else
                {
                    var existingEvents = EventStore[aggregate.Id];
                    var currentversion = existingEvents.Count - 1;
                    if (currentversion != expectedVersion)
                    {
                        throw new WrongExpectedVersionException("Expected version " + expectedVersion +
                                                                " but the version is " + currentversion);
                    }
                    existingEvents.AddRange(serializedEvents);
                }
                _latestEvents.AddRange(eventsToSave);
                aggregate.ClearUncommitedEvents();
                return eventsToSave.AsEnumerable();
            });
        }

        public override Task<TAggregate> GetByIdAsync<TAggregate>(Guid id)
        {
            return Task.Run(() =>
            {
                if (EventStore.ContainsKey(id))
                {
                    var events = EventStore[id];
                    var deserializedEvents = events.Select(e => JsonConvert.DeserializeObject(e, _serializationSettings) as IEvent);
                    return BuildAggregate<TAggregate>(deserializedEvents);
                }
                throw new AggregateNotFoundException("Could not found aggregate of type " + typeof(TAggregate) + " and id " + id);
            });
        }

        private string Serialize(IEvent arg)
        {
            return JsonConvert.SerializeObject(arg, _serializationSettings);
        }

        public IEnumerable<IEvent> GetLatestEvents()
        {
            return _latestEvents;
        }

        public void AddEvents(Dictionary<Guid, IEnumerable<IEvent>> eventsForAggregates)
        {
            foreach (var eventsForAggregate in eventsForAggregates)
            {
                EventStore.Add(eventsForAggregate.Key, eventsForAggregate.Value.Select(Serialize).ToList());
            }
        }
    }
}