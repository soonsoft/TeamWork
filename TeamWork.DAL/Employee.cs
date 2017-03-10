using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Employee table
    /// </summary>
    public class Employee
    {

        public Employee()
        {
            AttendanceExceptionBills = new List<AttendanceExceptionBill>();
            BusinessTripBills = new List<BusinessTripBill>();
            ProjectMeetingEmployees = new List<ProjectMeetingEmployee>();
            ProjectTeams = new List<ProjectTeam>();
            ReqestLeaveBills = new List<RequestLeaveBill>();
        }
        
        [Display(Name="Employee Id")]
        [Key]
        public System.Guid EmployeeId { get; set; }

        [Required]
        public bool Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [Display(Name="Cell Phone Number")]
        [StringLength(20)]
        public string CellPhoneNumber { get; set; }

        [Display(Name="Line Phone Number")]
        [StringLength(30)]
        public string LinePhoneNumber { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name="Email 2")]
        [StringLength(100)]
        public string Email2 { get; set; }

        [StringLength(20)]
        public string QQ { get; set; }

        [StringLength(30)]
        public string ID { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(300)]
        public string Photo { get; set; }

        public virtual ICollection<AttendanceExceptionBill> AttendanceExceptionBills { get; private set; }

        public virtual ICollection<BusinessTripBill> BusinessTripBills { get; private set; }

        public virtual ICollection<ProjectMeetingEmployee> ProjectMeetingEmployees { get; private set; }

        public virtual ICollection<ProjectTeam> ProjectTeams { get; private set; }

        public virtual ICollection<RequestLeaveBill> ReqestLeaveBills { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[EmployeeId] = " + EmployeeId;

        }

        public override int GetHashCode()
        {
            return EmployeeId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Employee;
            if (x == null) return false;
            return (EmployeeId == x.EmployeeId);

        }
        #endregion
    }
}
