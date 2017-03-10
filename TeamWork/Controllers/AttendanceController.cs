using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamWork.BLL;

namespace TeamWork.Controllers
{
    public class AttendanceController : Controller
    {
        // GET: Attended 出勤情况查询
        public ActionResult Attended()
        {
            SettingsLogic slogic = new SettingsLogic();
            ViewBag.WorkTimeSetting = slogic.GetEnabledWorkTimeSetting();

            EmployeeLogic empLogic = new EmployeeLogic();
            ViewBag.Employees = empLogic.GetAllEmployees(false);

            IUserData user = UserContext.CurrentUser;
            ViewBag.EmployeeId = user.UserId.ToString();
            return View();
        }

        public ActionResult LoadAttendanceInfo(Guid? userId, DateTime beginTime, DateTime endTime)
        {
            AttendanceLogic al = new AttendanceLogic();
            if(userId == null)
            {
                userId = UserContext.CurrentUser.UserId;
            }
            var data = al.QueryAttendance(userId.Value, beginTime, endTime);
            return Json(data);
        }

        public ActionResult GetHoliday(DateTime begin, DateTime end)
        {
            SettingsLogic sLogic = new SettingsLogic();
            var data = sLogic.GetHolidaySetting(begin, end);
            return Json(data);
        }
    }
}