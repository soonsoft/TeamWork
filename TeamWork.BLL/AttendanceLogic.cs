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
    public class AttendanceLogic
    {
        public List<AttendanceInfoModel> QueryAttendance(Guid userId, DateTime begin, DateTime end)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT a.* FROM AttendanceUsers u INNER JOIN AttendanceInfo a ON u.OutsideUserId = a.OutsideUserId WHERE u.UserId = @UserId AND a.Clock >= @BeginTime AND a.Clock <= @EndTime ORDER BY a.Clock ASC");

                dba.AddInParameter(cmd, "UserId", DbType.Guid, userId);
                dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, QueryParameterUtility.DateTimeStart(begin));
                dba.AddInParameter(cmd, "EndTime", DbType.DateTime, QueryParameterUtility.DateTimeEnd(end));

                List<AttendanceInfoModel> data = dba.ExecuteReader<List<AttendanceInfoModel>>(
                    cmd, (d, r) => 
                    {
                        d.Add(new AttendanceInfoModel
                        {
                            AttendanceInfoId = (Guid)r["AttendanceInfoId"],
                            DeviceId = r.GetStringOrNull("DeviceId"),
                            OutsideUserId = r.GetStringOrNull("OutsideUserId"),
                            Clock = r.GetDateTimeOrDefault("Clock").Value,
                            Remark = r.GetStringOrNull("Remark"),
                            UpdateTime = r.GetDateTimeOrDefault("UpdateTime").Value
                        });
                    });
                return data;
            }
        }
    }
}
