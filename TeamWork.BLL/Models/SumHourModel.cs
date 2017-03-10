using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamWork.BLL.Models
{
    public class SumHourModel
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public int ChildrenCount { get; set; }

        public double SumHour { get; set; }

        public double TotalHour { get; set; }

        public double TotalValue { get; set; }
    }

    public class UnitValueParam
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public bool IsParent { get; set; }
    }

    public class ProjectTime
    {
        public string PBeginTime { get; set; }
        public string PEndTime { get; set; }
        public string RBeginTime { get; set; }
        public string REndTime { get; set; }
    }

    public class ProjectDepartment
    {
        public string DepartmentName { get; set; }

        public string YearMonth { get; set; }

        public double SumHour { get; set; }

    }

    public class EmployeeStatic
    {
        public Guid EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public double SumHour { get; set; }

        public double SumValue { get; set; }

        public double TotalHour { get; set; }

        public string StatusName { get; set; }

        public string AttendanceName { get; set; }

        public string DepartmentName { get; set; }
    }

    public class EmployeeDetailStatic
    {
        public string ProjectName { get; set; }

        public double SumHour { get; set; }

        public double SumValue { get; set; }

        public double TotalHour { get; set; }

        public string YearMonth { get; set; }
    }
}
