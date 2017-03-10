using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing AttendanceExceptionBill table
    /// </summary>
    public class AttendanceExceptionBill
    {
        [Key]
        public System.Guid Id { get; set; }

        [Display(Name="Start Time")]
        [Required]
        public DateTime StartTime { get; set; }

        [Display(Name="End Time")]
        [Required]
        public DateTime EndTime { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        [Display(Name="Leader Sign")]
        public System.Guid? LeaderSign { get; set; }

        [Display(Name="Leader Sign Date")]
        public DateTime? LeaderSignDate { get; set; }

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
            var x = obj as AttendanceExceptionBill;
            if (x == null) return false;
            return (Id == x.Id);

        }
        #endregion
    }
}
