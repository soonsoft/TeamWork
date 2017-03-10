using BingImageDownloader;
using BingImageDownloader.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingImageDownloadService
{
    public partial class BingImageDownload : ServiceBase
    {
        public BingImageDownload()
        {
            InitializeComponent();
        }

        private Timer timer = null;

        protected override void OnStart(string[] args)
        {
            BingImageManager bingManager = new BingImageManager("cn.bing.com", "China");
            timer = new Timer(stateInfo =>
            {
                BingImageManager manager = (BingImageManager)stateInfo;
                bool result = false;
                try
                {
                    Log.Get().Info("开始检查bing图片下载情况！");
                    if (!bingManager.ContainsTodayBingImage())
                    {
                        Log.Get().Info("开始下载！");
                        result = bingManager.SaveTodayBingImage();
                    }

                }
                catch (Exception exception)
                {
                    Log.Get().Error("下载bing图片出现错误！", exception);
                }
                finally
                {
                    DateTime after;
                    if (result)
                    {
                        after = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:05:00")).AddDays(1);
                    }
                    else
                    {
                        after = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1);
                    }
                    TimeSpan time = after - DateTime.Now;
                    timer.Change(time, TimeSpan.Zero);
                }
            }, bingManager, TimeSpan.Zero, TimeSpan.Zero);
        }

        protected override void OnStop()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}
