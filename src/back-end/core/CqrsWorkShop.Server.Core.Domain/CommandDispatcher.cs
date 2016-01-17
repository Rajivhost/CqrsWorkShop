using System;
using System.Collections.Generic;
using System.Linq;
using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<object, IAggregate>> _routes;
        private readonly IDomainRepository _domainRepository;
        private readonly IEnumerable<Action<object>> _postExecutionPipe;
        private readonly IEnumerable<Action<ICommand>> _preExecutionPipe;

        public CommandDispatcher(IDomainRepository domainRepository, IEnumerable<Action<ICommand>> preExecutionPipe, IEnumerable<Action<object>> postExecutionPipe)
        {
            _domainRepository = domainRepository;
            _postExecutionPipe = postExecutionPipe;
            _preExecutionPipe = preExecutionPipe ?? Enumerable.Empty<Action<ICommand>>();
            _routes =  new Dictionary<Type, Func<object, IAggregate>>();
        }

        public void RegisterHandler<TCommand>(IHandle<TCommand> handler) where TCommand : class, ICommand
        {
            _routes.Add(typeof (TCommand), command => handler.Handle(command as TCommand));
        }

        public async void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandType = command.GetType();

            RunPreExecutionPipe(command);
            if (!_routes.ContainsKey(commandType))
            {
                throw new ApplicationException("Missing handler for " + commandType.Name);
            }
            var aggregate = _routes[commandType](command);
            var savedEvents = await _domainRepository.SaveAsync(aggregate).ConfigureAwait(false);
            RunPostExecutionPipe(savedEvents);
        }

        private void RunPostExecutionPipe(IEnumerable<object> savedEvents)
        {
            foreach (var savedEvent in savedEvents)
            {
                foreach (var action in _postExecutionPipe)
                {
                    action(savedEvent);
                }
            }
        }

        private void RunPreExecutionPipe(ICommand command)
        {
            foreach (var action in _preExecutionPipe)
            {
                action(command);
            }
        }
    }
}