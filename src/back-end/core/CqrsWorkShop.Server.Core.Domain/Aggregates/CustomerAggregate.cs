using System;
using Hse.CqrsWorkShop.Domain.Aggrates;
using Hse.CqrsWorkShop.Domain.Events;

namespace Hse.CqrsWorkShop.Domain.Aggregates
{
    public class CustomerAggregate : AggregateBase
    {
        public CustomerAggregate()
        {
            RegisterTransition<CustomerCreated>(Apply);
            RegisterTransition<CustomerMarkedAsPreferred>(Apply);
        }

        private CustomerAggregate(Guid id, string name) : this()
        {
            RaiseEvent(new CustomerCreated(id, name));
        }

        internal int Discount { get; set; }

        private void Apply(CustomerCreated obj)
        {
            Id = obj.Id;
        }

        private void Apply(CustomerMarkedAsPreferred obj)
        {
            Discount = obj.Discount;
        }

        internal static IAggregate Create(Guid id, string name)
        {
            return new CustomerAggregate(id, name);
        }

        internal void MakePreferred(int discount)
        {
            RaiseEvent(new CustomerMarkedAsPreferred(Id, discount));
        }
    }
}