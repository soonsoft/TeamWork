using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.Models
{
    public class LeaveBillModel : RequestLeaveBill
    {
        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public string LeaveTypeName { get; set; }
    }
}
