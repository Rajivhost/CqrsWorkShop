using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Aggregates;
using Hse.CqrsWorkShop.Domain.Commands;
using Hse.CqrsWorkShop.Domain.Exceptions;

namespace Hse.CqrsWorkShop.Domain.CommandHandlers
{
    public class CustomerCommandHandler : ICustomerCommandHandler
    {
        private readonly IDomainRepository _domainRepository;

        public CustomerCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<IAggregate> HandleAsync(CreateCustomer command)
        {
            try
            {
                var customer = await _domainRepository.GetByIdAsync<CustomerAggregate>(command.Id).ConfigureAwait(false);
                throw new CustomerAlreadyExistsException(command.Id);
            }
            catch (AggregateNotFoundException)
            {
                // We expect not to find anything
            }
            return CustomerAggregate.Create(command.Id, command.Name);
        }

        public async Task<IAggregate> HandleAsync(MarkCustomerAsPreferred command)
        {
            var customer = await _domainRepository.GetByIdAsync<CustomerAggregate>(command.Id).ConfigureAwait(false);
            customer.MakePreferred(command.Discount);
            return customer;
        }
    }
}