using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Project table
    /// </summary>
    public class ProjectMapping : EntityTypeConfiguration<Project>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectMapping()
        {
            //table
            ToTable("Project");
            // Properties
            //  ProjectId is primary key
            Property(x => x.ProjectId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.ProjectName).HasMaxLength(100).IsRequired();
            Property(x => x.ProjectType).HasMaxLength(40).IsRequired();
            Property(x => x.BeginTime);
            Property(x => x.EndTime);
            Property(x => x.Status).HasMaxLength(40);
            Property(x => x.PM);
            Property(x => x.ParentId);
            Property(x => x.Description).HasMaxLength(500);
            // Navigation properties
            //Foreign key to Budget (Budget)
            HasMany(x => x.Budgets);
            HasMany(x => x.ProjectMeetingSummaries);
            //Foreign key to ProjectTeam (ProjectTeam)
            HasMany(x => x.ProjectTeams);
            //Foreign key to Stage (Stage)
            HasMany(x => x.Stages);
        }
    }
}
