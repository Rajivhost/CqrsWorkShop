using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<object, Task<IAggregate>>> _routes;
        private readonly IDomainRepository _domainRepository;

        public CommandDispatcher(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
            _routes =  new Dictionary<Type, Func<object, Task<IAggregate>>>();
        }

        public void RegisterHandler<TCommand>(IHandle<TCommand> handler) where TCommand : class, ICommand
        {
            _routes.Add(typeof (TCommand), async command => await handler.HandleAsync(command as TCommand).ConfigureAwait(false));
        }

        public async void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandType = command.GetType();

            if (!_routes.ContainsKey(commandType))
            {
                throw new ApplicationException("Missing handler for " + commandType.Name);
            }

            var aggregate = await _routes[commandType](command).ConfigureAwait(false);
            var savedEvents = await _domainRepository.SaveAsync(aggregate).ConfigureAwait(false);
        }
    }
}