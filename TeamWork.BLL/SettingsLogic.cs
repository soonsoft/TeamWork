using STIM.Data.EntityFrameworkExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL
{
    /// <summary>
    /// 设置相关的业务逻辑
    /// </summary>
    public class SettingsLogic
    {
        #region 工时设置
        public bool SaveWorkTime(WorkTimeSetting time)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                WorkTimeSetting workTime = null;
                if (time.WorkTimeSettingId == Guid.Empty)
                {
                    workTime = time;
                    workTime.WorkTimeSettingId = Guid.NewGuid();
                    context.WorkTimeSettings.Add(workTime);
                }
                else
                {
                    var result = from w in context.WorkTimeSettings
                                 where w.WorkTimeSettingId == time.WorkTimeSettingId
                                 select w;
                    foreach (WorkTimeSetting t in result)
                    {
                        workTime = t;
                        break;
                    }
                    if (workTime == null)
                        return false;
                    workTime.AMStartTime = time.AMStartTime;
                    workTime.AMEndTime = time.AMEndTime;
                    workTime.PMStartTime = time.PMStartTime;
                    workTime.PMEndTime = time.PMEndTime;
                    workTime.Enabled = time.Enabled;
                    workTime.WorkHour = time.WorkHour;
                }
                
                var val = context.SaveChanges();
                return val > 0;
            }
        }

        public bool DeleteWorkTime(Guid timeId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                var result = from w in context.WorkTimeSettings
                             where w.WorkTimeSettingId == timeId
                             select w;
                WorkTimeSetting workTime = null;
                foreach(WorkTimeSetting t in result)
                {
                    workTime = t;
                    break;
                }
                if (workTime == null)
                    return false;
                context.WorkTimeSettings.Remove(workTime);
                var val = context.SaveChanges();
                return val > 0;
            }
        }

        public List<WorkTimeSetting> GetWorkTimeSetting()
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                var result = from w in context.WorkTimeSettings
                             select w;
                return result.ToList<WorkTimeSetting>();
            }
        }

        public WorkTimeSetting GetEnabledWorkTimeSetting()
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                WorkTimeSetting wts = context.WorkTimeSettings
                    .Where(s => s.Enabled == true)
                    .Single();
                return wts;
            }
        }

        #endregion

        #region 节假日设置

        public List<HolidaySetting> GetHolidaySetting(int year)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                var result = from h in context.HolidaySettings
                             where h.SettingYear == year
                             select h;
                return result.ToList<HolidaySetting>();
            }
        }

        public List<HolidaySetting> GetHolidaySetting(DateTime begin, DateTime end)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                var result = from h in context.HolidaySettings
                             where h.HolidayDate >= begin && h.HolidayDate <= end
                             select h;
                return result.ToList();
            }
        }

        public bool SaveHolidaySetting(HolidaySetting[] holidays, bool isInit)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("INSERT INTO HolidaySettings VALUES (@HolidayDate, @IsWeekend, @Festival, @SettingYear);");
                DbCommand cmd1 = dba.CreateCommand("");
                if(!isInit)
                {
                    cmd1.CommandText = "DELETE FROM HolidaySettings WHERE HolidayDate = @p1";
                    cmd1.CommandType = CommandType.Text;
                    dba.AddInParameter(cmd1, "p1", null);
                }
                var val = 0;
                var val1 = 0;
                var len = holidays.Length;
                using(var tran = dba.BeginTransaction())
                {
                    if (!isInit)
                    {
                        foreach (HolidaySetting hs in holidays)
                        {

                            cmd1.Parameters[0].Value = hs.HolidayDate;
                            val1 += dba.ExecuteNonQuery(cmd1);
                        }
                    }
                    val = dba.ExecuteNonQueryMultiple(cmd, holidays);
                    tran.Complete();
                }
                return val == len;
            }
        }

        public bool DeleteHolidaySetting(HolidaySetting[] holidays)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("DELETE FROM HolidaySettings WHERE HolidayDate = @p1");
                dba.AddInParameter(cmd, "p1", null);
                var len = holidays.Count();
                var val = 0;
                using (var tran = dba.BeginTransaction())
                {
                    foreach (HolidaySetting hs in holidays)
                    {
                        cmd.Parameters[0].Value = hs.HolidayDate;
                        val += dba.ExecuteNonQuery(cmd);
                    }
                    tran.Complete();
                }
                return val > 0;
            }
        }

        #endregion
    }
}
