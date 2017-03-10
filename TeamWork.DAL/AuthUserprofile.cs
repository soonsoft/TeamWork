using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing auth_UserProfile table
    /// </summary>
    public class AuthUserprofile
    {
        [Display(Name="User Id")]
        [Key]
        public System.Guid UserId { get; set; }

        [Display(Name="Property Name")]
        [Required]
        public string PropertyName { get; set; }

        [Display(Name="Property Values String")]
        [Required]
        public string PropertyValuesString { get; set; }

        [Display(Name="Property Values Binary")]
        [Required]
        public System.Byte[] PropertyValuesBinary { get; set; }

        [StringLength(40)]
        public string Status { get; set; }

        [Display(Name="Last Updated Date Time")]
        [Required]
        public DateTime LastUpdatedDateTime { get; set; }

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
            var x = obj as AuthUserprofile;
            if (x == null) return false;
            return (UserId == x.UserId);

        }
        #endregion
    }
}
