using STIM.Core;
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
    /// 机构相关的业务逻辑
    /// </summary>
    public class OrganizationLogic
    {
        public List<OrganizationModel> GetAllOrganizations(string OrganizationName, int pageIndex, int pageSize, out int rowCount)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                DatabaseAccess dba = context.DatabaseAccess();
                DbCommand cmd = dba.CreateCommand();
                StringBuilder sqlBuilder = new StringBuilder("SELECT org.*, dict.DictItemValue AS OrganizationTypeName FROM Organization org");
                sqlBuilder.Append(" INNER JOIN sys_DictionaryItem dict ON org.OrganizationType = dict.DictItemId ")
                          .Append(" WHERE 1 = 1");
                if (!String.IsNullOrEmpty(OrganizationName))
                {
                    sqlBuilder.Append(" AND org.OrganizationName LIKE @OrganizationName");
                    dba.AddInParameter(cmd, "OrganizationName", DbType.String, String.Concat("%", OrganizationName, "%"));
                }
                sqlBuilder.Append(" ORDER BY org.OrganizationName");
                cmd.CommandText = sqlBuilder.ToString();
                List<OrganizationModel> data = dba.ExecuteReaderPaging<List<OrganizationModel>>(
                    cmd, (d, r) =>
                    {
                        d.Add(new OrganizationModel
                        {
                            OrganizationId = Guid.Parse(r["OrganizationId"].ToString()),
                            OrganizationName = r.GetStringOrNull("OrganizationName"),
                            FullName = r.GetStringOrNull("FullName"),
                            Address = r.GetStringOrNull("Address"),
                            LinePhoneNumber = r.GetStringOrNull("LinePhoneNumber"),
                            LinePhoneNumber2 = r.GetStringOrNull("LinePhoneNumber2"),
                            CEO = r.GetStringOrNull("CEO"),
                            CEOId = r.GetGuidOrNull("CEOId"),
                            OrganizationType = r.GetStringOrNull("OrganizationType"),
                            OrganizationTypeName = r.GetStringOrNull("OrganizationTypeName"),
                            ParentId = r.GetGuidOrNull("ParentId")
                        });
                    }, pageIndex, pageSize, out rowCount);
                return data;
            }
        }

        public object SaveOrganization(Organization org)
        {
            using (TeamDBContext context = new TeamDBContext())
            {
                Guid? id = null;
                if (org.OrganizationId == Guid.Empty)
                {
                    org.OrganizationId = Guid.NewGuid();
                    context.Organizations.Add(org);
                    id = org.OrganizationId;
                }
                else
                {
                    Organization oldOrg = context.Organizations.Where(n => n.OrganizationId == org.OrganizationId).Single();
                    if (oldOrg == null)
                    {
                        throw new Exception("不存在此机构！");
                    }
                    oldOrg.OrganizationType = org.OrganizationType;
                    oldOrg.OrganizationName = org.OrganizationName;
                    oldOrg.FullName = org.FullName;
                    oldOrg.ParentId = org.ParentId;
                    oldOrg.CEOId = org.CEOId;
                    oldOrg.CEO = org.CEO;
                    oldOrg.LinePhoneNumber = org.LinePhoneNumber;
                    oldOrg.LinePhoneNumber2 = org.LinePhoneNumber2;
                    oldOrg.Address = org.Address;
                    id = oldOrg.OrganizationId;
                }
                int countChange = context.SaveChanges();
                return new { Result = countChange > 0, ID = id };
            }
        }

        public List<OrganizationSelectItem> GetOrganizationsDict(DatabaseAccess dba = null, bool getAll = true)
        {
            bool flag = false;
            TeamDBContext context = null;
            StringBuilder sqlBuilder = new StringBuilder("SELECT * FROM Organization org");
            if (!getAll)
            {
                sqlBuilder.Append(" WHERE org.ParentId IS null");
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
                List<OrganizationSelectItem> data = dba.ExecuteReader<List<OrganizationSelectItem>>(
                    cmd,
                    (d, r) =>
                    {
                        d.Add(new OrganizationSelectItem
                        {
                            OrganizationId = Guid.Parse(r["OrganizationId"].ToString()),
                            OrganizationName = r.GetStringOrNull("OrganizationName")
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
