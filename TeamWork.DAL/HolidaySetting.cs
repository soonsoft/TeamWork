using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing HolidaySettings table
    /// </summary>
    public class HolidaySetting
    {
        [Display(Name="Holiday Date")]
        [Key]
        public DateTime HolidayDate { get; set; }

        [Display(Name="Is Weekend")]
        public bool? IsWeekend { get; set; }

        [StringLength(20)]
        public string Festival { get; set; }

        [Display(Name="Setting Year")]
        public int? SettingYear { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[HolidayDate] = " + HolidayDate;

        }

        public override int GetHashCode()
        {
            return HolidayDate.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as HolidaySetting;
            if (x == null) return false;
            return (HolidayDate == x.HolidayDate);

        }
        #endregion
    }
}
