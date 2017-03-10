using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamWork.BLL.SecurityExtends
{
    public sealed class UserManager
    {
        private static readonly ConcurrentDictionary<Guid, EmployeeUser> employeeCache = new ConcurrentDictionary<Guid,EmployeeUser>();

        public static EmployeeUser GetEmployeeUser(Guid userId)
        {
            EmployeeUser empUser = null;
            if(employeeCache.TryGetValue(userId, out empUser))
            {
                return empUser;
            }
            return null;
        }

        public static EmployeeUser SetEmployeeUser(EmployeeUser empUser)
        {
            EmployeeUser user = null;
            user = employeeCache.AddOrUpdate(empUser.EmployeeId, empUser, UpdateEmployeeUserFunc);
            return user;
        }

        public static EmployeeUser UpdateEmployeeUserFunc(Guid key, EmployeeUser empUser)
        {
            return empUser;
        }
    }
}
