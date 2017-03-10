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
    /// 职位相关的业务逻辑
    /// </summary>
    public class PositionLogic
    {

        public List<PositionModel> GetAllPositions(string positionName, string departmentName, int pageIndex, int pageSize, out int rowCount)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT p.*, d.DepartmentName, dict.DictItemValue AS PositionLevelName FROM Position p");
                sqlBuilder.Append(" LEFT JOIN sys_DictionaryItem dict ON p.PositionLevel = dict.DictItemId ")
                    .Append(" INNER JOIN Department d on d.DepartmentId = p.DepartmentId")
                    .Append(" WHERE 1 = 1");
                if (!String.IsNullOrEmpty(positionName))
                {
                    sqlBuilder.Append(" AND p.PositionName LIKE @PositionName");
                    dba.AddInParameter(cmd, "PositionName", DbType.String, String.Concat("%", positionName, "%"));
                }
                if (!String.IsNullOrEmpty(departmentName))
                {
                    sqlBuilder.Append(" AND d.DepartmentName LIKE @DepartmentName");
                    dba.AddInParameter(cmd, "DepartmentName", DbType.String, String.Concat("%", departmentName, "%"));
                }
                sqlBuilder.Append(" ORDER BY p.DepartmentId, p.PositionName");
                cmd.CommandText = sqlBuilder.ToString();
                List<PositionModel> data = dba.ExecuteReaderPaging<List<PositionModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new PositionModel
                        {
                            PositionId = Guid.Parse(r["PositionId"].ToString()),
                            PositionName = r.GetStringOrNull("PositionName"),
                            PositionLevel = r.GetStringOrNull("PositionLevel"),
                            PositionLevelName = r.GetStringOrNull("PositionLevelName"),
                            DepartmentId = r.GetGuidOrNull("DepartmentId"),
                            DepartmentName = r.GetStringOrNull("DepartmentName")
                        });
                    }, pageIndex, pageSize, out rowCount);
                return data;
            }
        }

        public object SavePosition(PositionModel pModel)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                Department dept = (from d in context.Departments
                                   where d.DepartmentId == pModel.DepartmentId
                                   select d).Single();
                Guid? id = null;
                if (pModel.PositionId == Guid.Empty)
                {
                    Position position = new Position();
                    position.PositionId = Guid.NewGuid();
                    position.PositionName = pModel.PositionName;
                    position.PositionLevel = pModel.PositionLevel;
                    position.Department = dept;
                    context.Positions.Add(position);
                    id = position.PositionId;
                }
                else
                {
                    Position oldPosition = context.Positions.Where(n => n.PositionId == pModel.PositionId).Single();
                    if (oldPosition == null)
                    {
                        throw new Exception("不存在此职位！");
                    }
                    oldPosition.PositionName = pModel.PositionName;
                    oldPosition.PositionLevel = pModel.PositionLevel;
                    oldPosition.Department = dept;
                    id = oldPosition.PositionId;
                }
                int countChange = context.SaveChanges();
                return new { Result = countChange > 0, ID = id };
            }
        }

        public List<PositionSelectItem> GetPositionsDict(DatabaseAccess dba = null)
        {
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
                List<PositionSelectItem> data = dba.ExecuteReader<List<PositionSelectItem>>(
                    "SELECT * FROM [Position]",
                    (d, r) =>
                    {
                        d.Add(new PositionSelectItem
                        {
                            PositionId = Guid.Parse(r["PositionId"].ToString()),
                            PositionName = r.GetStringOrNull("PositionName")
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
