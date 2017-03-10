using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing sys_Function table
    /// </summary>
    public class SysFunction
    {

        public SysFunction()
        {
            AuthRoles = new List<AuthRole>();
            SysMenus = new List<SysMenu>();
        }
        
        [Display(Name="Function Id")]
        [Key]
        public System.Guid FunctionId { get; set; }

        [Display(Name="Function Code")]
        [StringLength(40)]
        public string FunctionCode { get; set; }

        [Display(Name="Function Name")]
        [StringLength(50)]
        public string FunctionName { get; set; }

        [Display(Name="Parent Func Id")]
        public System.Guid? ParentFuncId { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Display(Name="Application Name")]
        [Required]
        [StringLength(30)]
        public string ApplicationName { get; set; }

        public virtual ICollection<AuthRole> AuthRoles { get; private set; }

        public virtual ICollection<SysMenu> SysMenus { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[FunctionId] = " + FunctionId;

        }

        public override int GetHashCode()
        {
            return FunctionId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as SysFunction;
            if (x == null) return false;
            return (FunctionId == x.FunctionId);

        }
        #endregion
    }
}
