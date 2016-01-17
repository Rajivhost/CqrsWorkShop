namespace Hse.CqrsWorkShop.Domain.Events

open System


type IEvent = 
    abstract member Id: Guid
