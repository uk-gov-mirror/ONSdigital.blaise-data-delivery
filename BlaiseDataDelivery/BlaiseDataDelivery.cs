using Blaise.Queue.Api;
using Blaise.Queue.Contracts.Interfaces;
using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services;
using BlaiseDataDelivery.MessageHandler;
using BlaiseDataDelivery.Providers;
using BlaiseDataDelivery.Services;
using System.ServiceProcess;
using Unity;

namespace BlaiseDataDelivery
{
    partial class BlaiseDataDelivery : ServiceBase
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDataDeliveryService _dataDeliveryService;

        public BlaiseDataDelivery()
        {
            InitializeComponent();

            //IOC container
            _unityContainer = new UnityContainer();

            //register dependencies
            _unityContainer.RegisterSingleton<IFluentQueueProvider, FluentQueueProvider>();
            _unityContainer.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            _unityContainer.RegisterType<IMessageHandlerCallback, TestQueueEventHandlerCallback>();
            _unityContainer.RegisterType<ISubscriptionService, SubscriptionService>();
            _unityContainer.RegisterType<IPublishService, PublishService>();

            //main service
            _unityContainer.RegisterType<IDataDeliveryService, DataDeliveryService>();

            //resolve all dependencies as DataDeliveryService class is the main service entry point
            _dataDeliveryService = _unityContainer.Resolve<IDataDeliveryService>();
        }

        protected override void OnStart(string[] args)
        {
            _dataDeliveryService.Start();
        }

        protected override void OnStop()
        {
            _dataDeliveryService.Stop();
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }
}
