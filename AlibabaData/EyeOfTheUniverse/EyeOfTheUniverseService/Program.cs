using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfTheUniverseService
{
    static class Program
    {
        static void Main()
        {
#if DEBUG
            var service = new EyeOfTheUniverseListenUpdateService();
            service.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EyeOfTheUniverseListenUpdateService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
