using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Department table
    /// </summary>
    public class Department
    {

        public Department()
        {
            Positions = new List<Position>();
        }
        
        [Display(Name="Department Id")]
        [Key]
        public System.Guid DepartmentId { get; set; }

        [Display(Name="Department Name")]
        [Required]
        [StringLength(50)]
        public string DepartmentName { get; set; }

        [Display(Name="Department Type")]
        [StringLength(40)]
        public string DepartmentType { get; set; }

        [Display(Name="Parent Id")]
        public System.Guid? ParentId { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<Position> Positions { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[DepartmentId] = " + DepartmentId;

        }

        public override int GetHashCode()
        {
            return DepartmentId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Department;
            if (x == null) return false;
            return (DepartmentId == x.DepartmentId);

        }
        #endregion
    }
}
