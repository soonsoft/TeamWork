using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Stage table
    /// </summary>
    public class Stage
    {

        public Stage()
        {
            Activities = new List<Activity>();
        }
        
        [Display(Name="Stage Id")]
        [Key]
        public System.Guid StageId { get; set; }

        [Display(Name="Stage Name")]
        [Required]
        [StringLength(50)]
        public string StageName { get; set; }

        [Display(Name="Prev Stage")]
        public System.Guid? PrevStage { get; set; }

        [Display(Name="Next Stage")]
        public System.Guid? NextStage { get; set; }

        public System.Guid? Manager { get; set; }

        [Display(Name="Begin Time")]
        public DateTime? BeginTime { get; set; }

        [Display(Name="End Time")]
        public DateTime? EndTime { get; set; }

        [Required]
        [StringLength(40)]
        public string Status { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int? SortValue { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<Activity> Activities { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[StageId] = " + StageId;

        }

        public override int GetHashCode()
        {
            return StageId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Stage;
            if (x == null) return false;
            return (StageId == x.StageId);

        }
        #endregion
    }
}
