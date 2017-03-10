using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to ProjectTeam table
    /// </summary>
    public class ProjectTeamMapping : EntityTypeConfiguration<ProjectTeam>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectTeamMapping()
        {
            //table
            ToTable("ProjectTeam");
            // Primary key
            HasKey(x => x.TeamId);
            // Properties
            //  TeamId is primary key
            Property(x => x.TeamId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            HasRequired(x => x.Employee).WithMany(c => c.ProjectTeams).Map(m => m.MapKey("EmployeeId"));
            HasRequired(x => x.Project).WithMany(c => c.ProjectTeams).Map(m => m.MapKey("ProjectId"));
            // Navigation properties
        }
    }
}
