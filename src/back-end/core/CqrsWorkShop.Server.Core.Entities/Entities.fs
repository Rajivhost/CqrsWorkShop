namespace Hse.CqrsWorkShop.Domain.Entities

open System


type IEntity = 
    abstract member Id: int

type OrderLine = 
    {ProductId: Guid; ProductName: string; OriginalPrice: int; DiscountedPrice: int; Quantity: int}
