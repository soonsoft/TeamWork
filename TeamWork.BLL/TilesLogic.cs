using STIM.Modules;
using STIM.Data.EntityFrameworkExtensions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.BLL.Models;
using TeamWork.DAL;

namespace TeamWork.BLL
{
    public class TilesLogic : DisposableLogic<TeamDBContext>
    {
        protected override TeamDBContext CreateContext()
        {
            return new TeamDBContext();
        }
         
        public List<WorkTimeTileModel> GetWorkTimeDynamicData(Guid employeeId, DateTime day)
        {
            DatabaseAccess dba = DatabaseAccess;
            DbCommand cmd = dba.CreateCommand("SELECT BeginDate, [Description], SUM([Hours]) AS [Hours] FROM (SELECT SUBSTRING(CONVERT(varchar, w.BeginTime, 120), 0, 11) AS BeginDate, ISNULL(p.ProjectName, di.DictItemValue) AS [Description], w.[Hours] FROM WorkTime w LEFT JOIN Project p ON w.ProjectId = p.ProjectId INNER JOIN sys_DictionaryItem di ON w.AttendanceType = di.DictItemId WHERE w.EmployeeId = @EmployeeId AND w.BeginTime >= @BeginDate AND w.BeginTime <= @EndDate) t GROUP BY BeginDate, [Description]");
            dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
            dba.AddInParameter(cmd, "BeginDate", DbType.DateTime, QueryParameterUtility.DateTimeStart(day));
            dba.AddInParameter(cmd, "EndDate", DbType.DateTime, QueryParameterUtility.DateTimeEnd(day));

            List<WorkTimeTileModel> data = dba.ExecuteReader<List<WorkTimeTileModel>>(
                cmd, (d, r) => 
                {
                    d.Add(new WorkTimeTileModel
                        {
                            DateText = r["BeginDate"].ToString(),
                            Description = r["Description"].ToString(),
                            Hours = Convert.ToDecimal(r["Hours"])
                        });
                });
            return data;
        }

        public List<WorkHourTileModel> GetWorkHoursByMonth(Guid employeeId, DateTime begin, DateTime end)
        {
            DatabaseAccess dba = DatabaseAccess;
            DbCommand cmd = dba.CreateCommand("SELECT BeginDate, SUM([Hours]) AS [Hours], AVG(WorkHours) AS WorkHours FROM (SELECT SUBSTRING(CONVERT(varchar, w.BeginTime, 120), 0, 11) AS BeginDate, w.[Hours], ISNULL(wts.WorkHours, 8) AS WorkHours FROM WorkTime w LEFT JOIN WorkTimeSettings wts ON wts.WorkTimeSettingId = w.WorkTimeSettingId WHERE w.EmployeeId = @EmployeeId AND w.BeginTime >= @BeginDate AND w.BeginTime <= @EndDate) t GROUP BY BeginDate");

            dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
            dba.AddInParameter(cmd, "BeginDate", DbType.DateTime, QueryParameterUtility.DateTimeStart(begin));
            dba.AddInParameter(cmd, "EndDate", DbType.DateTime, QueryParameterUtility.DateTimeEnd(end));

            

            List<WorkHourTileModel> data = dba.ExecuteReader<List<WorkHourTileModel>>(
                cmd, (d, r) =>
                {
                    d.Add(new WorkHourTileModel
                    {
                        DateText = r["BeginDate"].ToString(),
                        Hours = Convert.ToDecimal(r["Hours"]),
                        WorkHours = Convert.ToDecimal(r["WorkHours"])
                    });
                });
            return data;
        }

        public string[] GetHolidayDays(DateTime day)
        {
            DateTime begin = QueryParameterUtility.DateTimeStart(day.AddDays(-(day.Day - 1)));
            DateTime end = QueryParameterUtility.DateTimeEnd(day.AddMonths(1).AddDays(-day.Day));

            DatabaseAccess dba = DatabaseAccess;
            DbCommand cmd = dba.CreateCommand("SELECT CONVERT(varchar, HolidayDate, 120) AS HolidayDate FROM HolidaySettings WHERE HolidayDate >= @p0 AND HolidayDate <= @p1");

            List<string> holidayDays = dba.ExecuteReader<List<string>>(
                cmd, 
                new object[] { begin, end }, 
                (d, r) =>
                {
                    d.Add(r.GetString(0));
                });

            return holidayDays.ToArray();
        }

        public List<object> UnapprovedWorkTime(Guid pm)
        {
            DatabaseAccess dba = DatabaseAccess;
            DbCommand cmd = dba.CreateCommand("SELECT t.ProjectName, COUNT(t.[Hours]) AS HourCount FROM (SELECT p.ProjectId, p.ProjectName, h.[Hours], p.PM, s.Manager FROM WorkTime h LEFT JOIN Project p ON h.ProjectId = p.ProjectId INNER JOIN Stage s ON p.ProjectId = s.ProjectId WHERE s.StageId = h.StageId AND (p.PM = @PM OR s.Manager = @PM) AND h.Approved <> 1 AND h.FaultFlag <> 1) t GROUP BY t.ProjectName");
            dba.AddInParameter(cmd, "PM", DbType.Guid, pm);
            List<object> data = new List<object>();
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    data.Add(new
                    {
                        ProjectName = reader.GetString(0),
                        HourCount = reader.GetInt32(1)
                    });
                }
            }
            return data;
        }

        public ProjectTileModel ProjectCount()
        {
            DatabaseAccess dba = DatabaseAccess;
            ProjectTileModel pModel = new ProjectTileModel();

            DbCommand cmd = dba.CreateCommand("SELECT t.DictItemValue AS Name, COUNT(t.ProjectId) AS Value FROM (SELECT di.DictItemValue, p.ProjectId FROM sys_DictionaryItem di LEFT JOIN (SELECT ProjectId, ParentId, [ProjectType] FROM Project WHERE ParentId IS NOT NULL) p ON di.DictItemId = p.ProjectType WHERE di.DictGroupId = '101') t GROUP BY t.DictItemValue");
            List<ProjectStatistic> data = dba.ExecuteReader<List<ProjectStatistic>>(
                cmd, (d, r) => 
                {
                    d.Add(new ProjectStatistic 
                    {
                        Name = r["Name"].ToString(),
                        Value = Convert.ToInt32(r["Value"])
                    });
                });
            if(data.Count > 0)
            {
                pModel.TypeStatistic = data;
            }

            cmd = dba.CreateCommand("SELECT t.DictItemValue AS Name, COUNT(t.ProjectId) AS Value FROM (SELECT di.DictItemValue, p.ProjectId FROM sys_DictionaryItem di LEFT JOIN (SELECT ProjectId, ParentId, [Status] FROM Project WHERE ParentId IS NOT NULL) p ON di.DictItemId = p.[Status] WHERE di.DictGroupId = '102') t GROUP BY t.DictItemValue");
            data = dba.ExecuteReader<List<ProjectStatistic>>(
                cmd, (d, r) =>
                {
                    d.Add(new ProjectStatistic
                    {
                        Name = r["Name"].ToString(),
                        Value = Convert.ToInt32(r["Value"])
                    });
                });
            if (data.Count > 0)
            {
                pModel.StatusStatistic = data;
            }
            return pModel;
        }
    }
}
