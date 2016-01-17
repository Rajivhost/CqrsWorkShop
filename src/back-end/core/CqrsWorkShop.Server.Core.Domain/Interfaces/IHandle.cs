using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain
{
    public interface IHandle<in TCommand> where TCommand : ICommand
    {
        IAggregate Handle(TCommand command);
    }
}