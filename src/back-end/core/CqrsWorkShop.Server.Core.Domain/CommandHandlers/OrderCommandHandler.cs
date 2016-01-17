
using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Aggrates;
using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Domain.CommandHandlers
{
    public class OrderCommandHandler : IOrderCommandHandler
    {
        private readonly IDomainRepository _domainRepository;

        public OrderCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<IAggregate> HandleAsync(ApproveOrder command)
        {
            var order = await _domainRepository.GetByIdAsync<OrderAggregate>(command.Id).ConfigureAwait(false);
            order.Approve();
            return order;
        }

        public async Task<IAggregate> HandleAsync(CancelOrder command)
        {
            var order = await _domainRepository.GetByIdAsync<OrderAggregate>(command.Id).ConfigureAwait(false);
            order.Cancel();
            return order;
        }

        public async Task<IAggregate> HandleAsync(ShipOrder command)
        {
            var order = await _domainRepository.GetByIdAsync<OrderAggregate>(command.Id).ConfigureAwait(false);
            order.ShipOrder();
            return order;
        }
    }
}