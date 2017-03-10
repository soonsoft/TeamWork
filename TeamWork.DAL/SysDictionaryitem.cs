using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing sys_DictionaryItem table
    /// </summary>
    public class SysDictionaryitem
    {
        [Display(Name="Dict Item Id")]
        [Key]
        [StringLength(40)]
        public string DictItemId { get; set; }

        [Display(Name="Dict Item Key")]
        [StringLength(20)]
        public string DictItemKey { get; set; }

        [Display(Name="Dict Item Value")]
        [StringLength(50)]
        public string DictItemValue { get; set; }

        [Display(Name="Tree Path")]
        [StringLength(200)]
        public string TreePath { get; set; }

        [Display(Name="Parent Item Id")]
        [StringLength(40)]
        public string ParentItemId { get; set; }

        [Required]
        public bool Enabled { get; set; }

        public virtual SysDictionarygroup DictGroup { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[DictItemId] = " + DictItemId;

        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(DictItemId)) return base.GetHashCode(); //transient instance
            return DictItemId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as SysDictionaryitem;
            if (x == null) return false;
            if (string.IsNullOrEmpty(DictItemId) && string.IsNullOrEmpty(DictItemId)) return object.ReferenceEquals(this, x);
            return (DictItemId == x.DictItemId);

        }
        #endregion
    }
}
