using System;
using System.Linq;
using Hse.CqrsWorkShop.Domain.Entities;
using Hse.CqrsWorkShop.Domain.Events;
using Hse.CqrsWorkShop.Domain.Exceptions;
using Microsoft.FSharp.Collections;

namespace Hse.CqrsWorkShop.Domain.Aggrates
{
    public class OrderAggregate : AggregateBase
    {
        private OrderState _orderState;

        private enum OrderState
        {
            ShippingProcessStarted,
            Created,
            Cancelled
        }

        public OrderAggregate()
        {
            RegisterTransition<OrderCreated>(Apply);
            RegisterTransition<ShippingProcessStarted>(Apply);
            RegisterTransition<OrderCancelled>(Apply);
        }

        private void Apply(OrderCancelled obj)
        {
            _orderState = OrderState.Cancelled;
        }

        private void Apply(ShippingProcessStarted obj)
        {
            _orderState = OrderState.ShippingProcessStarted;
        }

        private void Apply(OrderCreated obj)
        {
            _orderState = OrderState.Created;
            Id = obj.Id;
        }

        internal OrderAggregate(Guid basketId, FSharpList<OrderLine> orderLines) : this()
        {
            var id = GuidIdGenerator.GetNextId();
            RaiseEvent(new OrderCreated(id, basketId, orderLines));
            var totalPrice = orderLines.Sum(y => y.DiscountedPrice);
            if (totalPrice > 100000)
            {
                RaiseEvent(new NeedsApproval(id));
            }
            else
            {
                RaiseEvent(new OrderApproved(id));
            }
        }

        internal void Approve()
        {
            RaiseEvent(new OrderApproved(Id));
        }

        internal void StartShippingProcess()
        {
            if (_orderState == OrderState.Cancelled)
            {
                throw new OrderCancelledException();
            }

            RaiseEvent(new ShippingProcessStarted(Id));
        }

        internal void Cancel()
        {
            if (_orderState == OrderState.Created)
            {
                RaiseEvent(new OrderCancelled(Id));
            }
            else
            {
                throw new ShippingStartedException();
            }
        }

        internal void ShipOrder()
        {
            if (_orderState != OrderState.ShippingProcessStarted)
                throw new InvalidOrderState();
            RaiseEvent(new OrderShipped(Id));
        }
    }
}