using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace StartUp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                RunAsService();
                return;
            }

            Starter starter = new Starter();
            starter.Start(args);
        }

        static void RunAsService()
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] { new AttendanceLoaderService() };
            ServiceBase.Run(servicesToRun);
        }
    }
}
