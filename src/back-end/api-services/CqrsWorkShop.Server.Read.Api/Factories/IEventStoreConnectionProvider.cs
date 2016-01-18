using System.Net;
using EventStore.ClientAPI;

namespace Hse.CqrsWorkShop.Domain
{
    public interface IEventStoreConnectionProvider
    {
        IEventStoreConnection GetConnection();
        IPAddress EventStoreIp { get; }
        int EventStorePort { get; }
    }
}