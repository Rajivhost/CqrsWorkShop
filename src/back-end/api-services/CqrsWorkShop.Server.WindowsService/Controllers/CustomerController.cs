using System.Threading.Tasks;
using System.Web.Http;
using Hse.CqrsWorkShop.Api.Models;
using Hse.CqrsWorkShop.Domain;
using Hse.CqrsWorkShop.Domain.Commands;

namespace Hse.CqrsWorkShop.Api.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IGuidIdProvider _guidIdProvider;

        public CustomerController(ICommandDispatcher commandDispatcher, IGuidIdProvider guidIdProvider)
        {
            _commandDispatcher = commandDispatcher;
            _guidIdProvider = guidIdProvider;
        }

        [HttpPost, Route("createcustomer")]
        public async Task<IHttpActionResult> CreatecustomerAsync(CreateCustomerDto createCustomerDto)
        {
            var id = _guidIdProvider.GenerateId();
            var createCustomer = new CreateCustomer(id, createCustomerDto.Name);

            await _commandDispatcher.DispatchAsync(createCustomer).ConfigureAwait(false);

            return Ok();
        }
    }
}