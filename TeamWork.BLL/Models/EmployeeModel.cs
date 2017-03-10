using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.Models
{
    public class EmployeeModel : Employee, IUserData
    {
        #region IUserData

        public string Department { get; set; }

        public string DepartmentName { get; set; }

        public bool Enabled { get; set; }

        public string Organization { get; set; }

        public string OrganizationName { get; set; }

        public string Position { get; set; }

        public string PositionName { get; set; }

        public string RealName { get; set; }
        public IList<IRoleInfo> Roles { get; internal set; }

        public string Status { get; set; }

        public string ToJson()
        {
            return null;
        }

        public string UserCode { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        #endregion

        public string StatusName { get; set; }
    }

    public class EmployeeQueryModel : EmployeeModel
    {
        public decimal? PositionUnitPrice { get; set; }

        public decimal? PositionUnitValue { get; set; }

        public decimal? UserUnitPrice { get; set; }

        public decimal? UserUnitValue { get; set; }
    }

    public class EmployeeSelectItem
    {
        public Guid EmployeeId { get; set; }

        public string RealName { get; set; }

        public string Organization { get; set; }

        public string OrganizationName { get; set; }

        public string Department { get; set; }

        public string DepartmentName { get; set; }

        public string Position { get; set; }

        public string PositionName { get; set; } 

        public string Status { get; set; }

        public string StatusName { get; set; }
    }
}
