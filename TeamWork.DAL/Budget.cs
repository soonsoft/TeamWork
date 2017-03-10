using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Budget table
    /// </summary>
    public class Budget
    {
        [Display(Name="Budget Id")]
        [Key]
        public System.Guid BudgetId { get; set; }

        [Display(Name="Budget Category")]
        [Required]
        [StringLength(40)]
        public string BudgetCategory { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? Capital { get; set; }

        [Range(typeof(decimal), "0", "99999999")]
        public decimal? RDTime { get; set; }

        public virtual Project Project { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[BudgetId] = " + BudgetId;

        }

        public override int GetHashCode()
        {
            return BudgetId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Budget;
            if (x == null) return false;
            return (BudgetId == x.BudgetId);

        }
        #endregion
    }
}
