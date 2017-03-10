using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing ReqestLeaveBill table
    /// </summary>
    public class RequestLeaveBill
    {
        [Key]
        public System.Guid Id { get; set; }

        [Display(Name="Leave Type")]
        [StringLength(40)]
        public string LeaveType { get; set; }

        [Display(Name="Leave Start")]
        [Required]
        public DateTime LeaveStart { get; set; }

        [Display(Name="Leave End")]
        [Required]
        public DateTime LeaveEnd { get; set; }

        [Display(Name="Work Day Count")]
        public decimal? WorkDayCount { get; set; }

        [Display(Name="Leave Reason")]
        [StringLength(500)]
        public string LeaveReason { get; set; }

        public System.Guid? PMSign { get; set; }

        [Display(Name="PMSign Date")]
        public DateTime? PMSignDate { get; set; }

        public System.Guid? DLSign { get; set; }

        [Display(Name="DLSign Date")]
        public DateTime? DLSignDate { get; set; }

        public System.Guid? CEOSign { get; set; }

        [Display(Name="CEOSign Date")]
        public DateTime? CEOSignDate { get; set; }

        [Display(Name="Create Time")]
        [Required]
        public DateTime CreateTime { get; set; }

        public virtual Employee Employee { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[Id] = " + Id;

        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as RequestLeaveBill;
            if (x == null) return false;
            return (Id == x.Id);

        }
        #endregion
    }
}
