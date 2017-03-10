using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Organization table
    /// </summary>
    public class Organization
    {

        public Organization()
        {
            Departments = new List<Department>();
        }
        
        [Display(Name="Organization Id")]
        [Key]
        public System.Guid OrganizationId { get; set; }

        [Display(Name="Organization Name")]
        [StringLength(50)]
        public string OrganizationName { get; set; }

        [Display(Name="Full Name")]
        [StringLength(200)]
        public string FullName { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [Display(Name="Line Phone Number")]
        [StringLength(30)]
        public string LinePhoneNumber { get; set; }

        [Display(Name="Line Phone Number 2")]
        [StringLength(30)]
        public string LinePhoneNumber2 { get; set; }

        [StringLength(50)]
        public string CEO { get; set; }

        public System.Guid? CEOId { get; set; }

        [Display(Name="Organization Type")]
        [Required]
        [StringLength(40)]
        public string OrganizationType { get; set; }

        [Display(Name="Parent Id")]
        public System.Guid? ParentId { get; set; }

        public virtual ICollection<Department> Departments { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[OrganizationId] = " + OrganizationId;

        }

        public override int GetHashCode()
        {
            return OrganizationId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Organization;
            if (x == null) return false;
            return (OrganizationId == x.OrganizationId);

        }
        #endregion
    }
}
