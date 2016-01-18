using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain
{
    public interface ICommandDispatcher
    {
        void RegisterHandler<TCommand>(IHandle<TCommand> handler) where TCommand : class, ICommand;
        Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}