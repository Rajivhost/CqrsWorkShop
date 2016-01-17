using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain.CommandHandlers
{
    public interface ICustomerCommandHandler : IHandle<CreateCustomer>, IHandle<MarkCustomerAsPreferred>
    {
    }
}