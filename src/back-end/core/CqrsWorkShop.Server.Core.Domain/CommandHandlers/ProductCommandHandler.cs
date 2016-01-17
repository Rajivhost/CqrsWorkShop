using System.Threading.Tasks;
using Hse.CqrsWorkShop.Domain.Aggrates;
using Hse.CqrsWorkShop.Domain.Commands;
using Hse.CqrsWorkShop.Domain.Exceptions;

namespace Hse.CqrsWorkShop.Domain.CommandHandlers
{
    public class ProductCommandHandler : IProductCommandHandler
    {
        private readonly IDomainRepository _domainRepository;

        public ProductCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<IAggregate> HandleAsync(CreateProduct command)
        {
            try
            {
                var product = await _domainRepository.GetByIdAsync<ProductAggregate>(command.Id).ConfigureAwait(false);
                throw new ProductAlreadyExistsException(command.Id);
            }
            catch (AggregateNotFoundException)
            {
                // We expect not to find anything
            }
            return ProductAggregate.Create(command.Id, command.Name, command.Price);
        }
    }
}