using Hse.CqrsWorkShop.Domain.CommandHandlers;
using Hse.CqrsWorkShop.Domain.Commands;
using LightInject;

namespace Hse.CqrsWorkShop.Domain
{
    public class CommandDispatcherFactory : ICommandDispatcherFactory
    {
        private readonly IServiceFactory _serviceFactory;

        public CommandDispatcherFactory(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public ICommandDispatcher CreateCommandDispatcher()
        {
            var domainRepository = _serviceFactory.GetInstance<IDomainRepository>();

            var commandDispatcher = new CommandDispatcher(domainRepository);

            var customerCommandHandler = _serviceFactory.GetInstance<ICustomerCommandHandler>();
            commandDispatcher.RegisterHandler<CreateCustomer>(customerCommandHandler);
            commandDispatcher.RegisterHandler<MarkCustomerAsPreferred>(customerCommandHandler);

            var productCommandHandler = _serviceFactory.GetInstance<IProductCommandHandler>();
            commandDispatcher.RegisterHandler(productCommandHandler);

            var orderCommandHanler = _serviceFactory.GetInstance<IOrderCommandHandler>();
            commandDispatcher.RegisterHandler<ApproveOrder>(orderCommandHanler);
            commandDispatcher.RegisterHandler<CancelOrder>(orderCommandHanler);

            return commandDispatcher;
        }
    }
}