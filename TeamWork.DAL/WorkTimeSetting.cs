using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing WorkTimeSettings table
    /// </summary>
    public class WorkTimeSetting
    {
        [Display(Name="Work Time Setting Id")]
        [Key]
        public System.Guid WorkTimeSettingId { get; set; }

        [Display(Name="AMStart Time")]
        public System.TimeSpan? AMStartTime { get; set; }

        [Display(Name="AMEnd Time")]
        public System.TimeSpan? AMEndTime { get; set; }

        [Display(Name="PMStart Time")]
        public System.TimeSpan? PMStartTime { get; set; }

        [Display(Name="PMEnd Time")]
        public System.TimeSpan? PMEndTime { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Display(Name="Work Hour")]
        [Range(typeof(decimal), "0", "999")]
        public decimal? WorkHour { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[WorkTimeSettingId] = " + WorkTimeSettingId;

        }

        public override int GetHashCode()
        {
            return WorkTimeSettingId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as WorkTimeSetting;
            if (x == null) return false;
            return (WorkTimeSettingId == x.WorkTimeSettingId);

        }
        #endregion
    }
}
