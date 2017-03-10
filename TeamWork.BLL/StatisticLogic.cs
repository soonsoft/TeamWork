using STIM.Data.EntityFrameworkExtensions;
using STIM.Modules.Dictionary;
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
    public class StatisticLogic
    {
        #region recordStatistic

        public List<object> GetRecordByMonth(DateTime start, DateTime end, Guid? projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT du.DateStr, du.UserId AS EmployeeId, du.RealName, ISNULL(wg.SumHour, 0) AS SumHour FROM");
                sql.Append(" (SELECT s.UserId, s.RealName, d.DateStr FROM");
                sql.Append(" (SELECT us.UserId, us.RealName FROM auth_UserExtend us");
                sql.Append(" INNER JOIN auth_User u ON us.UserId = u.UserId");
                sql.Append(" WHERE u.UserName <> 'admin' AND us.[Status] = '1-01'");
                sql.Append(" AND NOT EXISTS(SELECT 1 FROM auth_Role r");
                sql.Append(" INNER JOIN auth_UsersInRoles ur ON r.RoleId = ur.RoleId");
                sql.Append(" WHERE ur.UserId = u.UserId AND RoleName = 'Executive')) s");
                sql.Append(" LEFT JOIN (SELECT h.DateStr FROM (SELECT CONVERT(nvarchar(10), BeginTime, 120) AS DateStr FROM WorkTime");
                sql.Append(" WHERE BeginTime >= @startTime AND EndTime <= @endTime");
                if(Guid.Empty != projectId.Value)
                {
                    sql.Append(" AND ProjectId = @projectId");
                }
                sql.Append(" ) h GROUP BY h.DateStr) d");
                sql.Append(" ON s.RealName <> d.DateStr) du");
                sql.Append(" LEFT JOIN (SELECT h1.DateStr, h1.EmployeeId, SUM([Hours]) AS SumHour FROM");
                sql.Append(" (SELECT [Hours], EmployeeId, CONVERT(nvarchar(10), BeginTime, 120) AS DateStr FROM WorkTime");
                sql.Append(" WHERE BeginTime >= @startTime AND EndTime <= @endTime");
                if (Guid.Empty != projectId.Value)
                {
                    sql.Append(" AND ProjectId = @projectId");
                }
                sql.Append(" ) h1 GROUP BY h1.DateStr, h1.EmployeeId) wg");
                sql.Append(" ON du.DateStr = wg.DateStr AND du.UserId = wg.EmployeeId");
                sql.Append(" WHERE du.DateStr IS NOT NULL");
                sql.Append(" ORDER BY du.DateStr, du.RealName");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                dba.AddInParameter(cmd, "startTime", start);
                dba.AddInParameter(cmd, "endTime", end);
                if (Guid.Empty != projectId.Value)
                {
                    dba.AddInParameter(cmd, "projectId", projectId.Value);
                }
                List<Object> records = dba.ExecuteReader<List<Object>>(
                cmd, (d, r) =>
                {
                    d.Add(new
                    {
                        DateStr = r.GetStringOrNull("DateStr"),
                        EmployeeId = (Guid)r["EmployeeId"],
                        EmployeeName = r.GetStringOrNull("RealName"),
                        SumHour = r.GetDecimalOrDefault("SumHour")
                    });
                });
                return records;
            }
        }

        #endregion

		#region ProjectStatistic

        public List<SumHourModel> QueryProjects(string projectName, string projectType, string status, DateTime? beginTime, DateTime? endTime, int pageIndex, int pageSize, out int rowCount)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT projects.ProjectId, projects.ProjectName, projects.ChildrenCount, projects.BeginTime, projects.EndTime, wt.SumHour")
                    .Append(" FROM (")
                    .Append(" SELECT p.*, ChildrenCount FROM Project p")
                    .Append(" INNER JOIN (SELECT p.ParentId AS ProjectId, COUNT(1) AS ChildrenCount FROM Project p GROUP BY p.ParentId HAVING p.ParentId IS NOT NULL) parent")
                    .Append(" ON p.ProjectId = parent.ProjectId WHERE ChildrenCount > 1")
                    .Append(" UNION ALL")
                    .Append(" SELECT p.*, 0 AS ChildrenCount FROM Project p")
                    .Append(" INNER JOIN (SELECT p.ParentId AS ProjectId, COUNT(1) AS ChildrenCount FROM Project p GROUP BY p.ParentId HAVING p.ParentId IS NOT NULL) parent")
                    .Append(" ON p.ParentId = parent.ProjectId WHERE ChildrenCount = 1")
                    .Append(" ) projects")
                    .Append(" LEFT JOIN (SELECT w.ProjectId, SUM(w.Hours) AS SumHour FROM WorkTime w GROUP BY w.ProjectId HAVING w.ProjectId IS NOT NULL) wt ON projects.ProjectId = wt.ProjectId")
                    .Append(" WHERE 1 = 1");

                if (!String.IsNullOrEmpty(projectName))
                {
                    sqlBuilder.Append(" AND projects.ProjectName LIKE @ProjectName");
                    dba.AddInParameter(cmd, "ProjectName", DbType.String, String.Concat("%", projectName, "%"));
                }
                if (!String.IsNullOrEmpty(projectType))
                {
                    sqlBuilder.Append(" AND projects.ProjectType = @ProjectType");
                    dba.AddInParameter(cmd, "ProjectType", DbType.String, projectType);
                }
                if (!String.IsNullOrEmpty(status))
                {
                    sqlBuilder.Append(" AND projects.[Status] = @Status");
                    dba.AddInParameter(cmd, "Status", DbType.String, status);
                }
                if (beginTime != null)
                {
                    sqlBuilder.Append(" AND projects.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, QueryParameterUtility.DateTimeStart(beginTime.Value));
                }
                if (endTime != null)
                {
                    sqlBuilder.Append(" AND projects.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, QueryParameterUtility.DateTimeEnd(endTime.Value));
                }
                sqlBuilder.Append(" ORDER BY projects.BeginTime DESC, projects.ProjectName ASC");
                cmd.CommandText = sqlBuilder.ToString();

                List<SumHourModel> results = dba.ExecuteReaderPaging<List<SumHourModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new SumHourModel
                        {
                            ProjectId = r.GetGuidOrNull("ProjectId").Value,
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            BeginTime = r.GetStringOrNull("BeginTime"),
                            EndTime = r.GetStringOrNull("EndTime"),
                            ChildrenCount = r.GetIntOrDefault("ChildrenCount").Value,
                            SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : (r.GetDoubleOrDefault("SumHour")).Value
                        });
                    }, pageIndex, pageSize, out rowCount);

                int length = results.Count;
                SumHourModel result = null;
                if (length > 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        result = results[i];
                        if (result.ChildrenCount > 0)
                        {
                            double SumHour = GetParentProjectSumHour(result.ProjectId);
                            result.SumHour = SumHour;
                        }
                    }
                }

                return results;
            }
        }

        //获取父级项目的开发总时数
        public double GetParentProjectSumHour(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT SUM(project.SumHour) AS SumHour FROM(");
                sqlBuilder.Append(" SELECT p.ProjectId, wt.SumHour FROM (select _p.projectId, _p.ProjectName, _p.ParentId from project _p where _p.ParentId = @ProjectId) p");
                sqlBuilder.Append(" INNER JOIN (SELECT w.ProjectId, SUM(w.Hours) AS SumHour from WorkTime w GROUP BY w.ProjectId) wt ON p.ProjectId = wt.ProjectId) project");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();

                double result = 0;
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        result = reader.GetDoubleOrDefault("SumHour") == null ? 0 : reader.GetDoubleOrDefault("SumHour").Value;
                    }
                }
                return result;
            }
        }

        //根据项目ID获取子项目
        public List<SumHourModel> QuerySubProjects(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT projects.ProjectId, projects.ProjectName, projects.ChildrenCount, projects.BeginTime, projects.EndTime, projects.SumHour")
                    .Append(" FROM (")
                    .Append(" SELECT p.*, 0 AS ChildrenCount, SumHour FROM Project p")
                    .Append(" INNER JOIN")
                    .Append(" (SELECT p.ParentId AS ProjectId, COUNT(1) AS ChildrenCount FROM Project p GROUP BY p.ParentId HAVING p.ParentId IS NOT NULL) parent")
                    .Append(" ON p.ParentId = parent.ProjectId")
                    .Append(" LEFT JOIN")
                    .Append(" (SELECT w.ProjectId, SUM(w.Hours) AS SumHour FROM WorkTime w GROUP BY w.ProjectId HAVING w.ProjectId IS NOT NULL) wt")
                    .Append(" ON p.projectId = wt.ProjectId")
                    .Append(" ) projects")
                    .Append(" WHERE projects.ParentId = @ParentId");
                dba.AddInParameter(cmd, "ParentId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();

                List<SumHourModel> results = dba.ExecuteReader<List<SumHourModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new SumHourModel
                        {
                            ProjectId = r.GetGuidOrNull("ProjectId").Value,
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            BeginTime = r.GetStringOrNull("BeginTime"),
                            EndTime = r.GetStringOrNull("EndTime"),
                            ChildrenCount = r.GetIntOrDefault("ChildrenCount").Value,
                            SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : (r.GetDoubleOrDefault("SumHour")).Value
                        });
                    });
                return results;
            }
        }

        //根据项目ID获取项目投入成本
        public SumHourModel GetTotalValueById(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT projects.ProjectName, SUM(projects.SumHour) AS TotalHour, SUM(projects.SumValue) AS TotalValue")
                    .Append(" FROM (")
                    .Append(" SELECT p.ProjectName, aue.RealName, lc.UnitValue, wt.SumHour, (wt.SumHour * (lc.UnitValue / 8)) AS SumValue")
                    .Append(" FROM (SELECT w.ProjectId, w.EmployeeId, SUM(w.Hours) AS SumHour FROM WorkTime w GROUP BY w.EmployeeId, w.ProjectId) wt")
                    .Append(" INNER JOIN auth_UserExtend aue ON aue.UserId = wt.EmployeeId")
                    .Append(" INNER JOIN LaborCost lc ON lc.TargetId = aue.Position")
                    .Append(" INNER JOIN project p ON p.ProjectId = wt.ProjectId")
                    .Append(" WHERE wt.ProjectId = @ProjectId")
                    .Append(" ) projects")
                    .Append(" GROUP BY projects.ProjectName");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();

                SumHourModel result = null;
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        result = new SumHourModel
                        {
                            ProjectName = reader.GetStringOrNull("ProjectName"),
                            TotalHour = reader.GetDoubleOrDefault("TotalHour") == null ? 0 : (reader.GetDoubleOrDefault("TotalHour")).Value,
                            TotalValue = reader.GetDoubleOrDefault("TotalValue") == null ? 0 : (reader.GetDoubleOrDefault("TotalValue")).Value
                        };
                    }
                }
                return result;
            }
        }

        //更具项目ID获取其所有子项目的ID
        public List<Guid> GetSubProjectIdsById(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT p.ProjectId FROM Project p WHERE p.ParentId = @ProjectId");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();

                List<Guid> results = dba.ExecuteReader<List<Guid>>(
                    cmd, (d, r) =>
                    {
                        d.Add(
                            r.GetGuidOrNull("ProjectId").Value
                        );
                    });
                return results;
            }
        }

        //获取所有项目的在某一个时段的开发总时数
        public Double GetTotalHour()
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT SUM(p.SumHour) AS TotalHour FROM");
                sqlBuilder.Append(" (SELECT p.ProjectId, wt.SumHour FROM project p");
                sqlBuilder.Append(" INNER JOIN (SELECT w.ProjectId, SUM(w.Hours) AS SumHour from WorkTime w GROUP BY w.ProjectId) wt ON p.ProjectId = wt.ProjectId");
                sqlBuilder.Append(" WHERE p.ParentId IS NOT NULL) p");
                cmd.CommandText = sqlBuilder.ToString();

                Double result = 0;
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        result = reader.GetDoubleOrDefault("TotalHour").Value;
                    }
                }
                return result;
            }
        }

        // 根据项目ID获取项目信息
        public ProjectModel GetProjectDetailById(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("");
                // 添加SQL语句
                sqlBuilder.Append("SELECT p.ProjectId,p.ProjectName,di.DictItemValue AS ProjectType,di2.DictItemValue AS ProjectStatus,wt.TotalHour ");
                sqlBuilder.Append(" FROM Project p ");
                sqlBuilder.Append(" INNER JOIN sys_DictionaryItem di ON di.DictItemId = p.ProjectType ");
                sqlBuilder.Append(" INNER JOIN sys_DictionaryItem di2 ON di2.DictItemId = p.Status ");
                sqlBuilder.Append(" LEFT JOIN ( ");
                sqlBuilder.Append(" SELECT SUM(Hours) AS TotalHour,ProjectId FROM WorkTime GROUP BY ProjectId ");
                sqlBuilder.Append(" ) wt ON wt.ProjectId = p.ProjectId ");
                sqlBuilder.Append(" WHERE p.ProjectId = @ProjectId ");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();

                ProjectModel result = null;
                using (DbDataReader r = dba.ExecuteReader(cmd))
                {
                    if (r.Read())
                    {
                        result = new ProjectModel
                        {
                            ProjectId = r.GetGuidOrNull("ProjectId").Value,
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            ProjectTypeName = r.GetStringOrNull("ProjectType"),
                            StatusName = r.GetStringOrNull("ProjectStatus"),
                            TotalHour = r.GetDoubleOrDefault("TotalHour") == null ? 0 : r.GetDoubleOrDefault("TotalHour").Value
                        };
                    }
                }
                return result;
            }
        }

        //按阶段统计项目
        public List<Object> GetProjectStageDetailById(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT projects.StageName, MIN(projects.BeginTime) AS BeginTime, MAX(projects.EndTime) AS EndTime, SUM(projects.SumHour) AS SumHour, SUM(projects.SumValue) AS SumValue, projects.SortValue");
                sqlBuilder.Append(" FROM (");
                sqlBuilder.Append(" SELECT s.StageId, s.StageName, wt.BeginTime, wt.EndTime, wt.SumHour, aue.RealName, lc.UnitValue, (wt.SumHour * (lc.UnitValue / 8)) AS SumValue, s.SortValue");
                sqlBuilder.Append("  FROM (SELECT _s.StageId, _s.StageName, _s.SortValue FROM Stage _s WHERE _S.ProjectId = @ProjectId) s");
                sqlBuilder.Append("	 LEFT JOIN (SELECT w.StageId, w.EmployeeId, SUM(w.Hours) AS SumHour, MIN(BeginTime) AS BeginTime, MAX(EndTime) AS EndTime FROM WorkTime w GROUP BY w.StageId, w.EmployeeId) wt ON wt.StageId = s.StageId");
                sqlBuilder.Append("	 LEFT JOIN auth_UserExtend aue ON aue.UserId = wt.EmployeeId");
                sqlBuilder.Append("	 LEFT JOIN LaborCost lc ON lc.TargetId = aue.Position");
                sqlBuilder.Append("  ) projects");
                sqlBuilder.Append("  GROUP BY projects.StageId, projects.StageName, projects.SortValue");
                sqlBuilder.Append("  ORDER BY projects.SortValue");
                // 添加参数
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();
                List<Object> data = dba.ExecuteReader<List<Object>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new
                        {
                            StageName = r.GetStringOrNull("StageName"),
                            BeginTime = r.GetStringOrNull("BeginTime") == null ? "" : r.GetStringOrNull("BeginTime"),
                            EndTime = r.GetStringOrNull("EndTime") == null ? "" : r.GetStringOrNull("EndTime"),
                            SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : r.GetDoubleOrDefault("SumHour").Value,
                            SumValue = r.GetDoubleOrDefault("SumValue") == null ? 0 : r.GetDoubleOrDefault("SumValue").Value
                        });
                    });
                return data;
            }
        }

        //根据项目ID获取参与该项目的每个部门的总耗时
        public List<Object> GetProjectDeparemntSumHourById(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT projects.DepartmentName, MIN(projects.BeginTime) AS BeginTime, MAX(Projects.EndTime) AS EndTime, SUM(projects.SumHour) AS SumHour");
                sqlBuilder.Append(" FROM (");
                sqlBuilder.Append(" SELECT d.DepartmentName, ww.BeginTime, ww.EndTime, ww.YearMonth, ww.SumHour FROM Department d");
                sqlBuilder.Append(" LEFT JOIN (");
                sqlBuilder.Append(" SELECT aue.Department, MIN(wt.BeginTime) AS BeginTime, MAX(wt.EndTime) AS EndTime, CONVERT(varchar(7), wt.BeginTime, 120) AS YearMonth, SUM(wt.Hours) SumHour FROM auth_UserExtend aue, WorkTime wt");
                sqlBuilder.Append(" WHERE aue.UserId = wt.EmployeeId AND ProjectId = @ProjectId");
                sqlBuilder.Append(" GROUP BY CONVERT(varchar(7), wt.BeginTime, 120), aue.Department");
                sqlBuilder.Append(" ) ww ON ww.Department = d.DepartmentId");
                sqlBuilder.Append(" WHERE d.ParentId IS NOT NULL");
                sqlBuilder.Append(" ) projects");
                sqlBuilder.Append(" GROUP BY projects.DepartmentName");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();
                List<Object> data = dba.ExecuteReader<List<Object>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new
                        {
                            DepartmentName = r.GetStringOrNull("DepartmentName"),
                            BeginTime = r.GetStringOrNull("BeginTime"),
                            EndTime = r.GetStringOrNull("EndTime"),
                            SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : r.GetDoubleOrDefault("SumHour")
                        });
                    });
                return data;
            }
        }

        //按部门统计项目
        public List<List<ProjectDepartment>> GetProjectDepartmentDetailById(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT d.DepartmentName, ww.YearMonth, ww.SumHour FROM Department d");
                sqlBuilder.Append(" LEFT JOIN (");
                sqlBuilder.Append(" SELECT aue.Department, CONVERT(varchar(7), wt.BeginTime, 120) AS YearMonth, SUM(wt.Hours) SumHour FROM auth_UserExtend aue, WorkTime wt");
                sqlBuilder.Append(" WHERE aue.UserId = wt.EmployeeId AND ProjectId = @ProjectId");
                sqlBuilder.Append(" GROUP BY CONVERT(varchar(7), wt.BeginTime, 120), aue.Department");
                sqlBuilder.Append(" ) ww ON ww.Department = d.DepartmentId");
                sqlBuilder.Append(" WHERE d.ParentId IS NOT NULL");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();
                List<ProjectDepartment> datas = dba.ExecuteReader<List<ProjectDepartment>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new ProjectDepartment
                        {
                            DepartmentName = r.GetStringOrNull("DepartmentName"),
                            YearMonth = r.GetStringOrNull("YearMonth"),
                            SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : r.GetDoubleOrDefault("SumHour").Value
                        });
                    });

                List<List<ProjectDepartment>> results = new List<List<ProjectDepartment>>();
                int length = datas.Count;
                if (length == 0)
                {
                }
                if (length > 0)
                {
                    List<ProjectDepartment> result = new List<ProjectDepartment>();
                    result.Add(datas[0]);
                    results.Add(result);
                    for (int i = 0; i < length - 1; i++)
                    {
                        if (datas[i + 1].DepartmentName != datas[i].DepartmentName)
                        {
                            result = new List<ProjectDepartment>();
                            results.Add(result);
                        }
                        result.Add(datas[i + 1]);
                    }
                }

                List<string> times = GetStatisticTimes(projectId);
                int count = times.Count;
                int len1 = results.Count;
                List<ProjectDepartment> rs = null;
                List<List<ProjectDepartment>> _results = new List<List<ProjectDepartment>>();
                List<ProjectDepartment> _rs = null;
                for (int i = 0; i < len1; i++)
                {
                    rs = results[i];
                    int len2 = rs.Count;
                    _rs = new List<ProjectDepartment>();
                    int index = 0;
                    for (int j = 0; j < count; j++)
                    {
                        if (index < len2)
                        {
                            if (times[j] != rs[index].YearMonth)
                            {
                                ProjectDepartment pd = new ProjectDepartment();
                                pd.DepartmentName = rs[0].DepartmentName;
                                pd.YearMonth = times[j];
                                pd.SumHour = 0;
                                _rs.Add(pd);
                            }
                            else
                            {
                                _rs.Add(rs[index]);
                                index++;
                            }
                        }
                        else
                        {
                            ProjectDepartment pd = new ProjectDepartment();
                            pd.DepartmentName = rs[0].DepartmentName;
                            pd.YearMonth = times[j];
                            pd.SumHour = 0;
                            _rs.Add(pd);
                        }
                    }
                    _results.Add(_rs);
                }
                return _results;
            }
        }

        //获取某个项目的 计划时间 和 实际时间
        public ProjectTime GetProjectTime(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT CONVERT(varchar(10), Min(p.BeginTime), 120) AS PBeginTime, CONVERT(varchar(10), MAX(p.EndTime), 120) AS PEndTime,");
                sqlBuilder.Append(" CONVERT(varchar(10), Min(wt.BeginTime), 120) AS RBeginTime, CONVERT(varchar(10), MAX(wt.EndTime), 120) AS REndTime FROM Project p");
                sqlBuilder.Append(" LEFT JOIN WorkTime wt ON wt.ProjectId = p.ProjectId");
                sqlBuilder.Append(" WHERE p.ProjectId = @ProjectId");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();

                ProjectTime result = null;
                using (DbDataReader r = dba.ExecuteReader(cmd))
                {
                    if (r.Read())
                    {
                        result = new ProjectTime
                        {
                            PBeginTime = r.GetStringOrNull("PBeginTime"),
                            PEndTime = r.GetStringOrNull("PEndTime") == null ? String.Format("{0:yyyy-MM-dd}", DateTime.Now) : r.GetStringOrNull("PEndTime"),
                            RBeginTime = r.GetStringOrNull("RBeginTime") == null ? String.Format("{0:yyyy-MM-dd}", DateTime.Now) : r.GetStringOrNull("RBeginTime"),
                            REndTime = r.GetStringOrNull("REndTime") == null ? String.Format("{0:yyyy-MM-dd}", DateTime.Now) : r.GetStringOrNull("REndTime")
                        };
                    }
                }
                return result;
            }
        }

        public List<string> GetStatisticTimes(Guid projectId)
        {
            ProjectTime ptime = GetProjectTime(projectId);
            string beginTime = ptime.PBeginTime;
            string endTime = String.Compare(ptime.PEndTime, ptime.REndTime) > 0 ? ptime.PEndTime : ptime.REndTime;
            string[] beginArray = beginTime.Split('-');
            string[] endArray = endTime.Split('-');
            int beginYear = int.Parse(beginArray[0]);
            int endYear = int.Parse(endArray[0]);
            int beginMonth = int.Parse(beginArray[1]);
            int endMonth = int.Parse(endArray[1]);
            List<string> results = new List<string>();
            string yearMonth = null;

            if (beginYear == endYear)
            {
                for (int month = beginMonth; month <= endMonth; month++)
                {
                    if (month < 10)
                    {
                        yearMonth = beginYear.ToString() + "-0" + month.ToString();
                    }
                    else
                    {
                        yearMonth = beginYear.ToString() + "-" + month.ToString();
                    }
                    results.Add(yearMonth);
                }
            }
            else if (beginYear < endYear)
            {
                //开始年份
                for (int month = beginMonth; month <= 12; month++)
                {
                    if (month < 10)
                    {
                        yearMonth = beginYear.ToString() + "-0" + month.ToString();
                    }
                    else
                    {
                        yearMonth = beginYear.ToString() + "-" + month.ToString();
                    }
                    results.Add(yearMonth);
                }
                //中间年份
                while ((beginYear + 1) < endYear)
                {
                    beginYear = beginYear + 1;
                    for (int month = 1; month <= 12; month++)
                    {
                        if (month < 10)
                        {
                            yearMonth = beginYear.ToString() + "-0" + month.ToString();
                        }
                        else
                        {
                            yearMonth = beginYear.ToString() + "-" + month.ToString();
                        }
                        results.Add(yearMonth);
                    }
                }
                //结束年份
                for (int month = 1; month <= endMonth; month++)
                {
                    if (month < 10)
                    {
                        yearMonth = endYear.ToString() + "-0" + month.ToString();
                    }
                    else
                    {
                        yearMonth = endYear.ToString() + "-" + month.ToString();
                    }
                    results.Add(yearMonth);
                }
            }
            return results;
        }


        //按成员统计项目
        public List<Object> GetProjectEmployeeDetailById(Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("");
                // 添加SQL语句
                sqlBuilder.Append("SELECT t.DepartmentId,t.DepartmentName,t.EmployeeId,t.RealName,SUM(HoursSum) AS HoursSum, ");
                sqlBuilder.Append("  SUM(t.DayValue) AS CostValue,MIN(t.BeginTime) AS BeginTime,MAX(EndTime) AS EndTime ");
                sqlBuilder.Append("FROM ( ");
                sqlBuilder.Append("  SELECT wt.EmployeeId,ue.RealName,wt.BeginTime,wt.EndTime,wt.HoursSum,ISNULL(lc.UnitValue,0) AS UnitValue, ");
                sqlBuilder.Append("    wt.HoursSum*ISNULL(lc.UnitValue,0)/8 AS DayValue,wt.StageId,s.StageName,d.DepartmentId,d.DepartmentName ");
                sqlBuilder.Append("  FROM (");
                sqlBuilder.Append("    SELECT pt.EmployeeId,wt1.StageId,wt1.BeginTime,wt1.EndTime,wt1.HoursSum FROM ( ");
                sqlBuilder.Append("      SELECT EmployeeId FROM ProjectTeam WHERE ProjectId = @ProjectId ");
                sqlBuilder.Append("      UNION ");
                sqlBuilder.Append("      SELECT EmployeeId FROM WorkTime WHERE ProjectId = @ProjectId GROUP BY EmployeeId ");
                sqlBuilder.Append("    ) pt ");
                sqlBuilder.Append("    LEFT JOIN ( ");
                sqlBuilder.Append("      SELECT SUM(Hours) AS HoursSum,EmployeeId,StageId,MIN(BeginTime) AS BeginTime,MAX(EndTime) AS EndTime ");
                sqlBuilder.Append("        FROM WorkTime WHERE ProjectId = @ProjectId GROUP BY EmployeeId,StageId ");
                sqlBuilder.Append("    ) wt1 ON wt1.EmployeeId = pt.EmployeeId ");
                sqlBuilder.Append("  ) wt ");
                sqlBuilder.Append("  LEFT JOIN Stage s ON s.StageId = wt.StageId ");
                sqlBuilder.Append("  LEFT JOIN auth_UserExtend ue ON ue.UserId = wt.EmployeeId ");
                sqlBuilder.Append("  LEFT JOIN Department d ON d.DepartmentId = ue.Department ");
                sqlBuilder.Append("  LEFT JOIN LaborCost lc ON lc.TargetId = ue.[Position] ");
                sqlBuilder.Append(") t ");
                sqlBuilder.Append("GROUP BY t.EmployeeId,t.RealName,t.DepartmentId,t.DepartmentName ");
                sqlBuilder.Append("ORDER BY t.DepartmentName,t.RealName ");
                // 添加参数
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                cmd.CommandText = sqlBuilder.ToString();
                List<Object> data = dba.ExecuteReader<List<Object>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new
                        {
                            DepartmentId = r.GetGuidOrNull("DepartmentId"),
                            DepartmentName = r.GetStringOrNull("DepartmentName"),
                            EmployeeId = r.GetGuidOrNull("EmployeeId"),
                            RealName = r.GetStringOrNull("RealName"),
                            SumHour = r.GetDoubleOrDefault("HoursSum") == null ? 0 : r.GetDoubleOrDefault("HoursSum").Value,
                            SumValue = r.GetDoubleOrDefault("CostValue") == null ? 0 : r.GetDoubleOrDefault("CostValue").Value,
                            BeginTime = r.GetStringOrNull("BeginTime") == null ? "" : r.GetStringOrNull("BeginTime"),
                            EndTime = r.GetStringOrNull("EndTime") == null ? "" : r.GetStringOrNull("EndTime"),
                            IsInProjectTeam = CheckIsInProjectTeam(projectId, r.GetGuidOrNull("EmployeeId").Value)
                        });
                    });
                return data;
            }
        }


        //判断某个成员是否属于某个项目组 0:不存在 1:存在
        public int CheckIsInProjectTeam(Guid projectId, Guid employeeId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT EmployeeId FROM ProjectTeam WHERE ProjectId = @ProjectId AND EmployeeId = @EmployeeId");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
                cmd.CommandText = sqlBuilder.ToString();

                int result = 0;
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        result = reader.GetGuidOrNull("EmployeeId") == null ? 0 : 1;
                    }
                }
                return result;
            }
        }

        #endregion

        #region MemberStatic

        //成员统计-获取成员在某一时间段的总共耗时
        public Double GetEmployeeTotalHour(DateTime? beginTime, DateTime? endTime)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT SUM(emps.SumHour) AS TotalHour");
                sqlBuilder.Append(" FROM (");
                sqlBuilder.Append(" SELECT wt.EmployeeId, SUM(wt.Hours) AS SumHour FROM WorkTime wt WHERE 1 = 1");
                if (beginTime != null)
                {
                    sqlBuilder.Append(" AND wt.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, beginTime);
                }
                if (endTime != null)
                {
                    sqlBuilder.Append(" AND wt.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, endTime);
                }
                sqlBuilder.Append(" GROUP BY wt.EmployeeId");
                sqlBuilder.Append(" ) emps");
                cmd.CommandText = sqlBuilder.ToString();

                Double result = 0;
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        result = reader.GetDoubleOrDefault("TotalHour") == null ? 0 : reader.GetDoubleOrDefault("TotalHour").Value;
                    }
                }
                return result;
            }
        }

        //成员统计-统计图
        public List<List<EmployeeStatic>> StatisticEmployeeForChart(string status, DateTime? beginTime, DateTime? endTime)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();

                StringBuilder sql = new StringBuilder("SELECT aue.UserId, aue.RealName, wt.SumHour, aue.status, dict1.DictItemValue AS StatusName, wt.AttendanceType, dict2.DictItemValue AS AttendanceName, aue.DepartmentName FROM auth_UserExtend aue");
                sql.Append(" LEFT JOIN (");
                sql.Append(" SELECT w.EmployeeId, w.AttendanceType, SUM(w.Hours) AS SumHour");
                sql.Append(" FROM WorkTime w WHERE 1 = 1");
                if (beginTime != null)
                {
                    sql.Append(" AND w.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, beginTime);
                }
                if (endTime != null)
                {
                    sql.Append(" AND w.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, endTime);
                }
                sql.Append(" GROUP BY w.EmployeeId, w.AttendanceType");
                sql.Append(" ) wt ON wt.EmployeeId = aue.UserId");
                sql.Append(" LEFT JOIN sys_DictionaryItem dict1 ON dict1.DictItemId = aue.Status");
                sql.Append(" LEFT JOIN sys_DictionaryItem dict2 ON dict2.DictItemId = wt.AttendanceType");
                sql.Append(" WHERE aue.Status IS NOT NULL");
                if (!String.IsNullOrEmpty(status))
                {
                    sql.Append(" AND aue.Status = @Status");
                    dba.AddInParameter(cmd, "Status", DbType.String, status);
                }
                sql.Append(" ORDER BY aue.RealName, aue.UserId");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                List<EmployeeStatic> datas = dba.ExecuteReader<List<EmployeeStatic>>(cmd, (d, r) =>
                {
                    d.Add(new EmployeeStatic
                    {

                        //EmployeeId = r.GetGuidOrNull("UserId").Value,
                        EmployeeName = r.GetStringOrNull("RealName"),
                        SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : r.GetDoubleOrDefault("SumHour").Value,
                        AttendanceName = r.GetStringOrNull("AttendanceName")
                    });
                });

                List<List<EmployeeStatic>> results = new List<List<EmployeeStatic>>();
                if (datas.Count > 0)
                {
                    int length = datas.Count;
                    List<EmployeeStatic> result = new List<EmployeeStatic>();
                    results.Add(result);
                    result.Add(datas[0]);
                    for (int i = 0; i < length - 1; i++)
                    {
                        if (datas[i + 1].EmployeeName != datas[i].EmployeeName)
                        {
                            result = new List<EmployeeStatic>();
                            results.Add(result);
                        }
                        result.Add(datas[i + 1]);
                    }
                }

                DictionaryLogic dictLogic = new DictionaryLogic();
                List<DictionaryItem> attendanceList = dictLogic.GetDictionaryItems(DictionaryLogic.AttendanceType);
                List<List<EmployeeStatic>> _results = new List<List<EmployeeStatic>>();
                List<EmployeeStatic> _resu = null;
                int len = results.Count;
                List<EmployeeStatic> resu = null;
                for (int i = 0; i < len; i++)
                {
                    resu = results[i];
                    int count = attendanceList.Count;
                    _resu = new List<EmployeeStatic>();
                    for (int j = 0; j < count; j++)
                    {
                        int size = resu.Count;
                        bool flag = false;
                        for (int k = 0; k < size; k++)
                        {
                            if (attendanceList[j].DictItemValue == resu[k].AttendanceName)
                            {
                                flag = true;
                                _resu.Add(resu[k]);
                                break;
                            }
                        }
                        if (!flag)
                        {
                            EmployeeStatic es = new EmployeeStatic();
                            es.EmployeeName = resu[0].EmployeeName;
                            es.SumHour = 0;
                            es.AttendanceName = attendanceList[j].DictItemValue;
                            _resu.Add(es);
                        }

                    }
                    _results.Add(_resu);
                }

                return _results;
            }
        }

        //成员统计-列表
        public List<EmployeeStatic> StatisticEmployeeForGrid(string status, DateTime? beginTime, DateTime? endTime)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();

                StringBuilder sql = new StringBuilder("SELECT aue.UserId, aue.RealName, wt.SumHour, (wt.SumHour * (lc.UnitValue / 8)) AS SumValue, aue.status, dict1.DictItemValue AS StatusName, aue.DepartmentName FROM auth_UserExtend aue");
                sql.Append(" LEFT JOIN (");
                sql.Append(" SELECT w.EmployeeId, SUM(w.Hours) AS SumHour");
                sql.Append(" FROM WorkTime w WHERE 1 = 1");
                if (beginTime != null)
                {
                    sql.Append(" AND w.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, beginTime);
                }
                if (endTime != null)
                {
                    sql.Append(" AND w.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, endTime);
                }
                sql.Append(" GROUP BY w.EmployeeId");
                sql.Append(" ) wt ON wt.EmployeeId = aue.UserId");
                sql.Append(" LEFT JOIN sys_DictionaryItem dict1 ON dict1.DictItemId = aue.Status");
                sql.Append(" LEFT JOIN LaborCost lc ON lc.TargetId = aue.Position");
                sql.Append(" WHERE aue.Status IS NOT NULL");
                if (!String.IsNullOrEmpty(status))
                {
                    sql.Append(" AND aue.Status = @Status");
                    dba.AddInParameter(cmd, "Status", DbType.String, status);
                }
                sql.Append(" ORDER BY aue.RealName, aue.UserId");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;

                Double totalHour = GetEmployeeTotalHour(beginTime, endTime);
                List<EmployeeStatic> results = dba.ExecuteReader<List<EmployeeStatic>>(cmd, (d, r) =>
                {
                    d.Add(new EmployeeStatic
                    {

                        EmployeeId = r.GetGuidOrNull("UserId").Value,
                        EmployeeName = r.GetStringOrNull("RealName"),
                        SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : r.GetDoubleOrDefault("SumHour").Value,
                        SumValue = r.GetDoubleOrDefault("SumValue") == null ? 0 : r.GetDoubleOrDefault("SumValue").Value,
                        TotalHour = totalHour,
                        StatusName = r.GetStringOrNull("StatusName"),
                        DepartmentName = r.GetStringOrNull("DepartmentName")
                    });
                });

                return results;
            }
        }



        //成员详细统计-根据成员ID获取成员信息
        public EmployeeModel getEmployeeNameById(Guid employeeId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT aue.UserId AS EmployeeId, aue.RealName FROM auth_UserExtend aue WHERE aue.UserId = @EmployeeId");
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
                cmd.CommandText = sqlBuilder.ToString();

                EmployeeModel result = null;
                using (DbDataReader r = dba.ExecuteReader(cmd))
                {
                    if (r.Read())
                    {
                        result = new EmployeeModel
                        {
                            EmployeeId = r.GetGuidOrNull("EmployeeId").Value,
                            RealName = r.GetStringOrNull("RealName")
                        };
                    }
                }
                return result;
            }
        }

        //成员详细统计-根据成员ID获取该成员的总耗时
        public Double getEmployeeTotalHour(Guid employeeId, DateTime? beginTime, DateTime? endTime)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT w.EmployeeId, SUM(w.Hours) AS TotalHour");
                sqlBuilder.Append(" FROM WorkTime w WHERE w.EmployeeId = @EmployeeId");
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
                if (beginTime != null)
                {
                    sqlBuilder.Append(" AND w.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, beginTime);
                }
                if (endTime != null)
                {
                    sqlBuilder.Append(" AND w.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, endTime);
                }
                sqlBuilder.Append(" GROUP BY w.EmployeeId");
                cmd.CommandText = sqlBuilder.ToString();

                Double result = 0;
                using (DbDataReader reader = dba.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        result = reader.GetDoubleOrDefault("TotalHour") == null ? 0 : reader.GetDoubleOrDefault("TotalHour").Value;
                    }
                }
                return result;
            }
        }

        //成员详细统计-根据成员ID参与的每个项目的用时-统计图
        public List<List<EmployeeDetailStatic>> StatisticEmployeeDetailForChart(Guid employeeId, DateTime? beginTime, DateTime? endTime)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();

                StringBuilder sql = new StringBuilder("SELECT projects.ProjectId, p.ProjectName, projects.YearMonth, projects.SumHour");
                sql.Append(" FROM (");
                sql.Append(" SELECT wt.YearMonth, wt.ProjectId, SUM(wt.Hours) AS SumHour");
                sql.Append(" FROM (");
                sql.Append(" SELECT CONVERT(nvarchar(7), EndTime, 120) AS YearMonth, w.Hours, w.ProjectId FROM WorkTime w");
                sql.Append(" WHERE w.EmployeeId = @EmployeeId");
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
                if (beginTime != null)
                {
                    sql.Append(" AND w.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, beginTime);
                }
                if (endTime != null)
                {
                    sql.Append(" AND w.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, endTime);
                }
                sql.Append(" ) wt GROUP BY wt.YearMonth, wt.ProjectId");
                sql.Append(" ) projects");
                sql.Append(" INNER JOIN Project p ON projects.ProjectId = p.ProjectId");
                sql.Append(" ORDER BY projects.YearMonth");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                List<EmployeeDetailStatic> datas = dba.ExecuteReader<List<EmployeeDetailStatic>>(cmd, (d, r) =>
                {
                    d.Add(new EmployeeDetailStatic
                    {
                        ProjectName = r.GetStringOrNull("ProjectName"),
                        SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : r.GetDoubleOrDefault("SumHour").Value,
                        YearMonth = r.GetStringOrNull("YearMonth")
                    });
                });

                List<List<EmployeeDetailStatic>> results = new List<List<EmployeeDetailStatic>>();
                if (datas.Count > 0)
                {
                    int length = datas.Count;
                    List<EmployeeDetailStatic> result = new List<EmployeeDetailStatic>();
                    results.Add(result);
                    result.Add(datas[0]);
                    for (int i = 0; i < length - 1; i++)
                    {
                        if (datas[i + 1].YearMonth != datas[i].YearMonth)
                        {
                            result = new List<EmployeeDetailStatic>();
                            results.Add(result);
                        }
                        result.Add(datas[i + 1]);
                    }
                }

                List<EmployeeDetailStatic> projects = StatisticEmployeeDetailForGrid(employeeId, beginTime, endTime);

                List<List<EmployeeDetailStatic>> _results = new List<List<EmployeeDetailStatic>>();
                List<EmployeeDetailStatic> _resu = null;
                int len = results.Count;
                List<EmployeeDetailStatic> resu = null;
                for (int i = 0; i < len; i++)
                {
                    resu = results[i];
                    int count = projects.Count;
                    _resu = new List<EmployeeDetailStatic>();
                    for (int j = 0; j < count; j++)
                    {
                        int size = resu.Count;
                        bool flag = false;
                        for (int k = 0; k < size; k++)
                        {
                            if (projects[j].ProjectName == resu[k].ProjectName)
                            {
                                flag = true;
                                _resu.Add(resu[k]);
                                break;
                            }
                        }
                        if (!flag)
                        {
                            EmployeeDetailStatic es = new EmployeeDetailStatic();
                            es.ProjectName = projects[j].ProjectName;
                            es.YearMonth = resu[0].YearMonth;
                            es.SumHour = 0;
                            _resu.Add(es);
                        }

                    }
                    _results.Add(_resu);
                }

                List<string> times = GetAllMonth(beginTime, endTime);
                int _count = times.Count;
                int leng = _results.Count;
                List<List<EmployeeDetailStatic>> _results_ = new List<List<EmployeeDetailStatic>>();
                for (int i = 0; i < _count; i++)
                {
                    bool flag = false;
                    for (int j = 0; j < leng; j++)
                    {
                        if (_results[j][0].YearMonth == times[i])
                        {
                            _results_.Add(_results[j]);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        List<EmployeeDetailStatic> _rs = GetOtherMonthData(employeeId, beginTime, endTime, times[i]);
                        _results_.Add(_rs);
                    }
                }

                return _results_;
            }
        }

        private List<EmployeeDetailStatic> GetOtherMonthData(Guid employeeId, DateTime? beginTime, DateTime? endTime, string yearMonth)
        {
            List<EmployeeDetailStatic> projects = StatisticEmployeeDetailForGrid(employeeId, beginTime, endTime);
            int length = projects.Count;
            for (int i = 0; i < length; i++)
            {
                projects[i].SumHour = 0;
                projects[i].YearMonth = yearMonth;
            }

            if (projects == null || projects.Count == 0)
            {
                EmployeeDetailStatic eds = new EmployeeDetailStatic();
                eds.SumHour = 0;
                eds.YearMonth = yearMonth;
                eds.ProjectName = "无";
                projects.Add(eds);
            }
            return projects;
        }

        //成员详细统计-根据成员ID参与的每个项目的用时以及成本-列表
        public List<EmployeeDetailStatic> StatisticEmployeeDetailForGrid(Guid employeeId, DateTime? beginTime, DateTime? endTime)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();

                StringBuilder sql = new StringBuilder("SELECT wt.EmployeeId, p.ProjectName, wt.SumHour, (wt.SumHour * (lc.UnitValue / 8)) AS SumValue FROM Project p");
                sql.Append(" INNER JOIN (");
                sql.Append(" SELECT w.EmployeeId, w.ProjectId, SUM(w.Hours) AS SumHour");
                sql.Append(" FROM WorkTime w WHERE w.EmployeeId = @EmployeeId");
                dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);

                if (beginTime != null)
                {
                    sql.Append(" AND w.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, beginTime);
                }
                if (endTime != null)
                {
                    sql.Append(" AND w.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, endTime);
                }
                sql.Append(" GROUP BY w.EmployeeId, w.ProjectId");
                sql.Append(" ) wt ON wt.ProjectId = p.ProjectId");


                sql.Append(" INNER JOIN auth_UserExtend aue ON aue.UserId = wt.EmployeeId");
                sql.Append(" INNER JOIN LaborCost lc ON lc.TargetId = aue.Position");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;

                Double totalHour = getEmployeeTotalHour(employeeId, beginTime, endTime);
                List<EmployeeDetailStatic> results = dba.ExecuteReader<List<EmployeeDetailStatic>>(cmd, (d, r) =>
                {
                    d.Add(new EmployeeDetailStatic
                    {
                        ProjectName = r.GetStringOrNull("ProjectName"),
                        SumHour = r.GetDoubleOrDefault("SumHour") == null ? 0 : r.GetDoubleOrDefault("SumHour").Value,
                        SumValue = r.GetDoubleOrDefault("SumValue") == null ? 0 : r.GetDoubleOrDefault("SumValue").Value,
                        TotalHour = totalHour
                    });
                });

                return results;
            }
        }


        //获取某个时间段的所有日期
        /*private DateTime[] GetAllDays(DateTime startDate, DateTime endDate)
        {
            List<DateTime> listDays = new List<DateTime>();
            DateTime dtDay = new DateTime();
            for (dtDay = startDate; dtDay.CompareTo(endDate) <= 0; dtDay = dtDay.AddDays(1))
            {
                listDays.Add(dtDay);
            }
            return listDays.ToArray();
        }*/

        //获取某个时间段的所有月份
        private List<string> GetAllMonth(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null)
            {
                startDate = DateTime.Now;
            }

            List<DateTime> listMonths = new List<DateTime>();
            DateTime dtMonth = new DateTime();
            for (dtMonth = startDate.Value; dtMonth.CompareTo(endDate) <= 0; dtMonth = dtMonth.AddMonths(1))
            {
                listMonths.Add(dtMonth);
            }

            List<string> listMonthArray = new List<string>();
            int length = listMonths.Count;
            for (int i = 0; i < length; i++)
            {
                listMonthArray.Add(String.Format("{0:yyyy-MM}", listMonths[i]));
            }

            return listMonthArray;
        }


        #endregion

        #region workloadStatistic

        public List<Object> getWorkloadForYear(DateTime start, DateTime end, int length, String status, String type)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT w.DateStr, p.ProjectId, p.ProjectName, ISNULL(SUM([Hours]),0) AS SumHour FROM");
                sql.Append(" (SELECT * FROM Project WHERE ParentId IS NOT NULL");
                if (!String.IsNullOrEmpty(status) && !status.Equals("null"))
                {
                    sql.Append(" AND [Status] = @status");
                    dba.AddInParameter(cmd, "status", status);
                }
                if (!String.IsNullOrEmpty(type) && !type.Equals("null"))
                {
                    sql.Append(" AND ProjectType =@projectTpye");
                    dba.AddInParameter(cmd, "projectTpye", type);
                }
                sql.Append(" ) p");

                sql.Append(" LEFT JOIN (SELECT [Hours], ProjectId, CONVERT(nvarchar(" + length + "), BeginTime, 120) AS DateStr FROM WorkTime");
                sql.Append(" WHERE BeginTime >= @startTime AND EndTime <= @endTime) w ON p.ProjectId = w.ProjectId");
                sql.Append(" GROUP BY w.DateStr, p.ProjectId, p.ProjectName");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                dba.AddInParameter(cmd, "startTime", start);
                dba.AddInParameter(cmd, "endTime", end);
                List<Object> datas = dba.ExecuteReader<List<Object>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new
                        {
                            DateStr = r.GetStringOrNull("DateStr"),
                            ProjectId = (Guid)r["ProjectId"],
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            SumHour = r.GetDecimalOrDefault("SumHour")
                        });
                    });
                return datas;
            }
        }

        public List<Object> getWorkloadForMonth(DateTime start, DateTime end)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT pd.ProjectId, pd.ProjectName, pd.DateStr, ISNULL(wg.SumHour, 0) AS SumHour FROM (");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                sql.Append(" ");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                dba.AddInParameter(cmd, "startTime", start);
                dba.AddInParameter(cmd, "endTime", end);
                List<Object> datas = dba.ExecuteReader<List<Object>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new
                        {
                            DateStr = r.GetStringOrNull("DateStr"),
                            ProjectId = (Guid)r["ProjectId"],
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            SumHour = r.GetDecimalOrDefault("SumHour")
                        });
                    });
                return datas;
            }
        }

        public List<Object> getWorkloadForDay(DateTime start, DateTime end, Guid projectId)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT du.DateStr, s.EmployeeId, s.RealName, ISNULL(wg.SumHour, 0) AS SumHour, isnull(wg1.totalhour, 0) AS totalHour FROM");
                sql.Append(" (SELECT e.*, us.RealName FROM");
                sql.Append(" (SELECT * FROM ");
                sql.Append(" (SELECT EmployeeId, ProjectId FROM WorkTime");
                sql.Append(" WHERE BeginTime >= @startTime AND EndTime <= @endTime AND ProjectId = @projectId ) h1 ");
                sql.Append(" GROUP BY h1.EmployeeId, ProjectId");
                sql.Append(" union");
                sql.Append(" SELECT EmployeeId, ProjectId FROM ProjectTeam WHERE ProjectId = @projectId");
                sql.Append(" ) e");
                sql.Append(" LEFT JOIN auth_UserExtend us ON e.EmployeeId = us.UserId) s");
                sql.Append(" LEFT JOIN (SELECT h.DateStr FROM");
                sql.Append(" (SELECT CONVERT(nvarchar(10), BeginTime, 120) AS DateStr FROM WorkTime");
                sql.Append(" WHERE BeginTime >= @startTime  AND EndTime <= @endTime ) h GROUP BY DateStr) du");
                sql.Append(" ON s.RealName <> du.DateStr");
                sql.Append(" LEFT JOIN (SELECT h1.DateStr, h1.EmployeeId, SUM([Hours]) AS SumHour FROM");
                sql.Append(" (SELECT [Hours], EmployeeId, CONVERT(nvarchar(10), BeginTime, 120) AS DateStr FROM WorkTime");
                sql.Append(" WHERE BeginTime >= @startTime  AND EndTime <= @endTime AND ProjectId = @projectId) h1 GROUP BY h1.DateStr, h1.EmployeeId) wg");
                sql.Append(" ON du.DateStr = wg.DateStr AND s.EmployeeId = wg.EmployeeId");
                sql.Append(" LEFT JOIN (SELECT h2.DateStr, h2.EmployeeId, SUM([Hours]) AS TotalHour FROM");
                sql.Append(" (SELECT [Hours], EmployeeId, CONVERT(nvarchar(10), BeginTime, 120) AS DateStr FROM WorkTime");
                sql.Append(" WHERE BeginTime >= @startTime AND EndTime <= @endTime ) h2 GROUP BY h2.DateStr, h2.EmployeeId) wg1");
                sql.Append(" ON du.DateStr = wg1.DateStr AND s.EmployeeId = wg1.EmployeeId");
                sql.Append(" WHERE du.DateStr IS NOT NULL");
                sql.Append(" ORDER BY du.DateStr, s.RealName");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                dba.AddInParameter(cmd, "startTime", start);
                dba.AddInParameter(cmd, "endTime", end);
                if (Guid.Empty != projectId)
                {
                    dba.AddInParameter(cmd, "projectId", projectId);
                }
                List<Object> records = dba.ExecuteReader<List<Object>>(
                cmd, (d, r) =>
                {
                    d.Add(new
                    {
                        DateStr = r.GetStringOrNull("DateStr"),
                        EmployeeId = (Guid)r["EmployeeId"],
                        EmployeeName = r.GetStringOrNull("RealName"),
                        SumHour = r.GetDecimalOrDefault("SumHour"),
                        TotalHour = r.GetDecimalOrDefault("TotalHour")
                    });
                });
                return records;
            }
        }

        public List<Object> getWorkRecordByProjectId(Guid empId, Guid projectId, DateTime start, DateTime end)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT BeginTime, EndTime, JobContent FROM WorkTime WHERE EmployeeId = @empId AND BeginTime >=@start AND EndTime <= @end AND ProjectId = @projectId ORDER BY BeginTime ");
                dba.AddInParameter(cmd, "empId", empId);
                dba.AddInParameter(cmd, "start", start);
                dba.AddInParameter(cmd, "end", end);
                dba.AddInParameter(cmd, "projectId", projectId);
                List<Object> records = dba.ExecuteReader<List<Object>>(
                cmd, (d, r) =>
                {
                    d.Add(new
                    {
                        BeginTime = r.GetDateTimeOrDefault("BeginTime"),
                        EndTime = r.GetDateTimeOrDefault("EndTime"),
                        JobContent = r.GetStringOrNull("JobContent")
                    });
                });
                return records;
            }
        }
        #endregion
    }
}
