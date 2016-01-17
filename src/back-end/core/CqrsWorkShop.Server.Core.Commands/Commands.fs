namespace Hse.CqrsWorkShop.Domain.Commands

open System

type ICommand = interface end

// Customer commands
type CreateCustomer = {Id: Guid; Name: string} with interface ICommand
type MarkCustomerAsPreferred = {Id: Guid; Discount: int } with interface ICommand

// Order commands
type CreateOrder = {Id: Guid; ClientId: Guid} with interface ICommand
type AddItemToOrder = { Id: Guid; ProductId: Guid; Quantity: int } with interface ICommand
type CancelOrder = { Id: Guid } with interface ICommand
type ShipOrder = { Id: Guid } with interface ICommand
type ApproveOrder = { Id: Guid } with interface ICommand

// Product commands
type CreateProduct = {Id: Guid; Name: string; Price: int } with interface ICommand

