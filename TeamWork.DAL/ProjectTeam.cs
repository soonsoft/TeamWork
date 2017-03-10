using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing ProjectTeam table
    /// </summary>
    public class ProjectTeam
    {
        [Display(Name="Team Id")]
        [Key]
        public System.Guid TeamId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Project Project { get; set; }

        #region overrides

        public override string ToString()
        {
            return "[TeamId] = " + TeamId;

        }

        public override int GetHashCode()
        {
            return TeamId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as ProjectTeam;
            if (x == null) return false;
            return (TeamId == x.TeamId);

        }
        #endregion
    }
}
