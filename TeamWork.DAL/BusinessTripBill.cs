using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing BusinessTripBill table
    /// </summary>
    public class BusinessTripBill
    {
        [Key]
        public System.Guid Id { get; set; }

        [Display(Name="Start Time")]
        [Required]
        public DateTime StartTime { get; set; }

        [Display(Name="End Time")]
        [Required]
        public DateTime EndTime { get; set; }

        [StringLength(100)]
        public string Destination { get; set; }

        [StringLength(200)]
        public string Path { get; set; }

        [Display(Name="Trip Reason")]
        [StringLength(500)]
        public string TripReason { get; set; }

        [StringLength(200)]
        public string Person { get; set; }

        [Display(Name="Real Person")]
        [StringLength(200)]
        public string RealPerson { get; set; }

        public System.Guid? PMSign { get; set; }

        [Display(Name="PMSign Date")]
        public DateTime? PMSignDate { get; set; }

        public System.Guid? DLSign { get; set; }

        [Display(Name="DLSign Date")]
        public DateTime? DLSignDate { get; set; }

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
            var x = obj as BusinessTripBill;
            if (x == null) return false;
            return (Id == x.Id);

        }
        #endregion
    }
}
