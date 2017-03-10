using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing sys_Menu table
    /// </summary>
    public class SysMenu
    {
        [Display(Name="Menu Id")]
        [Key]
        public System.Guid MenuId { get; set; }

        [Display(Name="Menu Key")]
        [StringLength(50)]
        public string MenuKey { get; set; }

        [StringLength(10)]
        public string Size { get; set; }

        [StringLength(500)]
        public string Icon { get; set; }

        [StringLength(10)]
        public string BGColor { get; set; }

        [StringLength(20)]
        public string Theme { get; set; }

        [StringLength(500)]
        public string Url { get; set; }

        [Display(Name="Is Tile")]
        [Required]
        public bool IsTile { get; set; }

        public virtual SysFunction Function { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[MenuId] = " + MenuId;

        }

        public override int GetHashCode()
        {
            return MenuId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as SysMenu;
            if (x == null) return false;
            return (MenuId == x.MenuId);

        }
        #endregion
    }
}
