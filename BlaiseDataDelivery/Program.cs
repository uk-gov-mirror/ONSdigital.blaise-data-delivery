using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlaiseDataDelivery
{
    class Program
    {
        static void Main(string[] args)
        {
            // Call the service class if run in debug mode so no need to install service for testing.
#if DEBUG           
            BlaiseDataDelivery dataDeliveryService = new BlaiseDataDelivery();
            dataDeliveryService.OnDebug();
#else
            log.Info("Blaise data deliveryy service starting in RELEASE mode.");
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
