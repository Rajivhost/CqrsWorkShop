using System;

namespace Hse.CqrsWorkShop.Data.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPreferred { get; set; }
        public int Discount { get; set; }
    }
}