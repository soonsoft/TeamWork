using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing StageTemplete table
    /// </summary>
    public class StageTemplete
    {
        [Display(Name="Stage Templete Id")]
        [Key]
        public System.Guid StageTempleteId { get; set; }

        [Display(Name="Project Type")]
        [Required]
        [StringLength(40)]
        public string ProjectType { get; set; }

        [Display(Name="Stage Name")]
        [Required]
        [StringLength(50)]
        public string StageName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int? SortValue { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[StageTempleteId] = " + StageTempleteId;

        }

        public override int GetHashCode()
        {
            return StageTempleteId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as StageTemplete;
            if (x == null) return false;
            return (StageTempleteId == x.StageTempleteId);

        }
        #endregion
    }
}
