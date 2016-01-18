using System;
using System.Linq;
using System.Threading.Tasks;
using Hse.CqrsWorkShop.Api;
using Hse.CqrsWorkShop.Data.Entities;
using Hse.CqrsWorkShop.Domain.Events;
using LiteDB;

namespace Hse.CqrsWorkShop.Data.EventHandlers
{
    public class CustomerEventHandler : ICustomerEventHandler
    {
        private readonly LiteCollection<Customer> _customersCollection;

        public CustomerEventHandler()
        {
            var liteDatabase = Database.Default;

            _customersCollection = liteDatabase.GetCollection<Customer>("customers");
        }

        public async Task HandleAsync(CustomerCreated customerCreated)
        {
            if (CustomerExists(customerCreated.Id)) return;

            var customer = new Customer
            {
                Id = customerCreated.Id,
                Name = customerCreated.Name
            };

            _customersCollection.Insert(customer);
        }

        public async Task HandleAsync(CustomerMarkedAsPreferred customerMarkedAsPreferred)
        {
            var customer = _customersCollection.Find(customer1 => customer1.Id == customerMarkedAsPreferred.Id).FirstOrDefault();

            if (customer != null)
            {
                customer.IsPreferred = true;
                customer.Discount = customerMarkedAsPreferred.Discount;

                _customersCollection.Update(customer);
            }
        }

        private bool CustomerExists(Guid id)
        {
            return _customersCollection.FindAll().Any(customer => customer.Id == id);
        }
    }
}