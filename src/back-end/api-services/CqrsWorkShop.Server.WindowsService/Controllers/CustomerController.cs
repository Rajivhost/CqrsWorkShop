using System.Threading.Tasks;
using System.Web.Http;
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
        public IHttpActionResult Createcustomer(string name)
        {
            var id = _guidIdProvider.GenerateId();
            var createCustomer = new CreateCustomer(id, name);

            _commandDispatcher.ExecuteCommand(createCustomer);

            return Created("createcustomer", createCustomer);
        }
    }
}