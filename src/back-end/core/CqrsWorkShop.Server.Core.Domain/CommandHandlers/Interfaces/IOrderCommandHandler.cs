using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain.CommandHandlers
{
    public interface IOrderCommandHandler : IHandle<ApproveOrder>, IHandle<CancelOrder>
    {
    }
}