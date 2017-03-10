using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing sys_DictionaryGroup table
    /// </summary>
    public class SysDictionarygroup
    {

        public SysDictionarygroup()
        {
            SysDictionaryitems = new List<SysDictionaryitem>();
        }
        
        [Display(Name="Dict Group Id")]
        [Key]
        [StringLength(20)]
        public string DictGroupId { get; set; }

        [Display(Name="Dict Group Name")]
        [Required]
        [StringLength(50)]
        public string DictGroupName { get; set; }

        [Display(Name="Is Tree")]
        [Required]
        public bool IsTree { get; set; }

        [Display(Name="Extension Table Name")]
        [StringLength(30)]
        public string ExtensionTableName { get; set; }

        public virtual ICollection<SysDictionaryitem> SysDictionaryitems { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[DictGroupId] = " + DictGroupId;

        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(DictGroupId)) return base.GetHashCode(); //transient instance
            return DictGroupId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as SysDictionarygroup;
            if (x == null) return false;
            if (string.IsNullOrEmpty(DictGroupId) && string.IsNullOrEmpty(DictGroupId)) return object.ReferenceEquals(this, x);
            return (DictGroupId == x.DictGroupId);

        }
        #endregion
    }
}
