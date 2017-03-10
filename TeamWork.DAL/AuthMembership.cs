using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing auth_Membership table
    /// </summary>
    public class AuthMembership
    {
        [Display(Name="User Id")]
        [Key]
        public System.Guid UserId { get; set; }

        [Display(Name="Create Date")]
        public DateTime? CreateDate { get; set; }

        [Required]
        [StringLength(256)]
        public string Password { get; set; }

        [Display(Name="Password Changed Date")]
        public DateTime? PasswordChangedDate { get; set; }

        [Display(Name="Password Salt")]
        [StringLength(128)]
        public string PasswordSalt { get; set; }

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
            var x = obj as AuthMembership;
            if (x == null) return false;
            return (UserId == x.UserId);

        }
        #endregion
    }
}
