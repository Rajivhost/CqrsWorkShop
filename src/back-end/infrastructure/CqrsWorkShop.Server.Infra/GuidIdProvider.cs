using System;
using MassTransit;

namespace Hse.CqrsWorkShop
{
    public class GuidIdProvider : IGuidIdProvider
    {
        public Guid GenerateId()
        {
            var id = NewId.Next().ToGuid();

            return id;
        }
    }
}