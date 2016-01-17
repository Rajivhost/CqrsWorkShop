namespace Hse.CqrsWorkShop
{
    using LightInject;
    public abstract class BootstrapperBase : IBootstrapper
    {
        public abstract void Compose(IServiceRegistry serviceRegistry);
    }
}