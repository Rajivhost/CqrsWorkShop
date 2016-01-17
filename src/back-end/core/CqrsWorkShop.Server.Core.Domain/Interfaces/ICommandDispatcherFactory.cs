namespace Hse.CqrsWorkShop.Domain
{
    public interface ICommandDispatcherFactory
    {
        ICommandDispatcher CreateCommandDispatcher();
    }
}