namespace Hse.CqrsWorkShop.Domain.Events

open System
open Hse.CqrsWorkShop.Domain.Entities


type IEvent = 
    abstract member Id: Guid


//Customer events
type CustomerCreated = {Id: Guid; Name: string } 
    with interface IEvent with member this.Id with get() = this.Id

type CustomerMarkedAsPreferred = {Id: Guid; Discount: int }
    with interface IEvent with member this.Id with get() = this.Id

// Product events
type ProductCreated = {Id: Guid; Name: string; Price: int }
    with interface IEvent with member this.Id with get() = this.Id

// Item events

type ItemAdded = { Id: Guid; OrderLine: OrderLine}
    with 
    override this.ToString() = sprintf "Item added. Id: %O, %O" this.Id this.OrderLine
    interface IEvent with member this.Id with get() = this.Id

// Order events
type OrderCreated ={ Id: Guid; BasketId: Guid; OrderLines: OrderLine list }
    with interface IEvent with member this.Id with get() = this.Id

type ShippingProcessStarted = {Id: Guid}
    with interface IEvent with member this.Id with get() = this.Id

type OrderCancelled = {Id: Guid}
    with interface IEvent with member this.Id with get() = this.Id

type OrderShipped = {Id: Guid}
    with interface IEvent with member this.Id with get() = this.Id

type NeedsApproval = {Id: Guid}
    with interface IEvent with member this.Id with get() = this.Id

type OrderApproved = {Id: Guid}
    with interface IEvent with member this.Id with get() = this.Id
 

