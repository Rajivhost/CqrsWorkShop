using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain.CommandHandlers
{
    public interface IProductCommandHandler : IHandle<CreateProduct>
    {
    }
}