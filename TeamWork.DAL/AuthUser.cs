using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing auth_User table
    /// </summary>
    public class AuthUser
    {

        public AuthUser()
        {
            AuthRoles = new List<AuthRole>();
        }
        
        [Display(Name="User Id")]
        [Key]
        public System.Guid UserId { get; set; }

        [Display(Name="User Name")]
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Display(Name="Application Name")]
        [Required]
        [StringLength(30)]
        public string ApplicationName { get; set; }

        public virtual ICollection<AuthRole> AuthRoles { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[UserId] = " + UserId;

        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as AuthUser;
            if (x == null) return false;
            return (UserId == x.UserId);

        }
        #endregion
    }
}
