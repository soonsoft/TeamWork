using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWork.DAL
{

    /// <summary>
    /// Class representing Project table
    /// </summary>
    public class Project
    {

        public Project()
        {
            Budgets = new List<Budget>();
            ProjectTeams = new List<ProjectTeam>();
            Stages = new List<Stage>();
        }
        
        [Display(Name="Project Id")]
        [Key]
        public System.Guid ProjectId { get; set; }

        [Display(Name="Project Name")]
        [Required]
        [StringLength(100)]
        public string ProjectName { get; set; }

        [Display(Name="Project Type")]
        [Required]
        [StringLength(40)]
        public string ProjectType { get; set; }

        [Display(Name="Begin Time")]
        public DateTime? BeginTime { get; set; }

        [Display(Name="End Time")]
        public DateTime? EndTime { get; set; }

        [StringLength(40)]
        public string Status { get; set; }

        public System.Guid? PM { get; set; }

        [Display(Name="Parent Id")]
        public System.Guid? ParentId { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public virtual ICollection<Budget> Budgets { get; private set; }

        public virtual ICollection<ProjectMeetingSummary> ProjectMeetingSummaries { get; private set; }

        public virtual ICollection<ProjectTeam> ProjectTeams { get; private set; }

        public virtual ICollection<Stage> Stages { get; private set; }

        #region overrides

        public override string ToString()
        {
            return "[ProjectId] = " + ProjectId;

        }

        public override int GetHashCode()
        {
            return ProjectId.GetHashCode();

        }

        public override bool Equals(object obj)
        {
            var x = obj as Project;
            if (x == null) return false;
            return (ProjectId == x.ProjectId);

        }
        #endregion
    }
}
