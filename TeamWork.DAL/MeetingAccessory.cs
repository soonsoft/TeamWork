using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing MeetingAccessory table
    /// </summary>
    public class MeetingAccessory
    {

        public MeetingAccessory()
        {
            ProjectMeetingSummaries = new List<ProjectMeetingSummary>();
        }
        
        [Display(Name="Accessory Id")]
        [Key]
        public System.Guid AccessoryId { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; }

        public virtual ProjectMeetingSummary Meeting { get; set; }

        public virtual ICollection<ProjectMeetingSummary> ProjectMeetingSummaries { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[AccessoryId] = " + AccessoryId;

        }

        public override int GetHashCode()
        {
            return AccessoryId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as MeetingAccessory;
            if (x == null) return false;
            return (AccessoryId == x.AccessoryId);

        }
        #endregion
    }
}
