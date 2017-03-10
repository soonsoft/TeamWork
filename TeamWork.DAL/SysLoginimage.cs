using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing sys_LoginImages table
    /// </summary>
    public class SysLoginimage
    {
        [Display(Name="Bing Image Id")]
        [Key]
        public System.Guid BingImageId { get; set; }

        [Display(Name="Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name="End Date")]
        public DateTime? EndDate { get; set; }

        [StringLength(200)]
        public string Copyright { get; set; }

        [Display(Name="Image Name")]
        [StringLength(200)]
        public string ImageName { get; set; }

        [Display(Name="Bing Host")]
        [StringLength(50)]
        public string BingHost { get; set; }

        [Display(Name="Image Url")]
        [StringLength(200)]
        public string ImageUrl { get; set; }

        [Display(Name="Image Data")]
        public System.Byte[] ImageData { get; set; }

        [StringLength(2000)]
        public string Hotspot { get; set; }

        [StringLength(2000)]
        public string Message { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[BingImageId] = " + BingImageId;

        }

        public override int GetHashCode()
        {
            return BingImageId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as SysLoginimage;
            if (x == null) return false;
            return (BingImageId == x.BingImageId);

        }
        #endregion
    }
}
