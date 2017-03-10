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
    /// <summary>
    /// 部门相关的业务逻辑
    /// </summary>
    public class DepartmentLogic
    {
        public List<DepartmentModel> GetAllDepartments(string departmentName, string organizationName, int pageIndex, int pageSize, out int rowCount)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT dept.*, dept2.DepartmentName AS ParentName, org.OrganizationName AS OrganizationName FROM Department dept");
                sqlBuilder.Append(" INNER JOIN Organization org ON dept.OrganizationId = org.OrganizationId")
                    .Append(" LEFT JOIN Department dept2 on dept.ParentId = dept2.DepartmentId")
                    .Append(" WHERE 1 = 1");
                if (!String.IsNullOrEmpty(departmentName))
                {
                    sqlBuilder.Append(" AND dept.DepartmentName LIKE @DepartmentName");
                    dba.AddInParameter(cmd, "DepartmentName", DbType.String, String.Concat("%", departmentName, "%"));
                }
                if (!String.IsNullOrEmpty(organizationName))
                {
                    sqlBuilder.Append(" AND org.OrganizationName LIKE @OrganizationName");
                    dba.AddInParameter(cmd, "OrganizationName", DbType.String, String.Concat("%", organizationName, "%"));
                }
                sqlBuilder.Append(" ORDER BY dept.OrganizationId, dept.ParentId, dept.DepartmentName");
                cmd.CommandText = sqlBuilder.ToString();
                List<DepartmentModel> data = dba.ExecuteReaderPaging<List<DepartmentModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new DepartmentModel
                        {
                            DepartmentId = Guid.Parse(r["DepartmentId"].ToString()),
                            DepartmentName = r.GetStringOrNull("DepartmentName"),
                            DepartmentType = r.GetStringOrNull("DepartmentType"),
                            ParentId = r.GetGuidOrNull("ParentId"),
                            ParentName = r.GetStringOrNull("ParentName"),
                            OrganizationId = r.GetGuidOrNull("OrganizationId"),
                            OrganizationName = r.GetStringOrNull("OrganizationName")
                        });
                    }, pageIndex, pageSize, out rowCount);
                return data;
            }
        }

        public object SaveDepartment(DepartmentModel deptModel)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                Organization org = (from o in context.Organizations
                                    where o.OrganizationId == deptModel.OrganizationId
                                    select o).Single();
                Guid? id = null;
                if (deptModel.DepartmentId == Guid.Empty)
                {
                    Department dept = new Department();
                    dept.DepartmentId = Guid.NewGuid();
                    dept.DepartmentName = deptModel.DepartmentName;
                    dept.DepartmentType = deptModel.DepartmentType;
                    dept.ParentId = deptModel.ParentId;
                    dept.Organization = org;
                    context.Departments.Add(dept);
                    id = dept.DepartmentId;
                }
                else
                {
                    Department oldDept = context.Departments.Where(n => n.DepartmentId == deptModel.DepartmentId).Single();
                    if (oldDept == null)
                    {
                        throw new Exception("不存在此部门！");
                    }
                    oldDept.DepartmentName = deptModel.DepartmentName;
                    oldDept.DepartmentType = deptModel.DepartmentType;
                    oldDept.ParentId = deptModel.ParentId;
                    oldDept.Organization = org;
                    id = oldDept.DepartmentId;
                }
                int countChange = context.SaveChanges();
                return new { Result = countChange > 0, ID = id };
            }
        }

        public List<DepartmentSelectItem> GetDepartmentsDict(DatabaseAccess dba = null, bool getAll = true)
        {
            bool flag = false;
            TeamDBContext context = null;
            StringBuilder sqlBuilder = new StringBuilder("SELECT * FROM Department dept");
            if (!getAll)
            {
                sqlBuilder.Append(" WHERE dept.ParentId IS null");
            }
            if (dba == null)
            {
                flag = true;
                context = new TeamDBContext();
                dba = context.DatabaseAccess();
            }
            DbCommand cmd = dba.CreateCommand();
            cmd.CommandText = sqlBuilder.ToString();
            try
            {
                List<DepartmentSelectItem> data = dba.ExecuteReader<List<DepartmentSelectItem>>(
                    cmd,
                    (d, r) =>
                    {
                        d.Add(new DepartmentSelectItem
                        {
                            DepartmentId = Guid.Parse(r["DepartmentId"].ToString()),
                            DepartmentName = r.GetStringOrNull("DepartmentName")
                        });
                    });
                return data;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    context.Dispose();
                }
            }
        }

    }
}
