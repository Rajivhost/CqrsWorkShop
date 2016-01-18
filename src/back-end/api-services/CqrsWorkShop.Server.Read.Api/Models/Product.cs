using System;

namespace Hse.CqrsWorkShop.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}