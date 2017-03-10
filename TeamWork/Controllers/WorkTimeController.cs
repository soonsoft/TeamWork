using STIM.Data.EntityFrameworkExtensions;
using STIM.Web;
using STIM.Web.Security;
using STIM.Web.MobileSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamWork.BLL;
using TeamWork.BLL.Models;
using TeamWork.DAL;

namespace TeamWork.Controllers
{
    public class WorkTimeController : Controller
    {
        //
        // GET: /WorkTime/
        [Mobile]
        public ActionResult Record()
        {
            DictionaryLogic dlogic = new DictionaryLogic();
            ViewBag.AttendanceTypes = new SelectList(
                dlogic.GetEnabledDictionaryItems(DictionaryLogic.AttendanceType), DictionaryLogic.ValueField, DictionaryLogic.TextField);

            ProjectLogic plogic = new ProjectLogic();
            var projects = plogic.GetProjectStage(UserContext.CurrentUser.UserId);
            ViewBag.Projects = projects;
            ViewBag.ProjectSelectList = new SelectList(
                projects, "ProjectId", "ProjectName");

            SettingsLogic slogic = new SettingsLogic();
            ViewBag.WorkTimeSetting = slogic.GetEnabledWorkTimeSetting();

            return View();
        }

        public ActionResult GetAllProjects()
        {
            ProjectLogic plogic = new ProjectLogic();
            var projects = plogic.GetProjectStage();
            return Json(projects);
        }

        public ActionResult LoadWorkTimeByYear(int year)
        {
            Guid employeeId = UserContext.CurrentUser.UserId;
            WorkTimeLogic wtLogic = new WorkTimeLogic();
            var data = wtLogic.GetWorkDateByYear(employeeId, year);
            return Json(data);
        }

        public ActionResult LoadWorkTimeByMonth(int year, int month)
        {
            Guid employeeId = UserContext.CurrentUser.UserId;
            WorkTimeLogic wtLogic = new WorkTimeLogic();
            var data = wtLogic.GetWorkTimeByMonth(employeeId, year, month);
            return Json(data);
        }

        public ActionResult LoadWorkTimeByWeek(DateTime beginTime, DateTime endTime)
        {
            WorkTimeLogic wtLogic = new WorkTimeLogic();
            var data = wtLogic.GetWorkTime(beginTime, endTime, UserContext.CurrentUser.UserId);
            return Json(data);
        }

        public ActionResult InsertRecord(WorkTime workTime, WorkTimeSetting workTimeSetting)
        {
            workTime.EmployeeId = UserContext.CurrentUser.UserId;
            WorkTimeLogic wtLogic = new WorkTimeLogic();
            var result = wtLogic.InsertWorkTime(workTime, workTimeSetting);
            return Json(new { Result = result, Data = workTime });
        }

        public ActionResult UpdateRecord(WorkTime workTime)
        {
            workTime.EmployeeId = UserContext.CurrentUser.UserId;
            WorkTimeLogic wtLogic = new WorkTimeLogic();
            bool isApproved = false;
            var result = wtLogic.UpdateWorkTime(workTime, ref isApproved);
            return Json(new { Result = result, Data = workTime, IsApproved = isApproved });
        }

        public ActionResult DeleteRecord(Guid workTimeId)
        {
            WorkTimeLogic wtLogic = new WorkTimeLogic();
            bool isApproved = false;
            var result = wtLogic.DeleteWorkTime(workTimeId, ref isApproved);
            return Json(new { Result = result, IsApproved = isApproved });
        }

        public ActionResult RecordManage()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();
            Guid pmId = UserContext.CurrentUser.UserId;

            List<ProjectSelectItem> pList;
            dictLogic.ProjectDictionary(out pList, pmId);
            ViewBag.Projects = new DropDownList(pList, "ProjectId", "ProjectName", true); 

            List<EmployeeSelectItem> eList;
            dictLogic.EmployeeDictionary(out eList, pmId);
            ViewBag.Employees = new DropDownList(eList, "EmployeeId", "RealName", true);

            ViewBag.AttendanceTypes = dictLogic.GetDictionaryItems(DictionaryLogic.AttendanceType);

            return View();
        }

        public ActionResult GetUnConfirmWorkTimes(Guid? projectId, Guid? stageId, Guid? employeeId, DateTime? beginTime, DateTime? endTime, string attendTypes, int pageIndex, int pageSize)
        {
            WorkTimeLogic logic = new WorkTimeLogic();
            int rowCount;
            Guid? pmId = UserContext.CurrentUser.UserId;
            List<WorkTimeModel> data = logic.GetWorkTimes(projectId, stageId, employeeId, beginTime, endTime, attendTypes, pageIndex, pageSize, out rowCount, false, pmId);
            return Json(new { Result = data, RowCount = rowCount });
        }

        public ActionResult WorkTimeApproved(Guid[] workTimeArray, bool? approved, bool? faultFlag, string faultReason)
        {
            WorkTimeLogic logic = new WorkTimeLogic();
            var result = logic.WorkTimeApproved(workTimeArray, approved, UserContext.CurrentUser.UserId, faultFlag, faultReason);
            return Json(new { Result = result });
        }

        public ActionResult GetHolidayByYear(int year)
        {
            SettingsLogic sLogic = new SettingsLogic();
            var data = sLogic.GetHolidaySetting(year);
            return Json(data);
        }

        #region QueryRecord

        public ActionResult QueryRecord()
        {
            ProjectLogic plogic = new ProjectLogic();
            ViewBag.Projects = plogic.GetProjectDict();
            ViewBag.Stages = plogic.GetProjectStageDict();

            EmployeeLogic elogic = new EmployeeLogic();
            ViewBag.Employees = elogic.GetAllEmployees(false);

            return View();
        }

        public ActionResult GetStagesByProgectId(Guid projectId)
        {
            ProjectLogic plogic = new ProjectLogic();
            var result = plogic.FindStageByProjectId(projectId);
            return Json(result);
        }

        public ActionResult GetWorkTime(Guid? projectId, Guid? stageId, Guid? employeeId, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            WorkTimeLogic logic = new WorkTimeLogic();
            int rowCount = 0;
            var result = logic.GetWorkTimes(projectId, stageId, employeeId, startTime, endTime, null, pageIndex, pageSize, out rowCount, true);
            return Json(new { Result = result, RowCount = rowCount });
        }

        #endregion
	}
}