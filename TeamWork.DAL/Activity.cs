using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Activity table
    /// </summary>
    public class Activity
    {
        [Display(Name="Activity Id")]
        [Key]
        public System.Guid ActivityId { get; set; }

        [Display(Name="Activity Text")]
        [StringLength(100)]
        public string ActivityText { get; set; }

        [Display(Name="Parent Id")]
        public System.Guid? ParentId { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public virtual Stage Stage { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[ActivityId] = " + ActivityId;

        }

        public override int GetHashCode()
        {
            return ActivityId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Activity;
            if (x == null) return false;
            return (ActivityId == x.ActivityId);

        }
        #endregion
    }
}
