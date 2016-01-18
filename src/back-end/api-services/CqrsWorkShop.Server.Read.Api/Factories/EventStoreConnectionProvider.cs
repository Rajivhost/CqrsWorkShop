using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Hse.CqrsWorkShop.Domain
{
    public class EventStoreConnectionProvider : IEventStoreConnectionProvider
    {
        public IEventStoreConnection GetConnection()
        {
            ConnectionSettings settings =
                ConnectionSettings.Create()
                    .UseConsoleLogger()
                    .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));
            var endPoint = new IPEndPoint(EventStoreIp, EventStorePort);
            var connection = EventStoreConnection.Create(settings, endPoint, null);
            //await connection.ConnectAsync().ConfigureAwait(false);
            return connection;
        }

        public IPAddress EventStoreIp
        {
            get
            {
                var hostname = ConfigurationManager.AppSettings["EventStoreHostName"];
                if (string.IsNullOrEmpty(hostname))
                {
                    return IPAddress.Loopback;
                }
                var ipAddresses = Dns.GetHostAddresses(hostname);
                return ipAddresses[0];
            }
        }

        public int EventStorePort
        {
            get
            {
                var esPort = ConfigurationManager.AppSettings["EventStorePort"];
                return string.IsNullOrEmpty(esPort) ? 1113 : int.Parse(esPort);
            }
        }
    }
}