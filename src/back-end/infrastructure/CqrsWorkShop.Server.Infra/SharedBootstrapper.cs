namespace Hse.CqrsWorkShop
{
    using LightInject;
    public class SharedBootstrapper : BootstrapperBase
    {
        protected virtual void RegisterServices(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IGuidIdProvider, GuidIdProvider>();
        }

        public override void Compose(IServiceRegistry serviceRegistry)
        {
            RegisterServices(serviceRegistry);
        }
    }
}
