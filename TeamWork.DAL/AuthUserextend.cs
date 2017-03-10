using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing auth_UserExtend table
    /// </summary>
    public class AuthUserextend
    {
        [Display(Name="User Id")]
        [Key]
        public System.Guid UserId { get; set; }

        [Display(Name="User Code")]
        [StringLength(50)]
        public string UserCode { get; set; }

        [Display(Name="Real Name")]
        [StringLength(100)]
        public string RealName { get; set; }

        [StringLength(50)]
        public string Organization { get; set; }

        [Display(Name="Organization Name")]
        [StringLength(100)]
        public string OrganizationName { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        [Display(Name="Department Name")]
        [StringLength(100)]
        public string DepartmentName { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        [Display(Name="Position Name")]
        [StringLength(100)]
        public string PositionName { get; set; }

        [StringLength(40)]
        public string Status { get; set; }

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
            var x = obj as AuthUserextend;
            if (x == null) return false;
            return (UserId == x.UserId);

        }
        #endregion
    }
}
