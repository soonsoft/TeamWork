using STIM.Web;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TeamWork.BLL;
using TeamWork.BLL.Models;
using TeamWork.DAL;

namespace TeamWork.Controllers
{
    public class ProjectController : Controller
    {
        #region Project

        //
        // GET: /Project/
        public ActionResult Manage()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();
            var projectTypes = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.ProjectType);
            var projectStatus = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.ProjectStatus);
            ViewBag.QueryProjectTypes = new DropDownList(
                projectTypes, DictionaryLogic.ValueField, DictionaryLogic.TextField, true);
            ViewBag.ProjectTypes = new SelectList(
                projectTypes, DictionaryLogic.ValueField, DictionaryLogic.TextField);
            ViewBag.QueryProjectStatus = new DropDownList(
                projectStatus, DictionaryLogic.ValueField, DictionaryLogic.TextField, true);
            ViewBag.ProjectStatus = new SelectList(
                projectStatus, DictionaryLogic.ValueField, DictionaryLogic.TextField);
            ViewBag.StageStatus = new SelectList(
                dictLogic.GetEnabledDictionaryItems(DictionaryLogic.StageStatus), 
                DictionaryLogic.ValueField, DictionaryLogic.TextField);
            ViewBag.BudgetCategory = new SelectList(
                dictLogic.GetEnabledDictionaryItems(DictionaryLogic.BudgetCategory),
                DictionaryLogic.ValueField, DictionaryLogic.TextField);

            EmployeeLogic empLogic = new EmployeeLogic();
            ViewBag.AllEmployees = empLogic.GetAllEmployees(false);

            return View();
        }

        public ActionResult QueryProjects(
            string projectName,
            string projectType,
            string status,
            DateTime? beginTime,
            DateTime? endTime,
            int pageIndex,
            int pageSize)
        {
            ProjectLogic logic = new ProjectLogic();
            int rowCount;
            var data = logic.QueryProjects(projectName, projectType, status, beginTime, endTime, pageIndex, pageSize, out rowCount);
            return Json(new { Result = data, RowCount = rowCount });
        }

        public ActionResult GetSubprojects(Guid projectId)
        {
            ProjectLogic logic = new ProjectLogic();
            var data = logic.GetSubprojects(projectId);
            return Json(data);
        }

        public ActionResult GetMasterProjects()
        {
            ProjectLogic logic = new ProjectLogic();
            var data = logic.GetMasterProject();
            return Json(data);
        }

        public ActionResult NewProject(Project project, Guid[] teamEmployees, Budget[] budgets)
        {
            ProjectLogic logic = new ProjectLogic();
            var result = logic.InsertProject(project, teamEmployees, budgets);
            return Json(new { Result = result });
        }

        public ActionResult EditProject(Project project, Guid[] teamEmployees, Budget[] budgets, bool? clearBudgets)
        {
            ProjectLogic logic = new ProjectLogic();
            if(budgets == null && clearBudgets == true)
            {
                budgets = new Budget[0];
            }
            var result = logic.UpdateProject(project, teamEmployees, budgets);
            return Json(new { Result = result });
        }

        public ActionResult GetTeam(Guid projectId)
        {
            ProjectLogic logic = new ProjectLogic();
            var data = logic.GetTeamByProjectId(projectId);
            return Json(data);
        }

        public ActionResult GetBudgets(Guid projectId)
        {
            ProjectLogic logic = new ProjectLogic();
            var data = logic.GetBudgetsByProjectId(projectId);
            return Json(data);
        }

        #endregion //End Project

        #region Stage

        public ActionResult GetStages(Guid projectId)
        {
            ProjectLogic logic = new ProjectLogic();
            var data = logic.FindStageByProjectId(projectId);
            return Json(data);
        }

        public ActionResult UpdateStages(Guid projectId, List<Stage> stages)
        {
            ProjectLogic logic = new ProjectLogic();
            var result = logic.UpdateProjectStages(projectId, stages);
            return Json(new { Result = result });
        }

        #endregion

        #region LaborCost

        public ActionResult LaborCostManage()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();

            List<EmployeeSelectItem> emps;
            dictLogic.EmployeeDictionary(out emps);
            ViewBag.Employees = emps;

            List<PositionSelectItem> pList;
            dictLogic.PositionDictionary(out pList);
            ViewBag.Positions = pList;
            ViewBag.PositionList = new DropDownList(pList, "PositionId", "PositionName", false);

            var laborCostList = new List<object>();
            laborCostList.Add(new { TypeId = (int)TargetType.Position, TypeName = "职位" });
            laborCostList.Add(new { TypeId = (int)TargetType.Employee, TypeName = "人员" });
            ViewBag.QueryType = new DropDownList(laborCostList, "TypeId", "TypeName", true);
            ViewBag.TargetType = new DropDownList(laborCostList, "TypeId", "TypeName", false);

            return View();
        }

        public ActionResult GetAllLaborCosts(int? targetType, int pageIndex, int pageSize)
        {
            ProjectLogic logic = new ProjectLogic();
            int rowCount = 0;
            List<LaborCostSelectItem> result = logic.GetAllLaborCosts(targetType, pageIndex, pageSize, out rowCount);
            return Json(new { Result = result,RowCount = rowCount});
        }

        public ActionResult SaveLaborCost(LaborCostSelectItem labItem)
        {
            if (labItem.LaborCostId == null)
            {
                labItem.LaborCostId = Guid.Empty;
            }
            ProjectLogic logic = new ProjectLogic();
            var result = logic.SaveLaborCost(labItem);
            return Json(result);
        }

        public ActionResult DeleteLaborCost(Guid?[] laborCostIds)
        {
            ProjectLogic logic = new ProjectLogic();
            Boolean result = logic.DeleteLaborCost(laborCostIds);
            return Json(new { Result = result});
        }

        #endregion

        #region StageTemplete

        public ActionResult TempleteManage()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();

            var projectTypes = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.ProjectType);
            ViewBag.ProjectTypes = projectTypes;

            return View();
        }

        public ActionResult GetTempletes(string projectType)
        {
            ProjectLogic logic = new ProjectLogic();
            List<StageTemplete> result = logic.GetTempletes(projectType);
            return Json(new { Result = result });
        }

        public ActionResult SaveTemplete(StageTemplete st, Guid?[] stageTempleteIds)
        {
            if (st.StageTempleteId == null)
            {
                st.StageTempleteId = Guid.Empty;
            }
            ProjectLogic logic = new ProjectLogic();
            var result = logic.SaveTemplete(st, stageTempleteIds);
            return Json(result);
        }

        public ActionResult DeleteTemplete(Guid? stageTempleteId)
        {
            ProjectLogic logic = new ProjectLogic();
            Boolean result = logic.DeleteTemplete(stageTempleteId);
            return Json(new { Result = result });
        }

        #endregion

        #region ProjectMeetingContent
        public ActionResult ProjectMeetingContent()
        {
            ProjectLogic plogic = new ProjectLogic();
            var projects = plogic.GetProjectDict();
            ViewBag.Projects = projects;
            ViewBag.ProjectSelectList = new SelectList(
                projects, "ProjectId", "ProjectName");

            EmployeeLogic empLogic = new EmployeeLogic();
            ViewBag.AllEmployees = empLogic.GetAllEmployees(false);

            return View();
        }

        public ActionResult QueryProjectMeetingDatas(Guid? projectId, string isSelf, int pageIndex, int pageSize)
        {
            int rowCount = 0;
            ProjectLogic logic = new ProjectLogic();
            if (projectId == null)
            {
                projectId = Guid.Empty;
            }
            var result = logic.getProjectMeetingSummarys(projectId, isSelf, pageIndex, pageSize, out rowCount);
            return Json(new { Result = result, RowCount = rowCount });
        }

        public ActionResult SaveProjectMeetingDatas(ProjectMeetingSummary pms, Guid ProjectId, List<Guid> empGuidList, List<string> empNameList, List<string> AccesList)
        {
            ProjectLogic logic = new ProjectLogic();
            bool result = false;
            if (pms.MeetingId == null || pms.MeetingId == Guid.Empty)
            {
                result = logic.saveProjectMeeting(pms, ProjectId, empGuidList, empNameList, AccesList);
            }
            else
            {
                result = logic.updateProjectMeeting(pms, ProjectId, empGuidList, empNameList, AccesList);
            }
            
            return Json(result);
        }

        #endregion
    }
}