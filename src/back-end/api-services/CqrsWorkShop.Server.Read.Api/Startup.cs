using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web.Http;
using EventStore.ClientAPI;
using Hse.CqrsWorkShop.Data;
using Hse.CqrsWorkShop.Data.Entities;
using Hse.CqrsWorkShop.Data.EventHandlers;
using Hse.CqrsWorkShop.Domain;
using Hse.CqrsWorkShop.Domain.Events;
using LightInject;
using LiteDB;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Diagnostics;
using Newtonsoft.Json.Serialization;
using Owin;
using Topshelf;

namespace Hse.CqrsWorkShop.Api
{
    public static class Database
    {
        static Database()
        {
            var connectionString = string.Format("filename={0}; journal=false; version=5", AppDomain.CurrentDomain.BaseDirectory + "Database.db");

            Default = new LiteDatabase(connectionString);
        }

        public static LiteDatabase Default { get; private set; }
    }

    public class EventHandlerService : ServiceControl
    {
        private Dictionary<Type, Action<object>> _eventHandlerMapping;
        private Position? _latestPosition;
        private ServiceContainer _serviceContainer;
        private EventStoreConnectionProvider _eventStoreConnectionProvider;
        private IEventStoreConnection _connection;
        private EventDispatcher _eventDispatcher;


        public bool Start(HostControl hostControl)
        {
            _serviceContainer = new ServiceContainer();

            _eventDispatcher = new EventDispatcher();

            _eventDispatcher.RegisterHandler<CustomerCreated>(new CustomerEventHandler());
            _eventDispatcher.RegisterHandler<CustomerMarkedAsPreferred>(new CustomerEventHandler());


            _eventStoreConnectionProvider = new EventStoreConnectionProvider();

            _eventHandlerMapping = CreateEventHandlerMapping();
            ConnectToEventstore();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _serviceContainer.Dispose();
            return true;
        }

        private async void ConnectToEventstore()
        {

            _latestPosition = Position.Start;
            _connection = _eventStoreConnectionProvider.GetConnection();
            _connection.Connected +=
                (sender, args) => _connection.SubscribeToAllFrom(_latestPosition, false, HandleEvent);
            await _connection.ConnectAsync().ConfigureAwait(false);
            Console.WriteLine("Indexing service started");
        }

        private void HandleEvent(EventStoreCatchUpSubscription arg1, ResolvedEvent arg2)
        {
            var @event = EventSerialization.DeserializeEvent(arg2.OriginalEvent);
            if (@event != null)
            {
                var eventType = @event.GetType();
                if (_eventHandlerMapping.ContainsKey(eventType))
                {
                    _eventHandlerMapping[eventType](@event);
                }
            }
            _latestPosition = arg2.OriginalPosition;
        }

        private Dictionary<Type, Action<object>> CreateEventHandlerMapping()
        {
            return new Dictionary<Type, Action<object>>
            {
                {typeof (CustomerCreated), async o => await _eventDispatcher.DispatchAsync(o as CustomerCreated).ConfigureAwait(false)},
                {typeof (CustomerMarkedAsPreferred), async o => await _eventDispatcher.DispatchAsync(o as CustomerMarkedAsPreferred).ConfigureAwait(false)},
                //{typeof (ItemAdded), o => Handle(o as ItemAdded)},
                //{typeof (OrderCreated), o => Handle(o as OrderCreated)},
                //{typeof (ProductCreated), o => Handle(o as ProductCreated)}
            };
        }

        //private void Handle(OrderCreated evt)
        //{
        //    var existinBasket = _indexer.Get<Basket>(evt.BasketId);
        //    existinBasket.BasketState = BasketState.Paid;
        //    _indexer.Index(existinBasket);
        //}

        //private void Handle(ItemAdded evt)
        //{
        //    var existingBasket = _indexer.Get<Basket>(evt.Id);
        //    var orderLines = existingBasket.OrderLines;
        //    if (orderLines == null || orderLines.Length == 0)
        //    {
        //        existingBasket.OrderLines = new[] { evt.OrderLine };
        //    }
        //    else
        //    {
        //        var orderLineList = orderLines.ToList();
        //        orderLineList.Add(evt.OrderLine);
        //        existingBasket.OrderLines = orderLineList.ToArray();
        //    }

        //    _indexer.Index(existingBasket);

        //    _graphClient.Cypher
        //        .Match("(basket:Basket)", "(product:Product)")
        //        .Where((Basket basket) => basket.Id == evt.Id)
        //        .AndWhere((Product product) => product.Id == evt.OrderLine.ProductId)
        //        .Create("basket-[:HAS_ORDERLINE {orderLine}]->product")
        //        .WithParam("orderLine", evt.OrderLine)
        //        .ExecuteWithoutResults();
        //}

        //private void Handle(ProductCreated evt)
        //{
        //    var product = new Product()
        //    {
        //        Id = evt.Id,
        //        Name = evt.Name,
        //        Price = evt.Price
        //    };
        //    _indexer.Index(product);
        //    _graphClient.Cypher
        //        .Create("(product:Product {newProduct})")
        //        .WithParam("newProduct", product)
        //        .ExecuteWithoutResults();
        //}

        //private void Handle(CustomerMarkedAsPreferred evt)
        //{
        //    var customer = _customersCollection.Find(customer1 => customer1.Id == evt.Id).FirstOrDefault();

        //    customer.IsPreferred = true;
        //    customer.Discount = evt.Discount;

        //    _customersCollection.Update(customer);
        //}

        //private void Handle(CustomerCreated evt)
        //{
        //    var customer = new Customer
        //    {
        //        Id = evt.Id,
        //        Name = evt.Name
        //    };

        //    _customersCollection.Insert(customer);
        //}
    }

    public class Startup : ServiceContainer
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureWebApi(app);

            ConfigureDiagnostics(app);
        }

        private static void ConfigureRouting(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();
        }

        private static void ConfigureFormatters(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Formatters.Remove(httpConfiguration.Formatters.XmlFormatter);
            httpConfiguration.Formatters.Remove(httpConfiguration.Formatters.FormUrlEncodedFormatter);
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }

        private static void ConfigureDiagnostics(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage(new ErrorPageOptions
            {
                //Shows the OWIN environment dictionary keys and values.
                //This detail is enabled by default if you are running your app from VS unless disabled in code. 
                ShowEnvironment = true,
                //Hides cookie details
                ShowCookies = true,
                //Shows the lines of code throwing this exception.
                //This detail is enabled by default if you are running your app from VS unless disabled in code. 
                ShowSourceCode = true
            });
#endif

            app.UseWelcomePage();
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            app.Map("/api", builder =>
            {
                var httpConfiguration = new HttpConfiguration();

                this.RegisterApiControllers();
                this.EnableWebApi(httpConfiguration);

                ConfigureRouting(httpConfiguration);
                ConfigureFormatters(httpConfiguration);

                builder.UseCors(CorsOptions.AllowAll);

                builder.UseWebApi(httpConfiguration);
            });
        }
    }
}