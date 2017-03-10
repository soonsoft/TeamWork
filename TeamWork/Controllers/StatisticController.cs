using STIM.Core;
using STIM.Modules.Dictionary;
using STIM.Web;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamWork.BLL;
using TeamWork.BLL.Models;
using TeamWork.BLL.SecurityExtends;
using TeamWork.DAL;

namespace TeamWork.Controllers
{
    public class StatisticController : Controller
    {
        #region 工时登记统计

        public ActionResult RecordStatistic()
        {
            EmployeeLogic elogic = new EmployeeLogic();
            ViewBag.Employees = elogic.GetAllEmployees(false);
            WorkTimeLogic wlogic = new WorkTimeLogic();
            ViewBag.WorkHours = wlogic.GetWorkHours();
            return View();
        }

        public ActionResult GetRecordByMonth(DateTime? start)
        {
            StatisticLogic slogic = new StatisticLogic();
            DateTime now = DateTime.Now;
            DateTime startTime;
            DateTime endTime;
            DateTime current = start.Value;
            if (start == null)
            {
                startTime = now.AddDays(1 + (-now.Day)).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second);
                endTime = now.AddMonths(1).AddDays(-now.Day).AddHours(23 - now.Hour).AddMinutes(59 - now.Minute).AddSeconds(59 - now.Second);
            }
            else
            {
                startTime = current.AddDays(1 + (-current.Day)).AddHours(-current.Hour).AddMinutes(-current.Minute).AddSeconds(-current.Second);
                endTime = current.AddMonths(1).AddDays(-current.Day).AddHours(23 - current.Hour).AddMinutes(59 - current.Minute).AddSeconds(59 - current.Second);
            }
            
            var result = slogic.GetRecordByMonth(startTime, endTime, Guid.Empty);
            return Json(result);
        }

        #endregion

        #region 项目统计
        public ActionResult ProjectStatistic()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();
            var projectTypes = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.ProjectType);
            var projectStatus = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.ProjectStatus);

            StatisticLogic stsLogic = new StatisticLogic();
            ViewBag.TotalHour = stsLogic.GetTotalHour();

            //ForRole
            IUserData user = UserContext.CurrentUser;
            //资讯设计人员
            if (user.HasRole("Consultant"))
            {
                ViewBag.forRole = true;
                var newProjectTypes = new List<DictionaryItem>();
                foreach (DictionaryItem dict in projectTypes)
                {
                    //只能看商业项目和商业项目（转包）
                    if (dict.DictItemId == "101-04" || dict.DictItemId == "101-05")
                    {
                        newProjectTypes.Add(dict);
                    }
                }
                ViewBag.QueryProjectTypes = newProjectTypes;
            }
            else
            {
                ViewBag.forRole = false;
                ViewBag.QueryProjectTypes = projectTypes;
            }
            ViewBag.QueryProjectStatus = projectStatus;
            return View();
        }

        public ActionResult QueryProjects(string projectName, string projectType, string status, DateTime? beginTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            StatisticLogic logic = new StatisticLogic();
            int rowCount;
            List<SumHourModel> results = logic.QueryProjects(projectName, projectType, status, beginTime, endTime, pageIndex, pageSize, out rowCount);
            return Json(new { Result = results, RowCount = rowCount });
        }

        public ActionResult QuerySubProjects(Guid projectId)
        {
            StatisticLogic logic = new StatisticLogic();
            List<SumHourModel> results = logic.QuerySubProjects(projectId);
            return Json(new { Result = results });
        }

        public ActionResult GetProjectUnitValue(List<UnitValueParam> args)
        {
            StatisticLogic logic = new StatisticLogic();
            if (args == null || args.Count == 0)
            {
                return null;
            }
            int length = args.Count;
            UnitValueParam param = null;
            List<SumHourModel> results = new List<SumHourModel>();
            for (int i = 0; i < length; i++)
            {
                param = args[i];
                if (param.IsParent)
                {
                    List<Guid> subProjectIds = logic.GetSubProjectIdsById(param.ProjectId);
                    int len = subProjectIds.Count;
                    SumHourModel pshm = new SumHourModel();
                    pshm.ProjectName = param.ProjectName;
                    pshm.TotalHour = 0;
                    pshm.TotalValue = 0;
                    for (int j = 0; j < len; j++)
                    {
                        SumHourModel cshm = logic.GetTotalValueById(subProjectIds[j]);
                        if (cshm != null)
                        {
                            pshm.TotalHour += cshm.TotalHour;
                            pshm.TotalValue += cshm.TotalValue;
                        }
                    }
                    results.Add(pshm);
                }
                else
                {
                    SumHourModel shm = logic.GetTotalValueById(param.ProjectId);
                    if (shm == null)
                    {
                        shm = new SumHourModel();
                        shm.ProjectName = param.ProjectName;
                        shm.TotalHour = 0;
                        shm.TotalValue = 0;
                    }
                    results.Add(shm);
                }
            }
            return Json(new { Result = results });
        }

        //打开项目详情统计页面
        public ActionResult ProjectDetailStatistic(Guid projectId)
        {
            StatisticLogic logic = new StatisticLogic();
            ProjectModel ProjectInfo = logic.GetProjectDetailById(projectId);
            //return View(ProjectInfo.ToPublicAnonymousType());
            SumHourModel shm = logic.GetTotalValueById(projectId);
            if (shm == null)
            {
                ProjectInfo.TotalValue = 0;
            }
            else
            {
                ProjectInfo.TotalValue = shm.TotalValue;
            }
            ViewBag.ProjectInfo = ProjectInfo;

            //计算折线统计图表中的PloatPlones的值
            List<String> times = logic.GetStatisticTimes(projectId);
            ProjectTime pt = logic.GetProjectTime(projectId);

            String planBYM = pt.PBeginTime.Substring(0, 7);
            String planEYM = pt.PEndTime.Substring(0, 7);
            String planBD = pt.PBeginTime.Substring(8, 2);
            String planED = pt.PEndTime.Substring(8, 2);

            String realBYM = pt.RBeginTime.Substring(0, 7);
            String realEYM = pt.REndTime.Substring(0, 7);
            String realBD = pt.RBeginTime.Substring(8, 2);
            String realED = pt.REndTime.Substring(8, 2);

            int count = times.Count;
            for (int i = 0; i < count; i++)
            {
                if (times[i] == planBYM) {
                    ViewBag.PlanBegin = i + (double.Parse(planBD) - getMonthMiddleDay(planBYM)) / getMonthDays(planBYM);
                }
                if (times[i] == planEYM) {
                    ViewBag.PlanEnd = i + (double.Parse(planED) - getMonthMiddleDay(planEYM)) / getMonthDays(planEYM);
                }
                if (times[i] == realBYM)
                {
                    ViewBag.RealBegin = i + (double.Parse(realBD) - getMonthMiddleDay(realBYM)) / getMonthDays(realBYM);
                }
                if (times[i] == realEYM)
                {
                    //此种情况为 还未登记 或只登记了一天，在前端统计图渲染时，实际投入的开始时间和结束时间相同，时间区域不会显示，这里故意 +0.008，为了让时间区域显示
                    if (pt.RBeginTime == pt.REndTime)
                    {
                        ViewBag.RealEnd = i + (double.Parse(realED) - getMonthMiddleDay(realEYM)) / getMonthDays(realEYM) + 0.008;
                    }
                    else 
                    {
                        ViewBag.RealEnd = i + (double.Parse(realED) - getMonthMiddleDay(realEYM)) / getMonthDays(realEYM);
                    }
                    
                }
            }
            ViewBag.ProjectTime = pt;

            return View();
        }

        private int getMonthDays(String yearMonth)
        {
            int year = int.Parse(yearMonth.Substring(0, 4));
            int month = int.Parse(yearMonth.Substring(5, 2));
            int days = DateTime.DaysInMonth(year, month);
            return days;
        }

        private double getMonthMiddleDay(String yearMonth)
        {
            int year = int.Parse(yearMonth.Substring(0, 4));
            int month = int.Parse(yearMonth.Substring(5, 2));
            int days = DateTime.DaysInMonth(year, month);
            return (double)days / 2;
        }

        //按阶段统计项目
        public ActionResult GetProjectStageDetailById(Guid projectId)
        {
            StatisticLogic logic = new StatisticLogic();
            var result = logic.GetProjectStageDetailById(projectId);
            return Json(result);
        }

        //根据项目ID获取参与该项目的每个部门的总耗时
        public ActionResult GetProjectDeparemntSumHourById(Guid projectId)
        {
            StatisticLogic logic = new StatisticLogic();
            var result = logic.GetProjectDeparemntSumHourById(projectId);
            return Json(result);
        }

        //按部门统计项目
        public ActionResult GetProjectDepartmentDetailById(Guid projectId)
        {
            StatisticLogic logic = new StatisticLogic();
            var result = logic.GetProjectDepartmentDetailById(projectId);
            return Json(result);
        }

        //获取统计时间段
        public ActionResult GetStatisticTimes(Guid projectId)
        {
            StatisticLogic logic = new StatisticLogic();
            var result = logic.GetStatisticTimes(projectId);
            return Json(result);
        }

        //按成员统计项目
        public ActionResult GetProjectEmployeeDetailById(Guid projectId)
        {
            StatisticLogic logic = new StatisticLogic();
            var result = logic.GetProjectEmployeeDetailById(projectId);
            return Json(result);
        }

        #endregion

        #region 成员统计

        public ActionResult EmployeeStatistic()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();
            var employeeStatus = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.EmployeeStatus);
            ViewBag.QueryEmployeeStatus = new DropDownList(employeeStatus, DictionaryLogic.ValueField, DictionaryLogic.TextField, true);
            return View();
        }

        public ActionResult StatisticEmployee(string status, DateTime? beginTime, DateTime? endTime)
        {
            StatisticLogic logic = new StatisticLogic();
            List<List<EmployeeStatic>> chartList = logic.StatisticEmployeeForChart(status, beginTime, endTime);
            List<EmployeeStatic> gridList = logic.StatisticEmployeeForGrid(status, beginTime, endTime);
            return Json(new { ChartResults = chartList, GridResults = gridList});
        }


        public ActionResult EmployeeDetailStatistic(Guid employeeId)
        {
            StatisticLogic logic = new StatisticLogic();
            EmployeeModel employeeInfo = logic.getEmployeeNameById(employeeId);
            ViewBag.EmployeeInfo = employeeInfo;
            return View();
        }

        public ActionResult StatisticDetailEmployee(Guid employeeId, DateTime? beginTime, DateTime? endTime)
        {
            StatisticLogic logic = new StatisticLogic();
            List<EmployeeDetailStatic> gridList = logic.StatisticEmployeeDetailForGrid(employeeId, beginTime, endTime);
            List<List<EmployeeDetailStatic>> chartList = logic.StatisticEmployeeDetailForChart(employeeId, beginTime, endTime);
            return Json(new { ChartResults = chartList, GridResults = gridList });
        }

        #endregion

        #region 工作量统计

        public ActionResult WorkloadStatistic()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();
            List<DictionaryItem> projectTypes = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.ProjectType);
            var projectStatus = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.ProjectStatus);

            //ForRole
            IUserData user = UserContext.CurrentUser;
            if (user.HasRole("Consultant"))
            { 
                ViewBag.forRole = true;
                var i = 0;
                DictionaryItem dict = new DictionaryItem();
                var newProjectTypes = new List<DictionaryItem>();
                for (i = 0; i < projectTypes.Count(); i++)
                {
                    dict = projectTypes[i];
                    if (dict.DictItemId == "101-04" || dict.DictItemId == "101-05")
                    {
                        newProjectTypes.Add(dict);
                    }
                }
                ViewBag.QueryProjectTypes = new DropDownList(newProjectTypes, DictionaryLogic.ValueField, DictionaryLogic.TextField, false);
            }
            else
            {
                ViewBag.forRole = false;
                ViewBag.QueryProjectTypes = new DropDownList(projectTypes, DictionaryLogic.ValueField, DictionaryLogic.TextField, true);
            }
            ViewBag.QueryProjectStatus = new DropDownList(projectStatus, DictionaryLogic.ValueField, DictionaryLogic.TextField, true);
            return View();
        }

        public ActionResult getWorkloadForYear(DateTime start, DateTime end, String status, String type)
        {
            StatisticLogic slogic = new StatisticLogic();
            List<Object> datas = slogic.getWorkloadForYear(start, end, 7, status, type);
            return Json(new { Result = datas });
        }

        //周视图
        public ActionResult WorkloadForWeekStatistic(String currentYear, String currentMonth, String projectType, String projectStatus)
        {
            ViewBag.currentYear = currentYear;
            ViewBag.currentMonth = currentMonth;
            if(String.IsNullOrEmpty(projectType))
            {
                ViewBag.projectType = "null";
            } else
            {
                ViewBag.projectType = projectType;
            }
            if(String.IsNullOrEmpty(projectStatus))
            {
                ViewBag.projectStatus = "null";
            } else
            {
                ViewBag.projectStatus = projectStatus;
            }
            
            //ForRole
            IUserData user = UserContext.CurrentUser;
            if (user.HasRole("Consultant"))
            {
                ViewBag.forRole = true;
            }
            else
            {
                ViewBag.forRole = false;
            }
            return View();
        }

        public ActionResult getWorkloadForMonth(DateTime start, DateTime end, String projectType, String projectStatus)
        {
            StatisticLogic slogic = new StatisticLogic();
            List<Object> datas = slogic.getWorkloadForYear(start, end, 10, projectStatus, projectType);
            return Json(new { Result = datas });

        }

        //日视图
        public ActionResult WorkloadForDayStatistic(String start, String end, Guid? projectId, String projectName)
        {
            if (start != null)
            {
                ViewBag.startTime = start;
            }
            if (end != null)
            {
                ViewBag.endTime = end;
            }
            if (projectId != null)
            {
                ViewBag.projectId = projectId.Value;
                ViewBag.isWeekRecord = "true";
            }
            else
            {
                ViewBag.isWeekRecord = "false";
            }
            if(!String.IsNullOrEmpty(projectName))
            {
                ViewBag.projectName = projectName;
            }
            return View();
        }

        public ActionResult GetRecordByWeek(String start, String end, Guid projectId)
        {
            StatisticLogic slogic = new StatisticLogic();
            DateTime startTime = DateTime.Parse(start);
            DateTime endTime = DateTime.Parse(end);
            var result = slogic.getWorkloadForDay(startTime, endTime, projectId);
            return Json(result);
        }

        public ActionResult GetRecordByDay(Guid empId, Guid projectId, DateTime start, DateTime end)
        {
            StatisticLogic slogic = new StatisticLogic();
            List<Object> result = slogic.getWorkRecordByProjectId(empId, projectId, start, end);
            return Json(result);
        }

        #endregion
    }
}