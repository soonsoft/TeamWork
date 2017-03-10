using STIM.Core;
using STIM.Data.EntityFrameworkExtensions;
using STIM.Modules.Menu;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL
{
    public class TeamWorkMenuLogic : SystemMenuLogic
    {
        public bool SortFunctions(IEnumerable<SystemMenu> menus)
        {
            Guard.ArgumentNotNull(menus, "menus");

            int count = menus.Count();
            if (count == 0)
            {
                return false;
            }

            DatabaseAccess dba = CreateDatabaseAccess();
            DbCommand cmd = dba.CreateCommand("UPDATE sys_Function SET ParentFuncID = @p0 WHERE FunctionID = @p1");
            List<object[]> parameters = new List<object[]>(count);
            foreach (SystemMenu menu in menus)
            {
                parameters.Add(new object[]
                {
                    menu.ParentFuncId,
                    menu.FunctionId
                });
            }
            int affectRows = dba.ExecuteNonQueryMultiple(cmd, parameters);
            return affectRows == count;
        }
    }
}
