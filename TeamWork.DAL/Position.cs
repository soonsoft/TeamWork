using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Position table
    /// </summary>
    public class Position
    {
        [Display(Name="Position Id")]
        [Key]
        public System.Guid PositionId { get; set; }

        [Display(Name="Position Name")]
        [Required]
        [StringLength(50)]
        public string PositionName { get; set; }

        [Display(Name="Position Level")]
        [StringLength(40)]
        public string PositionLevel { get; set; }

        public virtual Department Department { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[PositionId] = " + PositionId;

        }

        public override int GetHashCode()
        {
            return PositionId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Position;
            if (x == null) return false;
            return (PositionId == x.PositionId);

        }
        #endregion
    }
}
