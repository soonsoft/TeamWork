using STIM.Core;
using STIM.Data.EntityFrameworkExtensions;
using STIM.Web.Security;
using STIM.Web.Security.Simple;
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
    /// 员工相关的业务逻辑
    /// </summary>
    public class EmployeeLogic
    {
        /// <summary>
        /// 根据员工ID获取员工的信息
        /// </summary>
        /// <param name="employeeId">员工ID</param>
        /// <returns></returns>
        public Employee GetEmployeeById(Guid employeeId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                Employee emp = context.Employees.Where(e => e.EmployeeId == employeeId).Single();
                return emp;
            }
        }

        public EmployeeModel GetEmployeeInfoById(Guid employeeId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                EmployeeModel employeeInfo = null;
                DbCommand cmd = dba.CreateCommand("SELECT u.UserName, u.[Enabled], ue.*, e.*, di.DictItemValue AS StatusName FROM auth_User u INNER JOIN auth_UserExtend ue ON u.UserId = ue.UserId INNER JOIN Employee e ON ue.UserId = e.EmployeeId LEFT JOIN sys_DictionaryItem di ON ue.[Status] = di.DictItemId WHERE u.UserId = @EmployeeId");
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        employeeInfo = ReadEmployeeInfo<EmployeeQueryModel>(reader, false);
                    }
                }
                return employeeInfo;
            }
        }

        /// <summary>
        /// 获取当前所有在职的项目经理
        /// </summary>
        /// <returns></returns>
        public List<EmployeeSelectItem> GetProjectManager()
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT ue.UserId, ue.RealName, ue.Organization, ue.OrganizationName, ue.Department, ue.DepartmentName, ue.Position, ue.PositionName FROM auth_Role r INNER JOIN auth_UsersInRoles ur ON r.RoleId = ur.RoleId INNER JOIN auth_UserExtend ue ON ur.UserId = ue.UserId WHERE r.RoleName = 'PM' AND ue.[Status] = '1-01' ORDER BY ue.DepartmentName, ue.RealName");
                List<EmployeeSelectItem> data = dba.ExecuteReader<List<EmployeeSelectItem>>(
                    cmd, (d, r) => {
                        d.Add(new EmployeeSelectItem 
                        {
                            EmployeeId = (Guid)r["UserId"],
                            RealName = r.GetStringOrNull("RealName"),
                            Organization = r.GetStringOrNull("Organization"),
                            OrganizationName = r.GetStringOrNull("OrganizationName"),
                            Department = r.GetStringOrNull("Department"),
                            DepartmentName = r.GetStringOrNull("DepartmentName"),
                            Position = r.GetStringOrNull("Position"),
                            PositionName = r.GetStringOrNull("PositionName")
                        });
                    });
                return data;
            }
        }

        /// <summary>
        /// 获取员工（默认包含离职员工），如果只想获取在职的用户则传参数false
        /// </summary>
        /// <param name="isAll">是否获取全部用户（包含离职用户）</param>
        /// <returns></returns>
        public List<EmployeeSelectItem> GetAllEmployees(bool isAll = true)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                string sql = "SELECT ue.*, di.DictItemValue AS StatusName FROM [auth_User] u INNER JOIN [auth_UserExtend] ue ON u.UserId = ue.UserId INNER JOIN [sys_DictionaryItem] di ON ue.Status = di.DictItemId WHERE UserName <> 'admin' {0} ORDER BY ue.DepartmentName, ue.RealName";
                if(isAll)
                {
                    sql = String.Format(sql, String.Empty);
                }
                else
                {
                    //只保留在职员工
                    sql = String.Format(sql, "AND ue.STATUS = '1-01'");
                }
                DbCommand cmd = dba.CreateCommand(sql);
                List<EmployeeSelectItem> data = dba.ExecuteReader<List<EmployeeSelectItem>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new EmployeeSelectItem
                        {
                            EmployeeId = (Guid)r["UserId"],
                            RealName = r.GetStringOrNull("RealName"),
                            Organization = r.GetStringOrNull("Organization"),
                            OrganizationName = r.GetStringOrNull("OrganizationName"),
                            Department = r.GetStringOrNull("Department"),
                            DepartmentName = r.GetStringOrNull("DepartmentName"),
                            Position = r.GetStringOrNull("Position"),
                            PositionName = r.GetStringOrNull("PositionName"),
                            Status = r.GetStringOrNull("Status"),
                            StatusName = r.GetStringOrNull("StatusName")
                        });
                    });
                return data;
            }
        }

        public List<EmployeeModel> QueryEmployeeInfo(string name, string status, int pageIndex, int pageSize, out int rowCount)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT u.UserName, u.[Enabled], ue.*, e.*, lc.UnitPrice AS PositionUnitPrice, lc.UnitValue AS PositionUnitValue, lc2.UnitPrice AS UserUnitPrice, lc2.UnitValue AS UserUnitValue FROM auth_User u INNER JOIN auth_UserExtend ue ON u.UserId = ue.UserId LEFT JOIN Employee e ON ue.UserId = e.EmployeeId LEFT JOIN LaborCost lc ON ue.Position = lc.TargetId AND lc.TargetType = 1 LEFT JOIN LaborCost lc2 ON u.UserId = lc2.TargetId AND lc2.TargetType = 2 WHERE 1 = 1");
                if(!String.IsNullOrEmpty(name))
                {
                    sqlBuilder.Append(" AND ue.RealName LIKE @RealName");
                    dba.AddInParameter(cmd, "RealName", DbType.String, String.Concat("%", name, "%"));
                }
                sqlBuilder.Append(" AND ue.Status = @Status");
                if(String.IsNullOrEmpty(status))
                {
                    dba.AddInParameter(cmd, "Status", DbType.String, "1-01");
                } else
                {
                    dba.AddInParameter(cmd, "Status", DbType.String, String.Concat("1-0", status));
                }                
                sqlBuilder.Append(" ORDER BY ue.RealName, u.UserName");
                cmd.CommandText = sqlBuilder.ToString();

                List<EmployeeModel> data = null;
                using(DbDataReader reader = dba.ExecuteReaderPaging(cmd, PrepareQueryEmployeeInfoCommand, pageIndex, pageSize, out rowCount))
                {
                    if(reader.HasRows)
                    {
                        data = new List<EmployeeModel>(50);
                        Guid lastUserId = Guid.Empty;
                        Guid currentUserId;
                        EmployeeModel employee = null;
                        while (reader.Read())
                        {
                            currentUserId = reader.GetGuidOrNull("UserId").Value;
                            if (currentUserId != lastUserId)
                            {
                                employee = ReadEmployeeInfo<EmployeeQueryModel>(reader);
                                if (employee.Roles == null)
                                {
                                    employee.Roles = new List<IRoleInfo>(5);
                                }
                                data.Add(employee);
                                lastUserId = currentUserId;
                            }

                            IRoleInfo role = ReadRoleInfo(reader);
                            if (role != null)
                            {
                                employee.Roles.Add(role);
                            }
                        }
                    }
                }
                return data;
            }
        }

        public void LoadEmployeeCost(EmployeeModel employee)
        {
            EmployeeQueryModel queryModel = employee as EmployeeQueryModel;
            if(queryModel == null)
            {
                return;
            }

            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT lc.UnitPrice AS PositionUnitPrice, lc.UnitValue AS PositionUnitValue, lc2.UnitPrice AS UserUnitPrice, lc2.UnitValue AS UserUnitValue FROM auth_User u INNER JOIN auth_UserExtend ue ON u.UserId = ue.UserId LEFT JOIN LaborCost lc ON ue.Position = lc.TargetId AND lc.TargetType = 1 LEFT JOIN LaborCost lc2 ON u.UserId = lc2.TargetId AND lc2.TargetType = 2 WHERE ue.UserId = @UserId");
                dba.AddInParameter(cmd, "UserId", DbType.Guid, queryModel.UserId);
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        queryModel.PositionUnitPrice = reader.GetDecimalOrDefault("PositionUnitPrice");
                        queryModel.PositionUnitValue = reader.GetDecimalOrDefault("PositionUnitValue");
                        queryModel.UserUnitPrice = reader.GetDecimalOrDefault("UserUnitPrice");
                        queryModel.UserUnitValue = reader.GetDecimalOrDefault("UserUnitValue");
                    }
                }
            }
        }

        void PrepareQueryEmployeeInfoCommand(DbCommand cmd, DatabaseAccess dba)
        {
            cmd.CommandText = String.Concat("SELECT u.*, r.RoleId, r.RoleName, r.RoleText, di.DictItemValue AS StatusName FROM (", cmd.CommandText, ") u LEFT JOIN auth_UsersInRoles ur ON u.UserId = ur.UserId LEFT JOIN auth_Role r ON ur.RoleId = r.RoleId LEFT JOIN sys_DictionaryItem di ON u.[Status] = di.DictItemId ORDER BY u.RealName, u.UserName, r.RoleName");
        }

        T ReadEmployeeInfo<T>(DbDataReader reader, bool enableReadCost = true)
            where T : EmployeeModel, new()
        {
            EmployeeModel employeeInfo = new T()
            {
                UserId = Guid.Parse(reader["UserId"].ToString()),
                UserName = reader["UserName"].ToString(),
                Enabled = Convert.ToBoolean(reader["Enabled"]),
                UserCode = reader.GetStringOrNull("UserCode"),
                RealName = reader.GetStringOrNull("RealName"),
                Organization = reader.GetStringOrNull("Organization"),
                OrganizationName = reader.GetStringOrNull("OrganizationName"),
                Department = reader.GetStringOrNull("Department"),
                DepartmentName = reader.GetStringOrNull("DepartmentName"),
                Position = reader.GetStringOrNull("Position"),
                PositionName = reader.GetStringOrNull("PositionName"),
                Status = reader.GetStringOrNull("Status"),
                StatusName = reader.GetStringOrNull("StatusName"),

                Gender = reader.GetBooleanOrDefault("Gender", true).Value,
                Birthday = reader.GetDateTimeOrDefault("Birthday"),
                CellPhoneNumber = reader.GetStringOrNull("CellPhoneNumber"),
                LinePhoneNumber = reader.GetStringOrNull("LinePhoneNumber"),
                Email = reader.GetStringOrNull("Email"),
                Email2 = reader.GetStringOrNull("Email2"),
                QQ = reader.GetStringOrNull("QQ"),
                Address = reader.GetStringOrNull("Address")
            };

            if(employeeInfo is EmployeeQueryModel && enableReadCost)
            {
                EmployeeQueryModel empQueryInfo = employeeInfo as EmployeeQueryModel;
                empQueryInfo.PositionUnitPrice = reader.GetDecimalOrDefault("PositionUnitPrice");
                empQueryInfo.PositionUnitValue = reader.GetDecimalOrDefault("PositionUnitValue");
                empQueryInfo.UserUnitPrice = reader.GetDecimalOrDefault("UserUnitPrice");
                empQueryInfo.UserUnitValue = reader.GetDecimalOrDefault("UserUnitValue");
            }

            employeeInfo.EmployeeId = employeeInfo.UserId;
            return employeeInfo as T;
        }

        IRoleInfo ReadRoleInfo(DbDataReader reader)
        {
            Guid? roleId = reader.GetGuidOrNull("RoleId");
            if (roleId == null || !roleId.HasValue)
                return null;
            return new SimpleRole(roleId.Value, reader.GetStringOrNull("RoleName"))
            {
                RoleText = reader.GetStringOrNull("RoleText")
            };
        }

        public EmployeeModel CreateEmployeeModel(Guid employeeId, string userName)
        {
            Guard.ArgumentNotNullOrEmpty(userName, "userName");

            EmployeeModel employeeInfo = new EmployeeQueryModel
            {
                UserId = employeeId,
                EmployeeId = employeeId,
                UserName = userName
            };
            return employeeInfo;
        }

        public int InsertEmployeeHandler(DatabaseAccess dba, IUserData userData)
        {
            EmployeeModel employeeInfo = (EmployeeModel)userData;
            DbCommand cmd = dba.CreateCommand("INSERT INTO [Employee] VALUES(@EmployeeId, @Gender, @Birthday, @CellPhoneNumber, @LinePhoneNumber, @Email, @Email2, @QQ, @ID, @Address, @Photo)");
            AddParameters(dba, cmd, employeeInfo);
            return dba.ExecuteNonQuery(cmd);
        }

        public int UpdateEmployeeHandler(DatabaseAccess dba, IUserData userData)
        {
            EmployeeModel employeeInfo = (EmployeeModel)userData;
            DbCommand cmd = dba.CreateCommand("UPDATE [Employee] SET Gender = @Gender, Birthday = @Birthday, CellPhoneNumber = @CellPhoneNumber, LinePhoneNumber = @LinePhoneNumber, Email = @Email, Email2 = @Email2, QQ = @QQ, ID = @ID, Address = @Address, Photo = @Photo WHERE EmployeeId = @EmployeeId");
            AddParameters(dba, cmd, employeeInfo);
            return dba.ExecuteNonQuery(cmd);
        }

        public int DeleteEmployeeHandler(DatabaseAccess dba, Guid employeeId)
        {
            DbCommand cmd = dba.CreateCommand("DELETE FROM [Employee] WHERE EmployeeId = @EmployeeId");
            dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
            return dba.ExecuteNonQuery(cmd);
        }

        void AddParameters(DatabaseAccess dba, DbCommand cmd, EmployeeModel employeeInfo)
        {
            dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeInfo.EmployeeId);
            dba.AddInParameter(cmd, "Gender", DbType.Boolean, employeeInfo.Gender);
            dba.AddInParameter(cmd, "Birthday", DbType.Date, employeeInfo.Birthday);
            dba.AddInParameter(cmd, "CellPhoneNumber", DbType.String, employeeInfo.CellPhoneNumber);
            dba.AddInParameter(cmd, "LinePhoneNumber", DbType.String, employeeInfo.LinePhoneNumber);
            dba.AddInParameter(cmd, "Email", DbType.String, employeeInfo.Email);
            dba.AddInParameter(cmd, "Email2", DbType.String, employeeInfo.Email2);
            dba.AddInParameter(cmd, "QQ", DbType.String, employeeInfo.QQ);
            dba.AddInParameter(cmd, "ID", DbType.String, employeeInfo.ID);
            dba.AddInParameter(cmd, "Address", DbType.String, employeeInfo.Address);
            dba.AddInParameter(cmd, "Photo", DbType.String, employeeInfo.Photo);
        }

        public List<EmployeeSelectItem> GetEmployeesDict(Guid? pmId = null, DatabaseAccess dba = null)
        {
            bool flag = false;
            TeamDBContext context = null;
            if (dba == null)
            {
                flag = true;
                context = new TeamDBContext();
                dba = context.DatabaseAccess();
            }
            DbCommand cmd = dba.CreateCommand();

            StringBuilder sqlBuilder = new StringBuilder("SELECT DISTINCT u.UserId, u.RealName FROM auth_UserExtend u");
            if (pmId == null)
            {
                sqlBuilder.Append(" WHERE u.Position IS NOT NULL");
            }
            else 
            {
                sqlBuilder.Append(" INNER JOIN ProjectTeam pt ON pt.EmployeeId = u.UserId")
                .Append(" INNER JOIN(SELECT DISTINCT p.ProjectId FROM Project p INNER JOIN Stage s ON s.ProjectId = p.ProjectId")
                .Append(" WHERE (p.PM = @PM OR s.Manager = @PM)) t ON pt.ProjectId = t.ProjectId");
            }
            cmd.CommandText = sqlBuilder.ToString();
            dba.AddInParameter(cmd, "PM", DbType.Guid, pmId);
            try
            {
                List<EmployeeSelectItem> data = dba.ExecuteReader<List<EmployeeSelectItem>>(
                    cmd,
                    (d, r) =>
                    {
                        d.Add(new EmployeeSelectItem
                        {
                            EmployeeId = Guid.Parse(r["UserId"].ToString()),
                            RealName = r.GetStringOrNull("RealName")
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
