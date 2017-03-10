using STIM.Core;
using STIM.Data.EntityFrameworkExtensions;
using STIM.Modules;
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
    /// <summary>
    /// 工时逻辑
    /// </summary>
    public class WorkTimeLogic
    {
        /// <summary>
        /// 获取某一年登记工时的日期
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<DateTime> GetWorkDateByYear(Guid employeeId, int year)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT t.DateText FROM(SELECT SUBSTRING(CONVERT(varchar, BeginTime, 120), 0, 11) AS DateText FROM WorkTime WHERE YEAR(BeginTime) = @Year AND EmployeeId = @EmployeeId) t GROUP BY t.DateText");
                dba.AddInParameter(cmd, "Year", DbType.Int32, (object)year);
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);

                List<DateTime> workDates = dba.ExecuteReader<List<DateTime>>(
                    cmd, (d, r) => 
                    {
                        d.Add(DateTime.Parse(r["DateText"].ToString()));
                    });
                return workDates;

            }
        }

        public List<WorkTimeModel> GetWorkTimeByMonth(Guid employeeId, int year, int month)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT w.WorkTimeId, w.BeginTime, w.EndTime, w.ProjectId, p.ProjectName, w.AttendanceType, di.DictItemValue AS AttendanceTypeName, w.FaultFlag FROM WorkTime w LEFT JOIN Project p ON w.ProjectId = p.ProjectId INNER JOIN sys_DictionaryItem di ON w.AttendanceType = di.DictItemId WHERE YEAR(w.BeginTime) = @Year AND MONTH(w.BeginTime) = @Month AND w.EmployeeId = @EmployeeId ORDER BY w.BeginTime");
                dba.AddInParameter(cmd, "Year", DbType.Int32, (object)year);
                dba.AddInParameter(cmd, "Month", DbType.Int32, (object)month);
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);

                List<WorkTimeModel> workTimes = dba.ExecuteReader<List<WorkTimeModel>>(
                    cmd, (d, r) => 
                    {
                        d.Add(new WorkTimeModel
                            {
                                WorkTimeId = (Guid)r["WorkTimeId"],
                                BeginTime = Convert.ToDateTime(r["BeginTime"]),
                                EndTime = Convert.ToDateTime(r["EndTime"]),
                                ProjectId = r.GetGuidOrNull("ProjectId"),
                                ProjectName = r.GetStringOrNull("ProjectName"),
                                AttendanceType = r.GetStringOrNull("AttendanceType"),
                                AttendanceTypeName = r.GetStringOrNull("AttendanceTypeName"),
                                FaultFlag = r.GetBooleanOrDefault("FaultFlag")
                            });
                    });
                return workTimes;
            }
        }

        public List<WorkTimeModel> GetWorkTime(DateTime begin, DateTime end, Guid employeeId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT w.*, p.ProjectName, s.StageName, a.ActivityText, di.DictItemValue AS AttendanceTypeName, ue.RealName AS ApprovedManagerName FROM WorkTime w LEFT JOIN Project p ON w.ProjectId = p.ProjectId LEFT JOIN Stage s ON w.StageId = s.StageId LEFT JOIN Activity a ON w.ActivityId = a.ActivityId LEFT JOIN auth_UserExtend ue ON w.ApprovedManager = ue.UserId INNER JOIN sys_DictionaryItem di ON w.AttendanceType = di.DictItemId WHERE w.BeginTime >= @BeginTime AND w.EndTime <= @EndTime AND w.EmployeeId = @EmployeeId");
                dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, QueryParameterUtility.DateTimeStart(begin));
                dba.AddInParameter(cmd, "EndTime", DbType.DateTime, QueryParameterUtility.DateTimeEnd(end));
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);

                List<WorkTimeModel> workTimes = dba.ExecuteReader<List<WorkTimeModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new WorkTimeModel
                        {
                            WorkTimeId = (Guid)r["WorkTimeId"],
                            BeginTime = Convert.ToDateTime(r["BeginTime"]),
                            EndTime = Convert.ToDateTime(r["EndTime"]),
                            ProjectId = r.GetGuidOrNull("ProjectId"),
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            Hour = r.GetDecimalOrDefault("Hours", 0),
                            StageId = r.GetGuidOrNull("StageId"),
                            StageName = r.GetStringOrNull("StageName"),
                            ActivityId = r.GetGuidOrNull("ActivityId"),
                            ActivityName = r.GetStringOrNull("ActivityText"),
                            JobContent = r.GetStringOrNull("JobContent"),
                            AttendanceType = r.GetStringOrNull("AttendanceType"),
                            AttendanceTypeName = r.GetStringOrNull("AttendanceTypeName"),
                            Approved = r.GetBooleanOrDefault("Approved", false),
                            ApprovedTime = r.GetDateTimeOrDefault("ApprovedTime"),
                            ApprovedManager = r.GetGuidOrNull("ApprovedManager"),
                            ApprovedManagerName = r.GetStringOrNull("ApprovedManagerName"),
                            FaultFlag = r.GetBooleanOrDefault("FaultFlag"),
                            FaultReason = r.GetStringOrNull("FaultReason"),
                            WorkTimeSettingId = r.GetGuidOrNull("WorkTimeSettingId")
                        });
                    });
                return workTimes;
            }
        }

        public bool InsertWorkTime(WorkTime workTime, WorkTimeSetting workTimeSetting)
        {
            Guard.ArgumentNotNull(workTime, "workTime");
            Guard.ArgumentNotNullOrEmpty(workTime.JobContent, "workTime.JobContent");
            Guard.ArgumentNotNull(workTime.BeginTime, "workTime.BeginTime");
            Guard.ArgumentNotNull(workTime.EndTime, "workTime.EndTime");

            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                using(var conn = dba.GetOpenConnection())
                {
                    workTime.WorkTimeId = Guid.NewGuid();
                    PrepareWorkTime(workTime, workTimeSetting, context);
                    context.WorkTimes.Add(workTime);
                    int affectRows = context.SaveChanges();
                    return affectRows > 0;
                }
            }
        }

        public bool UpdateWorkTime(WorkTime workTime, ref bool isApproved)
        {
            Guard.ArgumentNotNull(workTime, "workTime");
            Guard.ArgumentNotNullOrEmpty(workTime.JobContent, "workTime.JobContent");
            Guard.ArgumentNotNull(workTime.BeginTime, "workTime.BeginTime");
            Guard.ArgumentNotNull(workTime.EndTime, "workTime.EndTime");
            
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                using(var conn = dba.GetOpenConnection())
                {
                    WorkTime old = context.WorkTimes
                        .Where(w => w.WorkTimeId == workTime.WorkTimeId)
                        .Single();
                    if(old.Approved == true) 
                    {
                        isApproved = true;
                        return false;
                    }
                    if(workTime.WorkTimeSettingId == null)
                    {
                        workTime.WorkTimeSettingId = old.WorkTimeSettingId;
                    }
                    else
                    {
                        old.WorkTimeSettingId = workTime.WorkTimeSettingId;
                    }
                    PrepareWorkTime(workTime, null, context);

                    old.EmployeeId = workTime.EmployeeId;
                    old.BeginTime = workTime.BeginTime;
                    old.EndTime = workTime.EndTime;
                    old.Hour = workTime.Hour;
                    old.ProjectId = workTime.ProjectId;
                    old.StageId = workTime.StageId;
                    old.ActivityId = workTime.ActivityId;
                    old.JobContent = workTime.JobContent;
                    old.AttendanceType = workTime.AttendanceType;
                    old.FaultFlag = workTime.FaultFlag;
                    old.FaultReason = workTime.FaultReason;
                    old.Approved = workTime.Approved;
                    old.ApprovedManager = workTime.ApprovedManager;
                    old.ApprovedTime = workTime.ApprovedTime;

                    int affectRows = context.SaveChanges();
                    return affectRows > 0;
                }
            }
        }

        void PrepareWorkTime(WorkTime workTime, WorkTimeSetting workTimeSetting, TeamDBContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            //如果是请假则不用关联项目信息
            if (workTime.AttendanceType == "3-02" && workTime.ProjectId != null)
            {
                workTime.ProjectId = null;
                workTime.StageId = null;
                workTime.ActivityId = null;
            }
            DateTime beginTime = workTime.BeginTime.Value;
            DateTime endTime = workTime.EndTime.Value;
            TimeSpan interval = endTime - beginTime;
            double hours = interval.Hours + (interval.Minutes / 60.0);

            //如果是正常上班或者请假则需要去除午休时间，外勤和出差则按全部时间记录
            if (workTime.AttendanceType == "3-01" || workTime.AttendanceType == "3-02")
            {
                if(workTimeSetting == null)
                {
                    if(workTime.WorkTimeSettingId == null)
                    {
                        workTimeSetting = context.WorkTimeSettings
                            .Where(wts => wts.Enabled == true)
                            .SingleOrDefault();
                    }
                    else
                    {
                        workTimeSetting = context.WorkTimeSettings
                            .Where(wts => wts.WorkTimeSettingId == workTime.WorkTimeSettingId)
                            .SingleOrDefault();
                    }
                    if (workTimeSetting == null)
                    {
                        throw new ModuleLogicException("没能获取到可用的工作时间设置信息");
                    }
                }

                DateTime amBeginTime = beginTime
                    .AddHours(workTimeSetting.AMStartTime.Value.Hours - beginTime.Hour)
                    .AddMinutes(workTimeSetting.AMStartTime.Value.Minutes - beginTime.Minute)
                    .AddSeconds(workTimeSetting.AMStartTime.Value.Seconds - beginTime.Second);
                DateTime amEndTime = beginTime
                    .AddHours(workTimeSetting.AMEndTime.Value.Hours - beginTime.Hour)
                    .AddMinutes(workTimeSetting.AMEndTime.Value.Minutes - beginTime.Minute)
                    .AddSeconds(workTimeSetting.AMEndTime.Value.Seconds - beginTime.Second);
                DateTime pmBeginTime = beginTime
                    .AddHours(workTimeSetting.PMStartTime.Value.Hours - beginTime.Hour)
                    .AddMinutes(workTimeSetting.PMStartTime.Value.Minutes - beginTime.Minute)
                    .AddSeconds(workTimeSetting.PMStartTime.Value.Seconds - beginTime.Second);
                DateTime pmEndTime = beginTime
                    .AddHours(workTimeSetting.PMEndTime.Value.Hours - beginTime.Hour)
                    .AddMinutes(workTimeSetting.PMEndTime.Value.Minutes - beginTime.Minute)
                    .AddSeconds(workTimeSetting.PMEndTime.Value.Seconds - beginTime.Second);

                if (beginTime > amEndTime && beginTime < pmBeginTime)
                    beginTime = amEndTime;
                if (endTime > amEndTime && endTime < pmBeginTime)
                    endTime = pmBeginTime;
                interval = endTime - beginTime;
                hours = interval.TotalHours;

                if (beginTime <= amEndTime && endTime >= pmBeginTime)
                {
                    interval = pmBeginTime - amEndTime;
                    double noonTime = interval.Hours + (interval.Minutes / 60.0);
                    hours -= noonTime;
                    if (hours <= 0)
                    {
                        throw new ModuleLogicException("在正常上班或请假时午休时间不能登记工时");
                    }
                }
                workTime.WorkTimeSettingId = workTimeSetting.WorkTimeSettingId;
            }
            workTime.Hour = Convert.ToDecimal(hours);
        }

        /// <summary>
        /// 工时确认
        /// </summary>
        /// <param name="workTimeArray">工时ID array</param>
        /// <param name="approved">是否确认</param>
        /// <param name="approvedManager">确认人ID</param>
        /// <param name="faultFlag">是否工时填报错误</param>
        /// <param name="faultReason">工时填报错误原因</param>
        /// <returns></returns>
        public bool WorkTimeApproved(Guid[] workTimeArray, bool? approved, Guid approvedManager, bool? faultFlag, string faultReason)
        {
            Guard.ArgumentNotNull(workTimeArray, "workTimeArray");
            if (faultFlag != null && faultFlag.Value)
            {
                approved = false;
                if(String.IsNullOrEmpty(faultReason))
                {
                    throw new ModuleLogicException("没有填写工时错误原因");
                }
            }
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("UPDATE [WorkTime] SET Approved = @p1, ApprovedTime = @p2, ApprovedManager = @p3, FaultFlag = @p4, FaultReason = @p5 WHERE WorkTimeId = @p0");
                IList<object[]> parameters = QueryParameterUtility.TransferMultipleParameters<Guid>(
                    workTimeArray, new object[] { approved, DateTime.Now, approvedManager, faultFlag, faultReason });
                int affectRows = dba.ExecuteNonQueryMultiple(cmd, parameters);
                return affectRows == workTimeArray.Length;
            }

        }

        public bool DeleteWorkTime(Guid workTimeId, ref bool isApproved)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                WorkTime old = context.WorkTimes
                    .Where(w => w.WorkTimeId == workTimeId)
                    .Single();
                if(old.Approved == true)
                {
                    isApproved = true;
                    return false;
                }
                context.WorkTimes.Remove(old);
                int affectRows = context.SaveChanges();
                return affectRows > 0;
            }
        }

        public bool DeleteWorkTimes(Guid[] workTimeIdArray)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("DELETE FROM [WorkTime] WHERE WorkTimeId = @p0");
                IList<object[]> parameters = QueryParameterUtility.TransferMultipleParameters<Guid>(workTimeIdArray);
                int affectRows = dba.ExecuteNonQueryMultiple(cmd, parameters);
                return affectRows > 0;
            }
        }

        public List<WorkTimeModel> GetWorkTimes(Guid? projectId, Guid? stageId, Guid? employeeId, DateTime? beginTime, DateTime? endTime, string attendTypes, int pageIndex, int pageSize, out int rowCount, bool getAll = false, Guid? pmId = null)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT w.*, dict.DictItemValue AttendanceTypeName, u.RealName EmployeeName, p.ProjectName, s.StageName FROM WorkTime w");
                sqlBuilder.Append(" LEFT JOIN sys_DictionaryItem dict ON dict.DictItemId = w.AttendanceType")
                    .Append(" INNER JOIN auth_UserExtend u ON u.UserId = w.EmployeeId")
                    .Append(" LEFT JOIN Project p ON p.ProjectId = w.ProjectId")
                    .Append(" LEFT JOIN Stage s ON s.StageId = w.StageId");
                if (pmId != null)
                {
                    sqlBuilder.Append(" INNER JOIN (SELECT DISTINCT s.StageId FROM Project p INNER JOIN Stage s ON p.ProjectId = s.ProjectId WHERE s.Manager = @PM) t ON w.StageId = t.StageId");
                    dba.AddInParameter(cmd, "PM", DbType.Guid, pmId);
                }
                sqlBuilder.Append(" WHERE 1 = 1");
                if (!getAll)
                {
                    sqlBuilder.Append(" AND (w.Approved <> 1 OR w.Approved IS NULL) AND (w.FaultFlag IS NULL OR w.FaultFlag <> 1)");
                }
                if (projectId != null)
                {
                    sqlBuilder.Append(" AND w.ProjectId = @ProjectId");
                    dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                }
                if (stageId != null)
                {
                    sqlBuilder.Append(" AND w.StageId = @StageId");
                    dba.AddInParameter(cmd, "StageId", DbType.Guid, stageId);
                }
                if (employeeId != null)
                {
                    sqlBuilder.Append(" AND w.EmployeeId = @EmployeeId");
                    dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
                }
                if (beginTime != null)
                {
                    sqlBuilder.Append(" AND w.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime,
                        QueryParameterUtility.DateTimeStart(beginTime.Value));
                }
                if (endTime != null)
                {
                    sqlBuilder.Append(" AND w.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime,
                        QueryParameterUtility.DateTimeEnd(endTime.Value));
                }
                if(!String.IsNullOrEmpty(attendTypes))
                {
                    sqlBuilder.Append(" AND w.AttendanceType = @AttendanceType");
                    dba.AddInParameter(cmd, "AttendanceType", DbType.String, attendTypes);
                }
                sqlBuilder.Append(" ORDER BY w.BeginTime DESC");
                cmd.CommandText = sqlBuilder.ToString();
                List<WorkTimeModel> data = dba.ExecuteReaderPaging<List<WorkTimeModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new WorkTimeModel
                        {
                            WorkTimeId = (Guid)r["WorkTimeId"],
                            EmployeeName = r.GetStringOrNull("EmployeeName"),
                            AttendanceTypeName = r.GetStringOrNull("AttendanceTypeName"),
                            BeginTime = Convert.ToDateTime(r["BeginTime"]),
                            EndTime = Convert.ToDateTime(r["EndTime"]),
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            Hour = r.GetDecimalOrDefault("Hours", 0),
                            StageName = r.GetStringOrNull("StageName"),
                            Approved = r.GetBooleanOrDefault("Approved", false),
                            JobContent = r.GetStringOrNull("JobContent")
                        });
                    }, pageIndex, pageSize, out rowCount);
                return data;
            }
        }

        public decimal GetWorkHours()
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                var result = context.WorkTimeSettings.Where(w => w.Enabled == true).Single();
                return result.WorkHour.Value;
            }
        }

    }
}
