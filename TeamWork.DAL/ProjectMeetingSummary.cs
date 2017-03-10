using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing ProjectMeetingSummarys table
    /// </summary>
    public class ProjectMeetingSummary
    {

        public ProjectMeetingSummary()
        {
            MeetingAccessories = new List<MeetingAccessory>();
            ProjectMeetingEmployees = new List<ProjectMeetingEmployee>();
        }
        
        [Display(Name="Meeting Id")]
        [Key]
        public System.Guid MeetingId { get; set; }

        [Display(Name="Start Time")]
        [Required]
        public DateTime StartTime { get; set; }

        [StringLength(500)]
        public string Content { get; set; }

        [Display(Name="Meeting Type Id")]
        [StringLength(10)]
        public string MeetingTypeId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Result { get; set; }

        [Display(Name="Create User")]
        [StringLength(100)]
        public string CreateUser { get; set; }

        [Display(Name="Create Time")]
        public DateTime? CreateTime { get; set; }

        public virtual Project Project { get; set; }

        public virtual MeetingAccessory Accessory { get; set; }

        public virtual ICollection<MeetingAccessory> MeetingAccessories { get; private set; }

        public virtual ICollection<ProjectMeetingEmployee> ProjectMeetingEmployees { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[MeetingId] = " + MeetingId;

        }

        public override int GetHashCode()
        {
            return MeetingId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as ProjectMeetingSummary;
            if (x == null) return false;
            return (MeetingId == x.MeetingId);

        }
        #endregion
    }
}
