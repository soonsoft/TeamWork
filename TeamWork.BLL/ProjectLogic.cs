using STIM.Core;
using STIM.Data.EntityFrameworkExtensions;
using STIM.Modules;
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
    /// 项目相关的业务逻辑
    /// </summary>
    public class ProjectLogic
    {
        #region Project 项目

        /// <summary>
        /// 查询项目，对于子项目超过一个的返回主项目信息，只有一个子项目的直接返回子项目信息
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="projectType">项目类型</param>
        /// <param name="status">项目状态</param>
        /// <param name="beginTime">项目开始时间 查询开始</param>
        /// <param name="endTime">项目开始时间 查询结束</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="rowCount">总记录数</param>
        /// <returns></returns>
        public List<ProjectModel> QueryProjects(
            string projectName, 
            string projectType, 
            string status, 
            DateTime? beginTime, 
            DateTime? endTime,
            int pageIndex,
            int pageSize,
            out int rowCount)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();

                StringBuilder sqlBuilder = new StringBuilder("SELECT projects.*, ue.RealName AS PMName, di1.DictItemValue AS ProjectTypeName, di2.DictItemValue AS StatusName FROM (");
                sqlBuilder.Append("SELECT p.*, ChildrenCount FROM Project p ")
                    .Append("INNER JOIN (SELECT p.ParentId AS ProjectId, COUNT(1) AS ChildrenCount FROM Project p ")
                    .Append("GROUP BY p.ParentId HAVING p.ParentId IS NOT NULL) parent ON p.ProjectId = parent.ProjectId WHERE ChildrenCount > 1 ")
                    .Append("UNION ALL ")
                    .Append("SELECT p.*, 0 AS ChildrenCount FROM Project p ")
                    .Append("INNER JOIN (SELECT p.ParentId AS ProjectId, COUNT(1) AS ChildrenCount FROM Project p ")
                    .Append("GROUP BY p.ParentId HAVING p.ParentId IS NOT NULL) parent ")
                    .Append("ON p.ParentId = parent.ProjectId ")
                    .Append("WHERE ChildrenCount = 1) projects ")
                    .Append("LEFT JOIN auth_UserExtend ue ON projects.PM = ue.UserId ")
                    .Append("INNER JOIN sys_DictionaryItem di1 ON projects.ProjectType = di1.DictItemId ")
                    .Append("LEFT JOIN sys_DictionaryItem di2 ON projects.[Status] = di2.DictItemId WHERE 1 = 1");
                if(!String.IsNullOrEmpty(projectName))
                {
                    sqlBuilder.Append(" AND projects.ProjectName LIKE @ProjectName");
                    dba.AddInParameter(cmd, "ProjectName", DbType.String, String.Concat("%", projectName, "%"));
                }
                if(!String.IsNullOrEmpty(projectType))
                {
                    sqlBuilder.Append(" AND projects.ProjectType = @ProjectType");
                    dba.AddInParameter(cmd, "ProjectType", DbType.String, projectType);
                }
                if(!String.IsNullOrEmpty(status))
                {
                    sqlBuilder.Append(" AND projects.[Status] = @Status");
                    dba.AddInParameter(cmd, "Status", DbType.String, status);
                }
                if(beginTime != null)
                {
                    sqlBuilder.Append(" AND projects.BeginTime >= @BeginTime");
                    dba.AddInParameter(cmd, "BeginTime", DbType.DateTime, QueryParameterUtility.DateTimeStart(beginTime.Value));
                }
                if(endTime != null)
                {
                    sqlBuilder.Append(" AND projects.EndTime <= @EndTime");
                    dba.AddInParameter(cmd, "EndTime", DbType.DateTime, QueryParameterUtility.DateTimeEnd(endTime.Value));
                }
                sqlBuilder.Append(" ORDER BY projects.BeginTime DESC, projects.ProjectName ASC");
                cmd.CommandText = sqlBuilder.ToString();

                List<ProjectModel> data = dba.ExecuteReaderPaging<List<ProjectModel>>(
                    cmd, (d, r) => 
                    {
                        d.Add(new ProjectModel
                            {
                                ProjectId = Guid.Parse(r["ProjectId"].ToString()),
                                ProjectName = r.GetStringOrNull("ProjectName"),
                                ProjectType = r.GetStringOrNull("ProjectType"),
                                BeginTime = r.GetDateTimeOrDefault("BeginTime"),
                                EndTime = r.GetDateTimeOrDefault("EndTime"),
                                Status = r.GetStringOrNull("Status"),
                                PM = r.GetGuidOrNull("PM"),
                                ParentId = r.GetGuidOrNull("ParentId"),
                                Description = r.GetStringOrNull("Description"),
                                ChildrenCount = Convert.ToInt32(r["ChildrenCount"]),
                                PMName = r.GetStringOrNull("PMName"),
                                ProjectTypeName = r.GetStringOrNull("ProjectTypeName"),
                                StatusName = r.GetStringOrNull("StatusName")
                            });
                    }, pageIndex, pageSize, out rowCount);
                return data;
            }
        }

        public List<ProjectSelectItem> GetMasterProject()
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                var data = from p in context.Projects
                           where p.ParentId == null
                           orderby p.EndTime descending
                           select new ProjectSelectItem
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName
                           };
                return data.ToList();
            }
        }

        public List<ProjectModel> GetSubprojects(Guid projectId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT p.*, ue.RealName AS PMName, di1.DictItemValue AS ProjectTypeName, di2.DictItemValue AS StatusName FROM Project p LEFT JOIN auth_UserExtend ue ON p.PM = ue.UserId INNER JOIN sys_DictionaryItem di1 ON p.ProjectType = di1.DictItemId LEFT JOIN sys_DictionaryItem di2 ON p.[Status] = di2.DictItemId WHERE p.ParentId = @ParentId ORDER BY p.BeginTime ASC");
                dba.AddInParameter(cmd, "ParentId", DbType.Guid, projectId);
                List<ProjectModel> data = dba.ExecuteReader<List<ProjectModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new ProjectModel
                        {
                            ProjectId = Guid.Parse(r["ProjectId"].ToString()),
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            ProjectType = r.GetStringOrNull("ProjectType"),
                            BeginTime = r.GetDateTimeOrDefault("BeginTime"),
                            EndTime = r.GetDateTimeOrDefault("EndTime"),
                            Status = r.GetStringOrNull("Status"),
                            PM = r.GetGuidOrNull("PM"),
                            ParentId = r.GetGuidOrNull("ParentId"),
                            Description = r.GetStringOrNull("Description"),
                            ChildrenCount = 0,
                            PMName = r.GetStringOrNull("PMName"),
                            ProjectTypeName = r.GetStringOrNull("ProjectTypeName"),
                            StatusName = r.GetStringOrNull("StatusName")
                        });
                    });
                return data;
            }
        }

        public List<EmployeeSelectItem> GetTeamByProjectId(Guid projectId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT ue.* FROM [ProjectTeam] p INNER JOIN [auth_UserExtend] ue ON p.EmployeeId = ue.UserId WHERE ProjectId = @ProjectId AND ue.Status = '1-01'");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);

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
                            PositionName = r.GetStringOrNull("PositionName")
                        });
                    });
                return data;
            }
        }

        public List<BudgetModel> GetBudgetsByProjectId(Guid projectId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT b.*, di.DictItemValue AS BudgetCategoryName FROM [Budget] b INNER JOIN sys_DictionaryItem di ON b.BudgetCategory = di.DictItemId WHERE b.ProjectId = @ProjectId");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);

                List<BudgetModel> data = dba.ExecuteReader<List<BudgetModel>>(
                    cmd, (d, r) => 
                    {
                        d.Add(new BudgetModel
                            {
                                BudgetId = (Guid)r["BudgetId"],
                                ProjectId = (Guid)r["ProjectId"],
                                BudgetCategory = r.GetStringOrNull("BudgetCategory"),
                                BudgetCategoryName = r.GetStringOrNull("BudgetCategoryName"),
                                Capital = r.GetDecimalOrDefault("Capital"),
                                RDTime = r.GetDecimalOrDefault("RDTime")
                            });
                    });
                return data;
            }
        }

        public bool InsertProject(Project project, Guid[] teamEmployees = null, Budget[] budgets = null)
        {
            Guard.ArgumentNotNull(project, "project");

            project.ProjectId = Guid.NewGuid();
            int affectRows = 0;
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                using(var conn = dba.GetOpenConnection())
                {
                    if (project.ParentId == null)
                    {
                        Project parentProject = new Project
                        {
                            ProjectId = Guid.NewGuid(),
                            ProjectName = project.ProjectName,
                            ProjectType = project.ProjectType,
                            BeginTime = project.BeginTime,
                            EndTime = project.EndTime
                        };
                        project.ParentId = parentProject.ProjectId;
                        context.Projects.Add(parentProject);
                    }
                    else
                    {
                        if (!context.Projects.Any(p => p.ProjectId == project.ParentId))
                        {
                            throw new ModuleLogicException("所选择的主项目不存在");
                        }
                    }

                    context.Projects.Add(project);
                    if(!String.IsNullOrEmpty(project.ProjectType))
                    {
                        List<StageTemplete> stList = FindStageTemplete(project.ProjectType);
                        if(stList != null && stList.Count > 0)
                        {
                            List<Stage> stages = new List<Stage>(stList.Count);
                            foreach (var t in stList)
                            {
                                stages.Add(new Stage
                                    {
                                        StageId = Guid.NewGuid(),
                                        Project = project,
                                        Manager = project.PM,
                                        //创建状态
                                        Status = "103-00",
                                        StageName = t.StageName,
                                        Description = t.Description
                                    });
                            }
                            LinkStages(stages);
                            //激活状态
                            stages[0].Status = "103-01";
                            foreach(Stage s in stages)
                            {
                                context.Stages.Add(s);
                            }
                        }
                    }
                    if (budgets != null)
                    {
                        foreach(var b in budgets)
                        {
                            b.BudgetId = Guid.NewGuid();
                            b.Project = project;
                            context.Budgets.Add(b);
                        }
                    }
                    affectRows += context.SaveChanges();

                    if (teamEmployees != null && teamEmployees.Length > 0)
                    {
                        affectRows += InsertProjectTeam(dba, project.ProjectId, teamEmployees);
                    }
                    return affectRows > 0;
                }
            }
        }

        public bool UpdateProject(Project project, Guid[] teamEmployees = null, Budget[] budgets = null)
        {
            Guard.ArgumentNotNull(project, "project");

            int affectRows = 0;
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                using (var conn = dba.GetOpenConnection())
                {
                    Project old = context.Projects
                        .Where(p => p.ProjectId == project.ProjectId)
                        .SingleOrDefault();
                    if(old == null)
                    {
                        throw new ModuleLogicException("正在更新的项目信息不存在，或已被删除。");
                    }
                    old.ProjectName = project.ProjectName;
                    old.ProjectType = project.ProjectType;
                    old.BeginTime = project.BeginTime;
                    old.EndTime = project.EndTime;
                    old.Status = project.Status;
                    old.PM = project.PM;
                    old.ParentId = project.ParentId;

                    if(teamEmployees != null && teamEmployees.Length > 0)
                    {
                        DbCommand cmd = dba.CreateCommand("DELETE FROM [ProjectTeam] WHERE ProjectId = @ProjectId");
                        dba.AddInParameter(cmd, "ProjectId", DbType.Guid, project.ProjectId);
                        affectRows += dba.ExecuteNonQuery(cmd);

                        affectRows += InsertProjectTeam(dba, project.ProjectId, teamEmployees);
                    }

                    if (budgets != null)
                    {
                        DbCommand cmd = dba.CreateCommand("DELETE FROM [Budget] WHERE ProjectId = @ProjectId");
                        dba.AddInParameter(cmd, "ProjectId", DbType.Guid, old.ProjectId);
                        affectRows += dba.ExecuteNonQuery(cmd);

                        foreach(var b in budgets)
                        {
                            b.BudgetId = Guid.NewGuid();
                            b.Project = old;
                            context.Budgets.Add(b);
                        }
                    }

                    affectRows += context.SaveChanges();
                    return affectRows > 0;
                }
            }
        }

        int InsertProjectTeam(DatabaseAccess dba, Guid projectId, Guid[] teamEmployees)
        {
            IList<object> parameters = new List<object>(teamEmployees.Length);
            for (int i = 0; i < teamEmployees.Length; i++)
            {
                parameters.Add(new
                {
                    TeamId = Guid.NewGuid(),
                    ProjectId = projectId,
                    EmployeeId = teamEmployees[i]
                });
            }
            return dba.ExecuteNonQueryMultiple("INSERT INTO [ProjectTeam] VALUES(@TeamId, @ProjectId, @EmployeeId)", parameters);
        }

        void LinkStages(List<Stage> stages)
        {
            Stage current;
            for (int i = 0; i < stages.Count; i++)
            {
                current = stages[i];
                current.SortValue = i;
                if (i > 0)
                {
                    current.PrevStage = stages[i - 1].StageId;
                }
                if(i < stages.Count - 1)
                {
                    current.NextStage = stages[i + 1].StageId;
                }
            }
        }

        /// <summary>
        /// 获取用户所属的项目以及该项目当前激活的阶段
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public List<ProjectStageSelectItem> GetProjectStage(Guid? employeeId = null)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT p.ProjectId, p.ProjectName, s.StageId, s.StageName FROM Project p INNER JOIN Stage s ON p.ProjectId = s.ProjectId AND s.[Status] = '103-01' INNER JOIN ProjectTeam pt ON p.ProjectId = pt.ProjectId");
                if(employeeId != null)
                {
                    sqlBuilder.Append(" WHERE pt.EmployeeId = @EmployeeId");
                    dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, employeeId);
                }
                sqlBuilder.Append(" ORDER BY p.BeginTime DESC");
                cmd.CommandText = sqlBuilder.ToString();

                List<ProjectStageSelectItem> items = dba.ExecuteReader<List<ProjectStageSelectItem>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new ProjectStageSelectItem
                            {
                                ProjectId = (Guid)r["ProjectId"],
                                ProjectName = r["ProjectName"].ToString(),
                                StageId = (Guid)r["StageId"],
                                StageName = r["StageName"].ToString()
                            });
                    });
                return items;
            }
        }

        #endregion

        #region Stage 项目阶段

        public List<StageTemplete> FindStageTemplete(string projectType, DatabaseAccess dba = null)
        {
            Guard.ArgumentNotNullOrEmpty(projectType, "projectType");

            bool flag = false;
            TeamDBContext context = null;
            if (dba == null)
            {
                flag = true;
                context = new TeamDBContext();
                dba = context.DatabaseAccess();
            }
            try
            {
                DbCommand cmd = dba.CreateCommand("SELECT * FROM [StageTemplete] WHERE ProjectType = @ProjectType ORDER BY SortValue");
                dba.AddInParameter(cmd, "ProjectType", DbType.String, projectType);
                List<StageTemplete> data = dba.ExecuteReader<List<StageTemplete>>(
                    cmd,
                    (d, r) =>
                    {
                        d.Add(new StageTemplete
                        {
                            StageTempleteId = (Guid)r["StageTempleteId"],
                            ProjectType = r["ProjectType"].ToString(),
                            StageName = r.GetStringOrNull("StageName"),
                            Description = r.GetStringOrNull("Description")
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
        
        public List<StageModel> FindStageByProjectId(Guid projectId)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand("SELECT s.*, di.DictItemValue AS StatusName, ue.RealName AS ManagerName FROM [Stage] s INNER JOIN sys_DictionaryItem di ON s.[Status] = di.DictItemId LEFT JOIN auth_UserExtend ue ON s.Manager = ue.UserId WHERE s.ProjectId = @ProjectId ORDER BY SortValue");
                dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);

                List<StageModel> data = dba.ExecuteReader<List<StageModel>>(
                    cmd, (d, r) => 
                    {
                        d.Add(new StageModel 
                        {
                            StageId = (Guid)r["StageId"],
                            StageName = r.GetStringOrNull("StageName"),
                            PrevStage = r.GetGuidOrNull("PrevStage"),
                            NextStage = r.GetGuidOrNull("NextStage"),
                            Manager = r.GetGuidOrNull("Manager"),
                            ManagerName = r.GetStringOrNull("ManagerName"),
                            BeginTime = r.GetDateTimeOrDefault("BeginTime"),
                            EndTime = r.GetDateTimeOrDefault("EndTime"),
                            Status = r.GetStringOrNull("Status"),
                            StatusName = r.GetStringOrNull("StatusName"),
                            Description = r.GetStringOrNull("Description")
                        });
                    });
                return data;
            }
        }

        public bool UpdateProjectStages(Guid projectId, List<Stage> stages)
        {
            Guard.ArgumentNotNull(stages, "stages");
            if(stages.Count == 0)
            {
                return false;
            }
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                using(var conn = dba.GetOpenConnection())
                {
                    DbCommand cmd = dba.CreateCommand("DELETE FROM [Stage] WHERE ProjectId = @ProjectId");
                    dba.AddInParameter(cmd, "ProjectId", DbType.Guid, projectId);
                    dba.ExecuteNonQuery(cmd);

                    Project project = context.Projects
                        .Where(p => p.ProjectId == projectId)
                        .Single();
                    LinkStages(stages);
                    foreach(Stage s in stages)
                    {
                        s.Project = project;
                        context.Stages.Add(s);
                    }
                    int affectRows = context.SaveChanges();
                    return affectRows > 0;
                }
            }
        }

        #endregion

        #region StageTemplete 项目模板

        public List<StageTemplete> GetTempletes(string projectType)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT * FROM StageTemplete");
                sqlBuilder.Append(" WHERE ProjectType = @ProjectType")
                    .Append(" ORDER BY SortValue");
                dba.AddInParameter(cmd, "ProjectType", DbType.String, projectType);
                cmd.CommandText = sqlBuilder.ToString();
                List<StageTemplete> data = dba.ExecuteReader<List<StageTemplete>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new StageTemplete
                        {
                            StageTempleteId = (Guid)r["StageTempleteId"],
                            ProjectType = r.GetStringOrNull("ProjectType"),
                            StageName = r.GetStringOrNull("StageName"),
                            Description = r.GetStringOrNull("Description"),
                            SortValue = r.GetIntOrDefault("SortValue")
                        });
                    });
                return data;
            }
        }

        public object SaveTemplete(StageTemplete st, Guid?[] stageTempleteIds)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                Guid? id = null;
                int sortEffectRow = 0;
                int countChange = 0;
                DatabaseAccess dba = context.DatabaseAccess();
                
                if (st.StageTempleteId == Guid.Empty)
                {
                    int totalCount = context.StageTempletes.Count();
                    var maxSortValue = context.StageTempletes.Max(n => n.SortValue);
                    var results = (from n in context.StageTempletes
                                    where n.StageName == st.StageName
                                    where n.ProjectType == st.ProjectType
                                    select n);
                    if (results.Count() > 0)
                    {
                        return new { Result = false, ErrorMessage = String.Format("[{0}] 已存在，不可重复添加", st.StageName) };
                    }
                    st.StageTempleteId = Guid.NewGuid();
                    st.SortValue = maxSortValue + 1;
                    context.StageTempletes.Add(st);
                    id = st.StageTempleteId;
                }
                else
                {
                    StageTemplete oldst = context.StageTempletes.Where(n => n.StageTempleteId == st.StageTempleteId).Single();
                    if (oldst == null)
                    {
                        throw new Exception("不存在此阶段信息！");
                    }
                    oldst.StageName = st.StageName;
                    oldst.Description = st.Description;
                    id = st.StageTempleteId;
                }
                countChange = context.SaveChanges();

                using (var conn = dba.GetOpenConnection())
                {
                    if (stageTempleteIds != null && stageTempleteIds.Length > 0)
                    {
                        int length = stageTempleteIds.Length;
                        List<object> param = new List<object>(length);
                        for (int i = 0; i < length; i++)
                        {
                            param.Add(new { SortValue = i, StageTempleteId = stageTempleteIds[i] });
                        }
                        sortEffectRow = dba.ExecuteNonQueryMultiple("UPDATE StageTemplete SET SortValue = @SortValue WHERE StageTempleteId = @StageTempleteId", param);
                    }
                }

                return new { Result = (countChange > 0 || sortEffectRow > 0), ID = id };
            }
        }

        public Boolean DeleteTemplete(Guid? stageTempleteId)
        {
            bool flag = false;
            if (stageTempleteId != null)
            {
                using (TeamDBContext context = new TeamDBContext())
                {
                    DatabaseAccess dba = context.DatabaseAccess();
                    DbCommand cmd = dba.CreateCommand("DELETE FROM StageTemplete WHERE StageTempleteId=@StageTempleteId");
                    dba.AddInParameter(cmd, "StageTempleteId", DbType.Guid, stageTempleteId);
                    int EffectRow = dba.ExecuteNonQuery(cmd);
                    if (EffectRow > 0)
                    {
                        flag = true;
                    }
                }
            }
            else
            {
                flag = true;
            }
            return flag;
        }

        #endregion

        #region LaborCost 人力成本
        public List<LaborCostSelectItem> GetAllLaborCosts(int? targetType , int pageIndex, int pageSize, out int rowCount)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT lab.*, ISNULL(u.RealName, p.PositionName) TargetName From LaborCost lab");
                sqlBuilder.Append(" LEFT JOIN auth_UserExtend u ON u.UserId = lab.TargetId")
                    .Append(" LEFT JOIN Position p ON p.PositionId = lab.TargetId")
                    .Append(" WHERE 1 = 1");
                if (targetType != null)
                {
                    sqlBuilder.Append(" AND lab.TargetType = @TargetType");
                    dba.AddInParameter(cmd, "TargetType", DbType.Int16, targetType);
                }
                sqlBuilder.Append(" ORDER BY lab.targetType");
                cmd.CommandText = sqlBuilder.ToString();
                List<LaborCostSelectItem> data = dba.ExecuteReaderPaging<List<LaborCostSelectItem>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new LaborCostSelectItem
                        {
                            LaborCostId = (Guid)r["LaborCostId"],
                            TargetType = (int)r.GetIntOrDefault("TargetType"),
                            TargetId = (Guid)r.GetGuidOrNull("TargetId"),
                            TargetName = r.GetStringOrNull("TargetName"),
                            UnitPrice = r.GetDecimalOrDefault("UnitPrice"),
                            UnitValue = r.GetDecimalOrDefault("UnitValue")
                        });
                    }, pageIndex, pageSize, out rowCount);
                return data;
            }
        }

        public object SaveLaborCost(LaborCostSelectItem labItem)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                Guid? id = null;
                if (labItem.LaborCostId == Guid.Empty)
                {
                    var results = (from n in context.LaborCosts
                                   where n.TargetType == labItem.TargetType
                                   where n.TargetId == labItem.TargetId
                                   select n);
                    if (results.Count() > 0) 
                    {
                        return new { Result = false, ErrorMessage = String.Format("成本目标 [{0}] 已存在，不可重复添加", labItem.TargetName) };
                    }
                    LaborCost lab = new LaborCost();
                    lab.LaborCostId = Guid.NewGuid();
                    lab.TargetType = labItem.TargetType;
                    lab.TargetId = labItem.TargetId;
                    lab.UnitPrice = labItem.UnitPrice;
                    lab.UnitValue = labItem.UnitValue;
                    context.LaborCosts.Add(lab);
                    id = lab.LaborCostId;
                }
                else
                {
                    LaborCost oldLab = context.LaborCosts.Where(n => n.LaborCostId == labItem.LaborCostId).Single();
                    if (oldLab == null)
                    {
                        throw new Exception("不存在此人力成本信息！");
                    }
                    oldLab.TargetType = labItem.TargetType;
                    oldLab.TargetId = labItem.TargetId;
                    oldLab.UnitPrice = labItem.UnitPrice;
                    oldLab.UnitValue = labItem.UnitValue;
                    id = labItem.LaborCostId;
                }
                int countChange = context.SaveChanges();
                return new { Result = countChange > 0, ID = id };
            }
        }

        public Boolean DeleteLaborCost(Guid?[] laborCostIds)
        {
            bool flag = false;
            if (laborCostIds != null && laborCostIds.Length > 0)
            {
                using (TeamDBContext context = new TeamDBContext())
                {
                    DatabaseAccess dba = context.DatabaseAccess();
                    DbCommand cmd = dba.CreateCommand("DELETE FROM LaborCost WHERE LaborCostId=@LaborCostId");
                    int length = laborCostIds.Length;
                    List<object> param = new List<object>(length);
                    for (int i = 0; i < length; i++ )
                    {
                        param.Add(new { LaborCostId = laborCostIds[i] });
                    }
                    int EffectRow = dba.ExecuteNonQueryMultiple(cmd, param);
                    if (EffectRow > 0)
                    {
                        flag = true;
                    }
                }
            }
            else
            {
                flag = true;
            }
            return flag;
        }

        #endregion

        #region 项目会议纪要
        public List<Object> getProjectMeetingSummarys(Guid? project, String isSelf, int pageIndex, int pageSize, out int rowCount)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT pms.MeetingId, pms.ProjectId, p.ProjectName, s.StageId, s.StageName, pms.Content, pms.Result, pms.CreateUser, pms.StartTime, e.EmployeeId, e.EmployeeName, may.Url ");
                sql.Append(" FROM ProjectMeetingSummarys pms");
                
                if (isSelf.Equals("1"))
                {
                    sql.Append(" INNER JOIN (SELECT * FROM ProjectMeetingEmployees");
                    Guid userId = UserContext.CurrentUser.UserId;
                    sql.Append(" WHERE EmployeeId = @p2 ");
                    dba.AddInParameter(cmd, "p2", userId);
                    sql.Append(" )pme ON pms.MeetingId = pme.MeetingId");
                }
                
                sql.Append(" INNER JOIN ProjectMeetingEmployees e ON pms.MeetingId = e.MeetingId ");
                sql.Append(" LEFT JOIN MeetingAccessory may ON pms.MeetingId = may.MeetingId");
                sql.Append(" INNER JOIN Project p ON pms.ProjectId = p.ProjectId");
                sql.Append(" INNER JOIN (SELECT * FROM Stage WHERE Status = '103-01') s ON p.ProjectId = s.ProjectId");
                sql.Append(" WHERE 1=1");
                if(!project.Equals(Guid.Empty)) {
                    sql.Append(" AND pms.ProjectId = @p1 ");
                    dba.AddInParameter(cmd, "p1", project.Value);
                }
                
                cmd.CommandText = sql.ToString();
                List<Object> data = dba.ExecuteReaderPaging<List<Object>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new
                        {
                            MeetingId = r.GetGuidOrNull("MeetingId"),
                            ProjectId = r.GetGuidOrNull("ProjectId"),
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            StageId = r.GetGuidOrNull("StageId"),
                            StageName = r.GetStringOrNull("StageName"),
                            Content = r.GetStringOrNull("Content"),
                            Result = r.GetStringOrNull("Result"),
                            CreateUser = r.GetStringOrNull("CreateUser"),
                            StartTime = r.GetDateTimeOrDefault("StartTime"),
                            EmployeeName = r.GetStringOrNull("EmployeeName"),
                            EmployeeId = r.GetGuidOrNull("EmployeeId"),
                            Url = r.GetStringOrNull("Url")
                        });
                    }, pageIndex, pageSize, out rowCount);
                return data;
            }
        }

        public bool saveProjectMeeting(ProjectMeetingSummary meetingSum, Guid ProjectId, List<Guid> empGuids, List<string> empNames, List<string> acces)
        {
            using(TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand meetingCmd = dba.CreateCommand("INSERT INTO ProjectMeetingSummarys VALUES (@MeetingId, @StartTime, @Content, null, @Result, @CreateUser, @CreateTime, @ProjectId, null)");
                DbCommand empCmd = dba.CreateCommand("INSERT INTO ProjectMeetingEmployees VALUES (@MettingEmployeeId, @EmployeeName, @MeetingId, @EmployeeId)");
                DbCommand accesCmd = dba.CreateCommand("INSERT INTO MeetingAccessory VALUES (@AccessoryId, @Url, @MeetingId)");
                int num = 0;
                int i = 0;
                Project pro = null;
                if (ProjectId != null && ProjectId != Guid.Empty)
                {
                    pro = (from p in context.Projects
                                   where p.ProjectId == ProjectId
                                   select p).Single();
                }
                
                using (var tran = dba.BeginTransaction())
                {
                    ProjectMeetingEmployee pEmp = null;
                    MeetingAccessory meetingAcces = null;
                    
                    meetingSum.Project = pro;
                    meetingSum.CreateTime = DateTime.Now;
                    meetingSum.CreateUser = UserContext.CurrentUser.UserName;
                    if (meetingSum.MeetingId == Guid.Empty)
                    {
                        meetingSum.MeetingId = Guid.NewGuid();
                    }
                    AddMeetingParameters(dba, meetingCmd, meetingSum);
                    num += dba.ExecuteNonQuery(meetingCmd);
                    if (empGuids != null && empGuids.Count() > 0)
                    {
                        AddEmployeeParameters(dba, empCmd);
                        for (; i < empGuids.Count(); i++)
                        {
                            pEmp = new ProjectMeetingEmployee();
                            pEmp.MettingEmployeeId = Guid.NewGuid();
                            pEmp.EmployeeName = empNames[i];
                            updateEmployeeParameters(empCmd, pEmp, empGuids[i], meetingSum.MeetingId);
                            num += dba.ExecuteNonQuery(empCmd);
                        }
                    }
                    if (acces != null && acces.Count() > 0)
                    {
                        AddAccessoryParameters(dba, accesCmd);
                        for(i = 0; i < acces.Count(); i++)
                        {
                            meetingAcces = new MeetingAccessory();
                            meetingAcces.AccessoryId = Guid.NewGuid();
                            meetingAcces.Url = acces[i];
                            updateAccessoryParameters(accesCmd, meetingAcces, meetingSum.MeetingId);
                            num += dba.ExecuteNonQuery(accesCmd);
                        }
                    }
                    tran.Complete();
                }
                return num > 0;
            }
        }

        public bool updateProjectMeeting(ProjectMeetingSummary meetingSum, Guid ProjectId, List<Guid> empGuids, List<string> empNames, List<string> acces)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand meetingCmd = dba.CreateCommand("UPDATE ProjectMeetingSummarys SET StartTime =@StartTime, Content=@Content, Result=@Result,CreateUser=@CreateUser,CreateTime=@CreateTime WHERE MeetingId = @MeetingId");
                DbCommand empDelCmd = dba.CreateCommand("DELETE FROM ProjectMeetingEmployees WHERE MeetingId =@MeetingId");
                DbCommand empCmd = dba.CreateCommand("INSERT INTO ProjectMeetingEmployees VALUES (@MettingEmployeeId, @EmployeeName, @MeetingId, @EmployeeId)");
                DbCommand accesCmd = dba.CreateCommand("INSERT INTO MeetingAccessory VALUES (@AccessoryId, @Url, @MeetingId)");
                DbCommand accesDelCmd = dba.CreateCommand("DELETE FROM MeetingAccessory WHERE MeetingId =@MeetingId ");

                int num = 0;
                int i = 0;
                using (var tran = dba.BeginTransaction())
                {
                    ProjectMeetingEmployee pEmp = null;
                    MeetingAccessory meetingAcces = null;
                    
                    UpdateMeetingParameters(dba, meetingCmd, meetingSum);
                    num += dba.ExecuteNonQuery(meetingCmd);

                    dba.AddInParameter(empDelCmd, "MeetingId", meetingSum.MeetingId);
                    dba.ExecuteNonQuery(empDelCmd);

                    if (empGuids != null && empGuids.Count() > 0)
                    {
                        AddEmployeeParameters(dba, empCmd);
                        for (; i < empGuids.Count(); i++)
                        {
                            pEmp = new ProjectMeetingEmployee();
                            pEmp.MettingEmployeeId = Guid.NewGuid();
                            pEmp.EmployeeName = empNames[i];
                            updateEmployeeParameters(empCmd, pEmp, empGuids[i], meetingSum.MeetingId);
                            num += dba.ExecuteNonQuery(empCmd);
                        }
                    }
                    dba.AddInParameter(accesDelCmd, "MeetingId", meetingSum.MeetingId);
                    dba.ExecuteNonQuery(accesDelCmd);
                    if (acces != null && acces.Count() > 0)
                    {
                        AddAccessoryParameters(dba, accesCmd);
                        for (i = 0; i < acces.Count(); i++)
                        {
                            meetingAcces = new MeetingAccessory();
                            meetingAcces.AccessoryId = Guid.NewGuid();
                            meetingAcces.Url = acces[i];
                            updateAccessoryParameters(accesCmd, meetingAcces, meetingSum.MeetingId);
                            num += dba.ExecuteNonQuery(accesCmd);
                        }
                    }
                    tran.Complete();
                }
                return num > 0;
            }
        }

        void AddMeetingParameters(DatabaseAccess dba, DbCommand cmd, ProjectMeetingSummary meetingSum)
        {
            dba.AddInParameter(cmd, "MeetingId", DbType.Guid, meetingSum.MeetingId);
            dba.AddInParameter(cmd, "StartTime", DbType.DateTime, meetingSum.StartTime);
            dba.AddInParameter(cmd, "Content", DbType.String, meetingSum.Content);
           // dba.AddInParameter(cmd, "MeetingTypeId", DbType.String, meetingSum.MeetingTypeId);
            dba.AddInParameter(cmd, "Result", DbType.String, meetingSum.Result);
            dba.AddInParameter(cmd, "CreateUser", DbType.String, meetingSum.CreateUser);
            dba.AddInParameter(cmd, "CreateTime", DbType.DateTime, meetingSum.CreateTime);
            dba.AddInParameter(cmd, "ProjectId", DbType.Guid, meetingSum.Project.ProjectId);
           // dba.AddInParameter(cmd, "AccessoryId", DbType.String, meetingSum.Accessory);
        }

        void UpdateMeetingParameters(DatabaseAccess dba, DbCommand cmd, ProjectMeetingSummary meetingSum)
        {
            dba.AddInParameter(cmd, "MeetingId", DbType.Guid, meetingSum.MeetingId);
            dba.AddInParameter(cmd, "StartTime", DbType.DateTime, meetingSum.StartTime);
            dba.AddInParameter(cmd, "Content", DbType.String, meetingSum.Content);
            dba.AddInParameter(cmd, "Result", DbType.String, meetingSum.Result);
            dba.AddInParameter(cmd, "CreateUser", DbType.String, UserContext.CurrentUser.RealName);
            dba.AddInParameter(cmd, "CreateTime", DbType.DateTime, DateTime.Now);
        }

        void AddEmployeeParameters(DatabaseAccess dba, DbCommand cmd)
        {
            dba.AddInParameter(cmd, "MettingEmployeeId", DbType.Guid, null);
            dba.AddInParameter(cmd, "EmployeeName", DbType.String, null);
            dba.AddInParameter(cmd, "MeetingId", DbType.Guid, null);
            dba.AddInParameter(cmd, "EmployeeId", DbType.Guid, null);
        }

        void updateEmployeeParameters(DbCommand cmd, ProjectMeetingEmployee employee, Guid empGuid, Guid meetingGuid)
        {
            cmd.Parameters[0].Value = employee.MettingEmployeeId;
            cmd.Parameters[1].Value = employee.EmployeeName;
            cmd.Parameters[2].Value = meetingGuid;
            cmd.Parameters[3].Value = empGuid;
        }

        void AddAccessoryParameters(DatabaseAccess dba, DbCommand cmd)
        {
            dba.AddInParameter(cmd, "AccessoryId", DbType.Guid, null);
            dba.AddInParameter(cmd, "Url", DbType.String, null);
            dba.AddInParameter(cmd, "MeetingId", DbType.Guid, null);
        }

        void updateAccessoryParameters(DbCommand cmd, MeetingAccessory acces, Guid meetingGuid)
        {
            cmd.Parameters[0].Value = acces.AccessoryId;
            cmd.Parameters[1].Value = acces.Url;
            cmd.Parameters[2].Value = meetingGuid;
        }

        #endregion

        #region Dict 字典
        public List<ProjectSelectItem> GetProjectDict(Guid? pmId = null, DatabaseAccess dba = null)
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
            if(pmId != null)
            {
                cmd.CommandText = "SELECT p.ProjectId, p.ProjectName, p.ParentId, MAX(p.BeginTime) AS BeginTime FROM Project p INNER JOIN Stage s ON s.ProjectId = p.ProjectId WHERE p.PM = @PM OR s.Manager = @PM GROUP BY p.ParentId, p.ProjectId, p.ProjectName ORDER BY BeginTime DESC";
                dba.AddInParameter(cmd, "PM", DbType.Guid, pmId);
            }
            else
            {
                cmd.CommandText = "SELECT p1.ProjectId, p1.ProjectName, p1.ParentId FROM Project p1 INNER JOIN (SELECT ParentId, COUNT(ProjectId) AS ChildrenCount FROM Project GROUP BY ParentId HAVING ParentId IS NOT NULL) t ON p1.ProjectId = t.ParentId AND t.ChildrenCount > 1 UNION ALL SELECT p2.ProjectId, p2.ProjectName, t.ParentId FROM Project p2 LEFT JOIN (SELECT ParentId, COUNT(ProjectId) AS ChildrenCount FROM Project GROUP BY ParentId HAVING ParentId IS NOT NULL) t ON p2.ParentId = t.ParentId AND t.ChildrenCount > 1 WHERE p2.ParentId IS NOT NULL";
            }
            try
            {
                List<ProjectSelectItem> data = dba.ExecuteReader<List<ProjectSelectItem>>(
                    cmd,
                    (d, r) =>
                    {
                        d.Add(new ProjectSelectItem
                        {
                            ProjectId = Guid.Parse(r["ProjectId"].ToString()),
                            ProjectName = r.GetStringOrNull("ProjectName"),
                            ParentId = r.GetGuidOrNull("ParentId")
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

        public List<ProjectStageSelectItem> GetProjectStageDict(Guid? pmId = null, DatabaseAccess dba = null)
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
            if(pmId == null)
            {
                cmd.CommandText = "SELECT p.ProjectId, s.StageId, s.StageName FROM Project p INNER JOIN Stage s ON p.ProjectId = s.ProjectId ORDER BY p.ProjectName, s.SortValue";
                
            }
            else
            {
                cmd.CommandText = "SELECT t.ProjectId, t.StageId, t.StageName FROM (SELECT p.ProjectId, s.StageId, s.StageName, p.ProjectName, s.SortValue FROM Project p INNER JOIN Stage s ON p.ProjectId = s.ProjectId WHERE (p.PM = @PM) OR s.Manager = @PM) t ORDER BY t.ProjectName, t.SortValue";
                dba.AddInParameter(cmd, "PM", DbType.Guid, pmId);
            }
            
            try
            {
                List<ProjectStageSelectItem> data = dba.ExecuteReader<List<ProjectStageSelectItem>>(
                    cmd,
                    (d, r) =>
                    {
                        d.Add(new ProjectStageSelectItem
                        {
                            StageId = (Guid)r["StageId"],
                            StageName = r.GetStringOrNull("StageName"),
                            ProjectId = (Guid)r["ProjectId"]
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

        #endregion
    }
}
