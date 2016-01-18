using Hse.CqrsWorkShop;
using LightInject;

namespace Hse.CqrsWorkShop.Api
{
    using Topshelf;

    public class ShopService : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}