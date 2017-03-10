using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing sys_Application table
    /// </summary>
    public class SysApplication
    {
        [Display(Name="Application Name")]
        [Key]
        [StringLength(30)]
        public string ApplicationName { get; set; }

        [Display(Name="Application Text")]
        [StringLength(50)]
        public string ApplicationText { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[ApplicationName] = " + ApplicationName;

        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(ApplicationName)) return base.GetHashCode(); //transient instance
            return ApplicationName.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as SysApplication;
            if (x == null) return false;
            if (string.IsNullOrEmpty(ApplicationName) && string.IsNullOrEmpty(ApplicationName)) return object.ReferenceEquals(this, x);
            return (ApplicationName == x.ApplicationName);

        }
        #endregion
    }
}
