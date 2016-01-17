using Catel;
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
            RegisterDomainServices(serviceRegistry);
        }

        protected virtual void RegisterRepositories(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IDomainRepository, EventStoreDomainRepository>();
        }

        protected virtual void RegisterDomainServices(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<ICommandDispatcher, CommandDispatcher>(new PerContainerLifetime());
        }
    }
}