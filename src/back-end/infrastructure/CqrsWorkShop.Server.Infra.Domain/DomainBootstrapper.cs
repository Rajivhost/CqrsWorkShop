using Catel;
using Hse.CqrsWorkShop.Domain.CommandHandlers;
using Hse.CqrsWorkShop.Domain.Repositories;
using LightInject;

namespace Hse.CqrsWorkShop.Domain
{
    public class DomainBootstrapper : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            Argument.IsNotNull("serviceRegistry", serviceRegistry);

            RegisterRepositories(serviceRegistry);
            RegisterCommandHandlers(serviceRegistry);
            RegisterDomainServices(serviceRegistry);
        }

        protected virtual void RegisterCommandHandlers(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<ICustomerCommandHandler, CustomerCommandHandler>();
            serviceRegistry.Register<IProductCommandHandler, ProductCommandHandler>();
            serviceRegistry.Register<IOrderCommandHandler, OrderCommandHandler>();
        }

        protected virtual void RegisterRepositories(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IDomainRepository, EventStoreDomainRepository>();
        }

        protected virtual void RegisterDomainServices(IServiceRegistry serviceRegistry)
        {
            //serviceRegistry.Register<ICommandDispatcher, CommandDispatcher>(new PerContainerLifetime());
            serviceRegistry.Register<ICommandDispatcherFactory>(factory => new CommandDispatcherFactory(factory), new PerContainerLifetime());
        }
    }
}