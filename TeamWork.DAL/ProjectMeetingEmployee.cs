using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing ProjectMeetingEmployees table
    /// </summary>
    public class ProjectMeetingEmployee
    {
        [Display(Name="Metting Employee Id")]
        [Key]
        public System.Guid MettingEmployeeId { get; set; }

        [Display(Name="Employee Name")]
        [StringLength(50)]
        public string EmployeeName { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ProjectMeetingSummary Meeting { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[MettingEmployeeId] = " + MettingEmployeeId;

        }

        public override int GetHashCode()
        {
            return MettingEmployeeId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as ProjectMeetingEmployee;
            if (x == null) return false;
            return (MettingEmployeeId == x.MettingEmployeeId);

        }
        #endregion
    }
}
