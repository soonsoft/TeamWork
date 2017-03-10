using STIM.Data.EntityFrameworkExtensions;
using STIM.Modules.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.BLL.Models;
using TeamWork.DAL;

namespace TeamWork.BLL
{
    /// <summary>
    /// 字典相关的业务逻辑
    /// </summary>
    public class DictionaryLogic : DictionaryLogicBase
    {
        public const string TextField = "DictItemValue";
        public const string ValueField = "DictItemId";

        #region Groups

        /// <summary>
        /// 员工状态
        /// </summary>
        public const string EmployeeStatus = "1";

        /// <summary>
        /// 机构类型
        /// </summary>
        public const string OrganizationType = "2";

        /// <summary>
        /// 出勤类型
        /// </summary>
        public const string AttendanceType = "3";

        /// <summary>
        /// 部门类型
        /// </summary>
        public const string DepartmentType = "4";

        /// <summary>
        /// 职位级别
        /// </summary>
        public const string PositionLevel = "5";

        /// <summary>
        /// 请假类型
        /// </summary>
        public const string LeaveType = "6";

        /// <summary>
        /// 项目类型
        /// </summary>
        public const string ProjectType = "101";

        /// <summary>
        /// 项目状态
        /// </summary>
        public const string ProjectStatus = "102";

        /// <summary>
        /// 阶段状态
        /// </summary>
        public const string StageStatus = "103";

        /// <summary>
        /// 预算种类
        /// </summary>
        public const string BudgetCategory = "204";

        #endregion

        public void OrganizationDictionary(
            out List<OrganizationSelectItem> organizations,
            out List<DepartmentSelectItem> departments,
            out List<PositionSelectItem> positions)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                using (var conn = dba.GetOpenConnection())
                {
                    OrganizationLogic oLogic = new OrganizationLogic();
                    organizations = oLogic.GetOrganizationsDict(dba);
                    DepartmentLogic dLogic = new DepartmentLogic();
                    departments = dLogic.GetDepartmentsDict(dba);
                    PositionLogic pLogic = new PositionLogic();
                    positions = pLogic.GetPositionsDict(dba);
                }
            }
        }

        public void EmployeeDictionary(out List<EmployeeSelectItem> employees, Guid? pmId = null)
        {

            using (TeamDBContext context = new TeamDBContext())
            {

                DatabaseAccess dba = context.DatabaseAccess();

                EmployeeLogic elogic = new EmployeeLogic();

                employees = elogic.GetEmployeesDict(pmId, dba);

            }
        }

        public void OrganizationDictionary(out List<OrganizationSelectItem> parentOrganizations, bool getAll)
        {
            using (TeamDBContext context = new TeamDBContext())
            {

                DatabaseAccess dba = context.DatabaseAccess();

                OrganizationLogic logic = new OrganizationLogic();

                parentOrganizations = logic.GetOrganizationsDict(dba, getAll);

            }
        }



        public void DepartmentDictionary(out List<DepartmentSelectItem> parentDepartments, bool getAll)
        {

            using (TeamDBContext context = new TeamDBContext())
            {

                DatabaseAccess dba = context.DatabaseAccess();

                DepartmentLogic logic = new DepartmentLogic();

                parentDepartments = logic.GetDepartmentsDict(dba, getAll);
            }
        }

        public void PositionDictionary(out List<PositionSelectItem> Positions)
        {

            using (TeamDBContext context = new TeamDBContext())
            {

                DatabaseAccess dba = context.DatabaseAccess();

                PositionLogic logic = new PositionLogic();

                Positions = logic.GetPositionsDict(dba);
            }
        }

        public void ProjectDictionary(out List<ProjectSelectItem> projects, Guid? pmId = null)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();

                ProjectLogic logic = new ProjectLogic();

                projects = logic.GetProjectDict(pmId, dba);
            }
        }
        public void ProjectStageDictionary(out List<ProjectStageSelectItem> projectStages, Guid? pmId = null)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();

                ProjectLogic logic = new ProjectLogic();

                projectStages = logic.GetProjectStageDict(pmId, dba);
            }
        }


    }
}
