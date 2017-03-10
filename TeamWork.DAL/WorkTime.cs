using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing WorkTime table
    /// </summary>
    public class WorkTime
    {
        [Display(Name="Work Time Id")]
        [Key]
        public System.Guid WorkTimeId { get; set; }

        [Display(Name="Employee Id")]
        [Required]
        public System.Guid EmployeeId { get; set; }

        [Display(Name="Begin Time")]
        public DateTime? BeginTime { get; set; }

        [Display(Name="End Time")]
        public DateTime? EndTime { get; set; }

        [Range(typeof(decimal), "0", "999")]
        public decimal? Hour { get; set; }

        [Display(Name="Project Id")]
        public System.Guid? ProjectId { get; set; }

        [Display(Name="Stage Id")]
        public System.Guid? StageId { get; set; }

        [Display(Name="Activity Id")]
        public System.Guid? ActivityId { get; set; }

        [Display(Name="Job Content")]
        [StringLength(500)]
        public string JobContent { get; set; }

        [Display(Name="Attendance Type")]
        [StringLength(40)]
        public string AttendanceType { get; set; }

        public bool? Approved { get; set; }

        [Display(Name="Approved Time")]
        public DateTime? ApprovedTime { get; set; }

        [Display(Name="Approved Manager")]
        public System.Guid? ApprovedManager { get; set; }

        [Display(Name="Fault Flag")]
        public bool? FaultFlag { get; set; }

        [Display(Name="Fault Reason")]
        [StringLength(100)]
        public string FaultReason { get; set; }

        [Display(Name="Work Time Setting Id")]
        public System.Guid? WorkTimeSettingId { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[WorkTimeId] = " + WorkTimeId;

        }

        public override int GetHashCode()
        {
            return WorkTimeId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as WorkTime;
            if (x == null) return false;
            return (WorkTimeId == x.WorkTimeId);

        }
        #endregion
    }
}
