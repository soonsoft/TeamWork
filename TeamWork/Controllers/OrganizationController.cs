using System.Web.Mvc;
using TeamWork.BLL;
using STIM.Web;
using TeamWork.BLL.Models;
using System.Collections.Generic;
using TeamWork.DAL;
using System;

namespace TeamWork.Controllers
{
    public class OrganizationController : Controller
    {
        #region 机构管理

        //进入页面
        public ActionResult OrganizationManage()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();

            var OrganizationType = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.OrganizationType);
            ViewBag.OrganizationType = new DropDownList(OrganizationType, DictionaryLogic.ValueField, DictionaryLogic.TextField, false);

            List<OrganizationSelectItem> oList;
            dictLogic.OrganizationDictionary(out oList, false);
            ViewBag.parentOrganizations = new DropDownList(oList, "OrganizationId", "OrganizationName", true).SetCustomText(" ");

            List<EmployeeSelectItem> eList;
            dictLogic.EmployeeDictionary(out eList);
            ViewBag.Employees = new DropDownList(eList, "EmployeeId", "RealName", true).SetCustomText(" ");

            return View();
        }

        //获取所有机构信息
        public ActionResult GetAllOrganizations(string organizationName, int pageIndex, int pageSize)
        {
            OrganizationLogic logic = new OrganizationLogic();
            int rowCount;
            var data = logic.GetAllOrganizations(organizationName, pageIndex, pageSize, out rowCount);
            return Json(new { Result = data, RowCount = rowCount });
        }

        //保存机构信息
        public ActionResult SaveOrganization(Organization org)
        {
            if (org.OrganizationId == null || org.OrganizationId.ToString().Length == 0)
            {
                org.OrganizationId = Guid.Empty;
            }
            if (org.ParentId == org.OrganizationId)
            {
                return Json(new { Result = false, ErrorMessage = String.Format("[{0}] 不可将 [{0}] 指定为所属机构", org.OrganizationName) });
            }
            OrganizationLogic logic = new OrganizationLogic();
            var result = logic.SaveOrganization(org);
            return Json(result);
        }

        #endregion

        #region 部门管理

        //进入页面
        public ActionResult DepartmentManage()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();

            List<DepartmentSelectItem> dList;
            dictLogic.DepartmentDictionary(out dList, false);
            ViewBag.parentDepartments = new DropDownList(dList, "DepartmentId", "DepartmentName", true).SetCustomText(" ");

            List<OrganizationSelectItem> oList;
            dictLogic.OrganizationDictionary(out oList, false);
            ViewBag.Organizations = new SelectList(oList, "OrganizationId", "OrganizationName");

            return View();
        }

        //获取所有部门信息
        public ActionResult GetAllDepartments(string departmentName, string organizationName, int pageIndex, int pageSize)
        {
            DepartmentLogic logic = new DepartmentLogic();
            int rowCount;
            var data = logic.GetAllDepartments(departmentName, organizationName, pageIndex, pageSize, out rowCount);
            return Json(new { Result = data, RowCount = rowCount });
        }

        //保存部门信息
        public ActionResult SaveDepartment(DepartmentModel deptModel)
        {
            if (deptModel.DepartmentId == null || deptModel.DepartmentId.ToString().Length == 0)
            {
                deptModel.DepartmentId = Guid.Empty;
            }
            if (deptModel.ParentId == deptModel.DepartmentId)
            {
                return Json(new { Result = false, ErrorMessage = String.Format("[{0}] 不可将 [{0}] 指定为上级部门", deptModel.ParentName) });
            }
            DepartmentLogic logic = new DepartmentLogic();
            var result = logic.SaveDepartment(deptModel);
            return Json(result);
        }

        #endregion

        #region 职位管理

        //进入页面
        public ActionResult PositionManage()
        {
            DictionaryLogic dictLogic = new DictionaryLogic();

            var PositionLevel = dictLogic.GetEnabledDictionaryItems(DictionaryLogic.PositionLevel);
            ViewBag.PositionLevel = new DropDownList(PositionLevel, DictionaryLogic.ValueField, DictionaryLogic.TextField, true).SetCustomText(" ");

            List<DepartmentSelectItem> dList;
            dictLogic.DepartmentDictionary(out dList, true);
            ViewBag.Departments = new SelectList(dList, "DepartmentId", "DepartmentName");

            return View();
        }

        //获取所有职位信息
        public ActionResult GetAllPositions(string positionName, string departmentName, int pageIndex, int pageSize)
        {
            PositionLogic logic = new PositionLogic();
            int rowCount;
            var data = logic.GetAllPositions(positionName, departmentName, pageIndex, pageSize, out rowCount);
            return Json(new { Result = data, RowCount = rowCount });
        }

        //保存职位信息
        public ActionResult SavePostion(PositionModel pModel)
        {
            if (pModel.PositionId == null || pModel.PositionId.ToString().Length == 0)
            {
                pModel.PositionId = Guid.Empty;
            }
            PositionLogic logic = new PositionLogic();
            var result = logic.SavePosition(pModel);
            return Json(result);
        }

        #endregion
    }
}