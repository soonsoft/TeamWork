using SoonFramework.Service;
using SoonFramework.Service.WindowsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StartUp
{
    public class Starter : ServiceStarter
    {
        private readonly static string _exePath = Assembly.GetExecutingAssembly().Location;

        protected override void InstallWindowsService()
        {
            SelfInstaller.InstallMe(_exePath);
            Console.WriteLine("Windows服务安装完成");
        }

        protected override void UninstallWindowsService()
        {
            SelfInstaller.UninstallMe(_exePath);
            Console.WriteLine("Windows服务卸载完成");
        }

        protected override void RunAsConsole()
        {
            Console.WriteLine("尝试读取最新的考勤信息...");

            try
            {
                AttendanceLoader loader = new AttendanceLoader();
                loader.LoadNewAttendanceInfo();
            }
            catch(Exception e)
            {
                Console.WriteLine("发生错误");
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("读取完成，按任意键退出");
            Console.ReadKey();
		}
    }
}
