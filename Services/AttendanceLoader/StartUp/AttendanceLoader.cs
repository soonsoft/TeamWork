using Newtonsoft.Json.Linq;
using SoonFramework.Core.Logging;
using SoonFramework.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartUp
{
    public class AttendanceLoader
    {
        string directoryPath;
        string historyDirectoryPath;
        string userName;
        string password;
        string ip;

        ILog _logger;
        ILogFactory _logFactory;

        public AttendanceLoader()
        {
            directoryPath = ConfigurationManager.AppSettings["AttendancePath"];
            historyDirectoryPath = ConfigurationManager.AppSettings["HistoryDirectory"];
            userName = ConfigurationManager.AppSettings["UserName"];
            password = ConfigurationManager.AppSettings["Password"];
            ip = ConfigurationManager.AppSettings["IP"];

            InitLogger("考勤信息下载器");
        }

        public ILog Logger
        {
            get
            {
                return _logger;
            }
        }

        void InitLogger(string loggerName)
        {
            if (_logFactory == null)
            {
                _logFactory = new Log4NetLogFactory();
            }
            if (String.IsNullOrEmpty(loggerName))
            {
                loggerName = this.GetType().Name;
            }
            _logger = _logFactory.GetLog(loggerName);
        }

        DateTime? GetLastDate(DatabaseAccess dba)
        {
            DbCommand cmd = dba.CreateCommand("SELECT TOP(1) Clock FROM AttendanceInfo ORDER BY UpdateTime DESC");
            DateTime? lastDate = dba.ExecuteScalar(cmd) as DateTime?;
            return lastDate;
        }

        void CheckHistoryDirectory()
        {
            DirectoryInfo historyDirectory = new DirectoryInfo(historyDirectoryPath);
            if(!historyDirectory.Exists)
            {
                historyDirectory.Create();
            }
        }

        void MoveToHistory(FileInfo file)
        {
            file.CopyTo(Path.Combine(historyDirectoryPath, file.Name));
            file.Delete();
        }

        void SaveAttendanceInfo(FileInfo file, DatabaseAccess dba)
        {
            try
            {
                string jsonText = null;
                using (StreamReader reader = new StreamReader(file.OpenRead()))
                {
                    jsonText = reader.ReadToEnd();
                }

                if (String.IsNullOrEmpty(jsonText))
                {
                    throw new Exception(String.Format("{0} 没有读取到数据", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                }

                JArray array = JArray.Parse(jsonText);
                int affectRows = 0;
                using (var tran = dba.BeginTransaction())
                {
                    DbCommand cmd = dba.CreateCommand("INSERT INTO AttendanceInfo VALUES(@AttendanceInfoId, @DeviceId, @OutsideUserId, @Clock, @Remark, @UpdateTime)");
                    foreach (JToken token in array)
                    {
                        cmd.Parameters.Clear();

                        dba.AddInParameter(cmd, "AttendanceInfoId", DbType.Guid, Guid.NewGuid());
                        dba.AddInParameter(cmd, "DeviceId", DbType.String, token["DN"].Value<string>());
                        dba.AddInParameter(cmd, "OutsideUserId", DbType.String, token["DIN"].Value<string>());
                        dba.AddInParameter(cmd, "Clock", DbType.String, token["Clock"].Value<DateTime>());
                        dba.AddInParameter(cmd, "Remark", DbType.String, token["Remark"].Value<string>());
                        dba.AddInParameter(cmd, "UpdateTime", DbType.String, DateTime.Now);

                        affectRows += dba.ExecuteNonQuery(cmd);
                    }
                    tran.Complete();
                }

                Logger.Info(String.Format("{0} 成功写入文件[{1}]的考勤数据 {2} 条", 
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                    file.Name,
                    affectRows));
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public void LoadNewAttendanceInfo()
        {
            DatabaseAccess dba = DatabaseAccessFactory.CreateDatabase();
            using (var conn = dba.GetOpenConnection())
            {
                DateTime? lastDate = GetLastDate(dba);
                try
                {
                    using (IdentityScope iss = new IdentityScope(userName, password, ip))
                    {
                        CheckHistoryDirectory();

                        DirectoryInfo directory = new DirectoryInfo(directoryPath);
                        FileInfo[] files = directory.GetFiles();

                        /*
                        if (lastDate != null)
                        {
                            DateTime now = DateTime.Now;
                            now = now.AddHours(-now.Hour)
                                .AddMinutes(-now.Minute)
                                .AddSeconds(-now.Second)
                                .AddMilliseconds(-now.Millisecond);
                            DateTime last = lastDate.Value;
                            last = last.AddHours(-last.Hour)
                                .AddMinutes(-last.Minute)
                                .AddSeconds(-last.Second)
                                .AddMilliseconds(-last.Millisecond);
                            //因为每天只能读取前一天的数据，所以数据库中的时间比文件名时间早一天
                            last = last.AddDays(1);
                            int days = (now - last).Days;
                            if (days == 0)
                            {
                                return;
                            }

                            string[] dateArray = new string[days];
                            string dateFormat = "yyyyMMdd";
                            for (int i = 1; i <= days; i++)
                            {
                                dateArray[i - 1] = last.AddDays(i).ToString(dateFormat);
                            }
                            files = files.Where(f => dateArray.Any(d => f.Name.StartsWith(d))).ToArray();
                        }
                        */

                        foreach (FileInfo file in files)
                        {
                            SaveAttendanceInfo(file, dba);
                            MoveToHistory(file);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }
    }
}
