using Hse.CqrsWorkShop.Domain;
using LightInject;

namespace Hse.CqrsWorkShop
{
    public class ApiBootstrapper : ServiceContainer
    {
        public ApiBootstrapper()
        {
            RegisterFrom<SharedBootstrapper>();
            RegisterFrom<DomainBootstrapper>();

            var commandDispatcherFactory = GetInstance<ICommandDispatcherFactory>();
            var commandDispatcher = commandDispatcherFactory.CreateCommandDispatcher();
            RegisterInstance(commandDispatcher);
        }
    }
}