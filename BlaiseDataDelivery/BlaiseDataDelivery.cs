using Blaise.Queue.Api;
using Blaise.Queue.Contracts.Interfaces;
using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services;
using BlaiseDataDelivery.Interfaces.Services.File;
using BlaiseDataDelivery.Interfaces.Services.Queue;
using BlaiseDataDelivery.Mappers;
using BlaiseDataDelivery.MessageHandlers;
using BlaiseDataDelivery.Providers;
using BlaiseDataDelivery.Services;
using BlaiseDataDelivery.Services.File;
using BlaiseDataDelivery.Services.Queue;
using System.ServiceProcess;
using Unity;

namespace BlaiseDataDelivery
{
    partial class BlaiseDataDelivery : ServiceBase
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IInitialiseDeliveryService _dataDeliveryService;

        public BlaiseDataDelivery()
        {
            InitializeComponent();

            //IOC container
            _unityContainer = new UnityContainer();

            //register dependencies
            _unityContainer.RegisterSingleton<IFluentQueueProvider, FluentQueueProvider>();
            _unityContainer.RegisterType<IConfigurationProvider, ConfigurationProvider>();

            _unityContainer.RegisterType<ISubscriptionService, SubscriptionService>();

            _unityContainer.RegisterType<IMessageModelMapper, MessageModelMapper>();

            _unityContainer.RegisterType<IFileProcessingService, FileProcessingService>();
            _unityContainer.RegisterType<IFileEncryptionService, FileEncryptionService>();
            _unityContainer.RegisterType<IZipFileCreationService, ZipFileCreationService>();

            //main service classes
            _unityContainer.RegisterType<IInitialiseDeliveryService, InitialiseDeliveryService>();
            _unityContainer.RegisterType<IMessageHandlerCallback, DataDeliveryMessageHandler>();

            //resolve all dependencies as DataDeliveryService class is the main service entry point
            _dataDeliveryService = _unityContainer.Resolve<IInitialiseDeliveryService>();
        }

        protected override void OnStart(string[] args)
        {
            _dataDeliveryService.SetupSubscription();
        }

        protected override void OnStop()
        {
            _dataDeliveryService.CancelSubscription();
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }
}
