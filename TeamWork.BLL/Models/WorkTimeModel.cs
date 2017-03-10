using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.Models
{
    public class WorkTimeModel : WorkTime
    {
        public string ProjectName { get; set; }

        public string StageName { get; set; }

        public string EmployeeName { get; set; }

        public string ActivityName { get; set; }

        public string AttendanceTypeName { get; set; }

        public string ApprovedManagerName { get; set; }
    }
}
