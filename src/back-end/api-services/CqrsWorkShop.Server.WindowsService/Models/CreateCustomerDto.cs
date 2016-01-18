namespace Hse.CqrsWorkShop.Api.Models
{
    public class CreateCustomerDto
    {
        public CreateCustomerDto(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
