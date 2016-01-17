using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain
{
    public interface ICommandDispatcher
    {
        void RegisterHandler<TCommand>(IHandle<TCommand> handler) where TCommand : class, ICommand;
        void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand;
    }
}