using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.SecurityExtends
{
    public static class UserDataExtensions
    {
        public static EmployeeUser ToEmployeeUser(this IUserData userData)
        {
            EmployeeUser empUser = UserManager.GetEmployeeUser(userData.UserId);
            if(empUser == null)
            {
                EmployeeLogic empLogic = new EmployeeLogic();
                Employee emp = empLogic.GetEmployeeById(userData.UserId);
                empUser = new EmployeeUser(userData)
                {
                    EmployeeId = userData.UserId,
                    Birthday = emp.Birthday,
                    CellPhoneNumber = emp.CellPhoneNumber,
                    LinePhoneNumber = emp.LinePhoneNumber,
                    Email = emp.Email,
                    Email2 = emp.Email2,
                    QQ = emp.QQ,
                    ID = emp.ID,
                    Address = emp.Address
                };
                UserManager.SetEmployeeUser(empUser);
            }
            return empUser;
        }
    }
}
