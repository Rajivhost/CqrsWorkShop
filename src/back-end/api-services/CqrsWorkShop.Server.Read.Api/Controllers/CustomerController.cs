using System.Threading.Tasks;
using System.Web.Http;
using Hse.CqrsWorkShop.Data.Entities;
using LiteDB;

namespace Hse.CqrsWorkShop.Api.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly LiteCollection<Customer> _customersCollection;

        public CustomerController()
        {
            var liteDatabase = Database.Default;

            _customersCollection = liteDatabase.GetCollection<Customer>("customers");
        }

        [HttpGet, Route("getallcustomers")]
        public async Task<IHttpActionResult> GetAllAsync()
        {
            var customers = _customersCollection.FindAll();
            return Ok(customers);
        }
    }
}