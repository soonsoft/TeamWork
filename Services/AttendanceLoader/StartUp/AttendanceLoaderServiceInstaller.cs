using SoonFramework.Service.WindowsService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartUp
{
    [RunInstaller(true)]
    public class AttendanceLoaderServiceInstaller : WindowsServiceInstaller
    {
    }
}
