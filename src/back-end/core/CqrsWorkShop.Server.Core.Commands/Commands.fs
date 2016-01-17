namespace Hse.CqrsWorkShop.Domain.Commands

open System

type ICommand = interface end

// Clients commands
type CreateClient = {Id: Guid; Name: string} with interface ICommand

// Order commands
type CreateOrder = {Id: Guid; ClientId: Guid} with interface ICommand
type AddItemToOrder = { Id: Guid; ProductId: Guid; Quantity: int } with interface ICommand
type CloseOrder = { OrderId: Guid } with interface ICommand


