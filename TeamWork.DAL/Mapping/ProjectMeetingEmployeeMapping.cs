using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to ProjectMeetingEmployees table
    /// </summary>
    public class ProjectMeetingEmployeeMapping : EntityTypeConfiguration<ProjectMeetingEmployee>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectMeetingEmployeeMapping()
        {
            //table
            ToTable("ProjectMeetingEmployees");
            // Primary key
            HasKey(x => x.MettingEmployeeId);
            // Properties
            //  MettingEmployeeId is primary key
            Property(x => x.MettingEmployeeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.EmployeeName).HasMaxLength(50);
            HasRequired(x => x.Employee).WithMany(c => c.ProjectMeetingEmployees).Map(m => m.MapKey("EmployeeId"));
            HasRequired(x => x.Meeting).WithMany(c => c.ProjectMeetingEmployees).Map(m => m.MapKey("MeetingId"));
            // Navigation properties
        }
    }
}
