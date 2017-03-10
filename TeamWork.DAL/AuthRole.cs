using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing auth_Role table
    /// </summary>
    public class AuthRole
    {

        public AuthRole()
        {
            SysFunctions = new List<SysFunction>();
            AuthUsers = new List<AuthUser>();
        }
        
        [Display(Name="Role Id")]
        [Key]
        public System.Guid RoleId { get; set; }

        [Display(Name="Role Name")]
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [Display(Name="Role Text")]
        [StringLength(100)]
        public string RoleText { get; set; }

        [Display(Name="Application Name")]
        [Required]
        [StringLength(30)]
        public string ApplicationName { get; set; }

        public virtual ICollection<SysFunction> SysFunctions { get; private set; }

        public virtual ICollection<AuthUser> AuthUsers { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[RoleId] = " + RoleId;

        }

        public override int GetHashCode()
        {
            return RoleId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as AuthRole;
            if (x == null) return false;
            return (RoleId == x.RoleId);

        }
        #endregion
    }
}
