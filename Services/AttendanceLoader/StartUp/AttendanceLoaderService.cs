using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StartUp
{
    partial class AttendanceLoaderService : ServiceBase
    {
        Timer timer = null;

        public AttendanceLoaderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            AttendanceLoader loader = new AttendanceLoader();
            timer = new Timer(stateInfo =>
            {
                AttendanceLoader al = (AttendanceLoader)stateInfo;
                al.LoadNewAttendanceInfo();

                DateTime now = DateTime.Now;
                DateTime tomorrow = DateTime.Parse(now.ToString("yyyy-MM-dd 06:00:00")).AddDays(1);
                timer.Change(tomorrow - now, TimeSpan.Zero);
            }, loader, TimeSpan.Zero, TimeSpan.Zero);
        }

        protected override void OnStop()
        {
            if(timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        protected override void OnShutdown()
        {
            OnStop();
        }
    }
}
