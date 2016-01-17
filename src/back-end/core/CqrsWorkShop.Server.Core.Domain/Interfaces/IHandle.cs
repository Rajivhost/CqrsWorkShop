using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain
{
    public interface IHandle<in TCommand> where TCommand : ICommand
    {
        Task<IAggregate> HandleAsync(TCommand command);
    }
}