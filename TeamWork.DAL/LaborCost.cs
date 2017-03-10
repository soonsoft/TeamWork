using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing LaborCost table
    /// </summary>
    public class LaborCost
    {
        [Display(Name="Labor Cost Id")]
        [Key]
        public System.Guid LaborCostId { get; set; }

        [Display(Name="Target Id")]
        [Required]
        public System.Guid TargetId { get; set; }

        [Display(Name="Target Type")]
        [Required]
        public int TargetType { get; set; }

        [Display(Name="Unit Price")]
        [Range(typeof(decimal), "0", "99999999")]
        public decimal? UnitPrice { get; set; }

        [Display(Name="Unit Value")]
        [Range(typeof(decimal), "0", "99999999")]
        public decimal? UnitValue { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[LaborCostId] = " + LaborCostId;

        }

        public override int GetHashCode()
        {
            return LaborCostId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as LaborCost;
            if (x == null) return false;
            return (LaborCostId == x.LaborCostId);

        }
        #endregion
    }
}
