using STIM.Core;
using STIM.Data.EntityFrameworkExtensions;
using STIM.Modules;
using STIM.Web.Security;
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
    public class RequestLeaveLogic
    {
        public List<LeaveBillModel> QueryLeaveBills(string employeeID, DateTime? beginTime, DateTime? endTime, int pageIndex, int pageSize, out int rowCount)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT b.*, di.DictItemValue AS LeaveTypeName, ue.RealName AS EmployeeName, ue.DepartmentName FROM RequestLeaveBill b INNER JOIN sys_DictionaryItem di ON b.LeaveType = di.DictItemId INNER JOIN auth_UserExtend ue ON b.EmployeeId = ue.UserId WHERE 1 = 1");
                if(!String.IsNullOrEmpty(employeeID))
                {
                    sqlBuilder.Append(" AND b.EmployeeId = @EmployeeId");
                    dba.AddInParameter(cmd, "EmployeeId", DbType.String, employeeID);
                }
                if(beginTime != null)
                {
                    sqlBuilder.Append(" AND b.LeaveStart >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, QueryParameterUtility.DateTimeStart(beginTime.Value));
                }
                if(endTime != null)
                {
                    sqlBuilder.Append(" AND b.LeaveStart <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, QueryParameterUtility.DateTimeStart(endTime.Value));
                }
                sqlBuilder.Append(" ORDER BY b.CreateTime DESC");

                cmd.CommandText = sqlBuilder.ToString();

                List<LeaveBillModel> bills = dba.ExecuteReaderPaging<List<LeaveBillModel>>(cmd, (d, r) => 
                {
                    d.Add(ReadLeaveBill(r));
                }, pageIndex, pageSize, out rowCount);

                return bills;
            }
        }

        public bool InsertRequestLeaveBill(RequestLeaveBill bill, IUserData user, bool amStart, bool amEnd)
        {
            Guard.ArgumentNotNull(bill, "bill");

            bill.Id = Guid.NewGuid();
            using (TeamDBContext context = new TeamDBContext())
            {
                bill.WorkDayCount = CalculatetWorkdayCount(context, bill, amStart, amEnd);
                if(bill.WorkDayCount == 0)
                {
                    throw new ModuleLogicException("请假的工作日为0天，请检查请假的开始结束时间是否填写正确");
                }
                bill.CreateTime = DateTime.Now;
                bill.Employee = context.Employees.Where(p => p.EmployeeId == user.UserId).Single();
                
                context.RequestLeaveBills.Add(bill);
                int affectRows = context.SaveChanges();
                return affectRows > 0;
            }
        }


        public bool UpdateRequestLeaveBill(RequestLeaveBill bill, IUserData user, bool amStart, bool amEnd)
        {
            Guard.ArgumentNotNull(bill, "bill");

            using (TeamDBContext context = new TeamDBContext())
            {
                bill.WorkDayCount = CalculatetWorkdayCount(context, bill, amStart, amEnd);
                if (bill.WorkDayCount == 0)
                {
                    throw new ModuleLogicException("请假的工作日为0天，请检查请假的开始结束时间是否填写正确");
                }

                RequestLeaveBill old = context.RequestLeaveBills
                    .Where(p => p.Id == bill.Id)
                    .Single();

                old.LeaveStart = bill.LeaveStart;
                old.LeaveEnd = bill.LeaveEnd;
                old.LeaveType = bill.LeaveType;
                old.WorkDayCount = bill.WorkDayCount;
                old.LeaveReason = bill.LeaveReason;
                old.PMSign = bill.PMSign;
                old.PMSignDate = bill.PMSignDate;
                old.DLSign = bill.DLSign;
                old.DLSignDate = bill.DLSignDate;
                old.CEOSign = bill.CEOSign;
                old.CEOSignDate = bill.CEOSignDate;

                return context.SaveChanges() > 0;
            }
        }

        public bool DeleteRequestLeaveBillById(Guid id)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                int effectRows = dba.ExecuteNonQuery(
                    "DELETE FROM [RequestLeaveBill] WHERE Id = @p0", 
                    new object[] { id });
                return effectRows > 0;
            }
        }

        public List<LeaveBillModel> GetLeaveBillsByIdArray(string[] idArray)
        {
            Guard.ArgumentNotNull(idArray, "billIdList");
            if(idArray.Length > 100)
            {
                throw new ModuleLogicException("一次最多可以获取100个请假单");
            }

            List<LeaveBillModel> result = new List<LeaveBillModel>(idArray.Length);
            if(idArray.Length == 0)
            {
                return result;
            }

            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT b.*, di.DictItemValue AS LeaveTypeName, ue.RealName AS EmployeeName, ue.DepartmentName FROM RequestLeaveBill b INNER JOIN sys_DictionaryItem di ON b.LeaveType = di.DictItemId INNER JOIN auth_UserExtend ue ON b.EmployeeId = ue.UserId WHERE b.Id = @BillId");
                dba.AddInParameter(cmd, "BillId", DbType.Guid, null);
                using (var conn = dba.GetOpenConnection())
                {
                    foreach (string id in idArray)
                    {
                        if (!String.IsNullOrWhiteSpace(id))
                        {
                            cmd.Parameters[0].Value = Guid.Parse(id.Trim());
                            using (DbDataReader reader = dba.ExecuteReader(cmd))
                            {
                                if(reader.Read())
                                {
                                    result.Add(ReadLeaveBill(reader));
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private decimal CalculatetWorkdayCount(TeamDBContext context, RequestLeaveBill bill, bool amStart, bool amEnd)
        {
            //获取假期
            DateTime startTime = QueryParameterUtility.DateTimeStart(bill.LeaveStart);
            DateTime endTime = QueryParameterUtility.DateTimeEnd(bill.LeaveEnd);
            DatabaseAccess dba = context.DatabaseAccess();
            HashSet<string> holidays = dba.ExecuteReader<HashSet<string>>(
                "SELECT CONVERT(varchar, HolidayDate, 120) AS HolidayDate FROM HolidaySettings WHERE HolidayDate >= @p0 AND HolidayDate <= @p1",
                new object[] { startTime, endTime },
                (d, r) => {
                    d.Add(r.GetString(0));
                }
            );
            //获取工作时间
            WorkTimeSetting setting = context.WorkTimeSettings
                    .Where(s => s.Enabled)
                    .Single();

            //设置时间
            if (amStart)
                bill.LeaveStart = SetTime(bill.LeaveStart, setting.AMStartTime.Value);
            else
                bill.LeaveStart = SetTime(bill.LeaveStart, setting.PMStartTime.Value);
            if (amEnd)
                bill.LeaveEnd = SetTime(bill.LeaveEnd, setting.AMEndTime.Value);
            else
                bill.LeaveEnd = SetTime(bill.LeaveEnd, setting.PMEndTime.Value);

            decimal workDayCount = 0;
            TimeSpan time = bill.LeaveEnd - bill.LeaveStart;
            string formatter = "yyyy-MM-dd";
            if (time.Days == 0)
            {
                if(!holidays.Contains(bill.LeaveStart.ToString(formatter)))
                {
                    //当天请假，只计算小时
                    workDayCount = CalculatetByDay(bill.LeaveStart, bill.LeaveEnd, setting);
                }
            }
            else if(time.Days >= 1)
            {
                if (time.Days > 1)
                {
                    //跨多天请假，中间按自然天，头尾按小时
                    //计算中间的工作日（除去请假开始的那天和请假结束的那天）
                    DateTime middleStart = QueryParameterUtility.DateTimeStart(bill.LeaveStart.AddDays(1));
                    DateTime middleEnd = QueryParameterUtility.DateTimeStart(bill.LeaveEnd);
                    if (middleStart < middleEnd)
                    {
                        middleEnd = middleEnd.AddDays(-1);
                    }
                    int middleDays = (middleEnd - middleStart).Days + 1;
                    //移除休息日
                    for (int i = 0; i < middleDays; i++)
                    {
                        if (!holidays.Contains(middleStart.AddDays(i).ToString(formatter)))
                        {
                            workDayCount++;
                        }
                    }
                }

                //跨天请假，分别计算小时
                DateTime begin, end;
                if (!holidays.Contains(bill.LeaveStart.ToString(formatter)))
                {
                    begin = bill.LeaveStart;
                    end = SetTime(bill.LeaveStart, setting.PMEndTime.Value);
                    workDayCount += CalculatetByDay(begin, end, setting);
                }

                if(!holidays.Contains(bill.LeaveEnd.ToString(formatter)))
                {
                    begin = SetTime(bill.LeaveEnd, setting.AMStartTime.Value);
                    end = bill.LeaveEnd;
                    workDayCount += CalculatetByDay(begin, end, setting);
                }
            }

            return workDayCount;
        }

        /// <summary>
        /// 只计算当天的工作日
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        private decimal CalculatetByDay(DateTime begin, DateTime end, WorkTimeSetting setting)
        {
            TimeSpan interval = begin - end;
            if(interval.Days > 0)
            {
                throw new ArgumentException("start和end都必需在同一天");
            }

            DateTime amBegin = SetTime(begin, setting.AMStartTime.Value);
            DateTime amEnd = SetTime(begin, setting.AMEndTime.Value);
            DateTime pmBegin = SetTime(end, setting.PMStartTime.Value);
            DateTime pmEnd = SetTime(end, setting.PMEndTime.Value);

            if(begin < amBegin)
                begin = amBegin;
            if (end > pmEnd)
                end = pmEnd;

            decimal dayCount = 0;
            decimal halfDay = new decimal(0.5);
            if (begin < amEnd)
            {
                dayCount += halfDay;
            }
            if(end > pmBegin)
            {
                dayCount += halfDay;
            }

            /* 精确的小时算法，暂时不用
            //校准时间，用于午休时间的修正
            if (begin > amEnd && begin < pmBegin)
                begin = amEnd;
            if (end > amEnd && end < pmBegin)
                end = pmBegin;
            interval = end - begin;
            double hours = interval.Hours + (interval.Minutes / 60.0);

            //如果时间中包含午休时间则除去午休时间
            if(begin <= amEnd && end >= pmBegin)
            {
                interval = pmBegin - amEnd;
                hours -= interval.Hours + (interval.Minutes / 60.0);
            }
            decimal dayCount;
            if(hours <= 0)
            {
                dayCount = 0;
            }
            else
            {
                interval = amEnd - amBegin;
                double totalHours = interval.Hours + (interval.Minutes / 60.0);
                interval = pmEnd - pmBegin;
                totalHours += interval.Hours + (interval.Minutes / 60.0);
                dayCount = Convert.ToDecimal(hours / totalHours);
            }
            */
            return dayCount;
        }

        private DateTime SetTime(DateTime date, TimeSpan time)
        {
            return date.AddHours(time.Hours - date.Hour)
                    .AddMinutes(time.Minutes - date.Minute)
                    .AddSeconds(time.Seconds - date.Second);
        }

        private LeaveBillModel ReadLeaveBill(DbDataReader r)
        {
            return new LeaveBillModel
            {
                Id = (Guid)r["Id"],
                EmployeeID = r.GetStringOrNull("EmployeeID"),
                EmployeeName = r.GetStringOrNull("EmployeeName"),
                DepartmentName = r.GetStringOrNull("DepartmentName"),
                LeaveType = r.GetStringOrNull("LeaveType"),
                LeaveTypeName = r.GetStringOrNull("LeaveTypeName"),
                LeaveStart = r.GetDateTimeOrDefault("LeaveStart").Value,
                LeaveEnd = r.GetDateTimeOrDefault("LeaveEnd").Value,
                WorkDayCount = r.GetDecimalOrDefault("WorkDayCount"),
                LeaveReason = r.GetStringOrNull("LeaveReason"),
                PMSign = r.GetGuidOrNull("PMSign"),
                PMSignDate = r.GetDateTimeOrDefault("PMSignDate"),
                DLSign = r.GetGuidOrNull("DLSign"),
                DLSignDate = r.GetDateTimeOrDefault("DLSignDate"),
                CEOSign = r.GetGuidOrNull("CEOSign"),
                CEOSignDate = r.GetDateTimeOrDefault("CEOSignDate"),
                CreateTime = r.GetDateTimeOrDefault("CreateTime").Value
            };
        }
    }
}
