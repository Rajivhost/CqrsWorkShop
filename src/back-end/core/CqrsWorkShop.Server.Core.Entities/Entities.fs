namespace Hse.CqrsWorkShop.Domain.Entities

open System


type IEntity = 
    abstract member Id: int

type OrderLine = 
    {Id: int; ProductId: Guid; ProductName: string; OriginalPrice: int; DiscountedPrice: int; Quantity: int}
    with interface IEntity with member this.Id with get() = this.Id
