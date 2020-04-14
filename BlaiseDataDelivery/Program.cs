
using log4net;
using System.ServiceProcess;

namespace BlaiseDataDelivery
{
    static class Program
    {
        // Instantiate logger.
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main()
        {
            // Call the service class if run in debug mode so no need to install service for testing.
#if DEBUG           
            _logger.Info("Blaise data delivery service starting in DEBUG mode.");
            BlaiseDataDelivery dataDeliveryService = new BlaiseDataDelivery();
            dataDeliveryService.OnDebug();
#else
            _logger.Info("Blaise data delivery service starting in RELEASE mode.");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BlaiseDataDelivery()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
