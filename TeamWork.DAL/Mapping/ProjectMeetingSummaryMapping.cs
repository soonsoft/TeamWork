using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to ProjectMeetingSummarys table
    /// </summary>
    public class ProjectMeetingSummaryMapping : EntityTypeConfiguration<ProjectMeetingSummary>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectMeetingSummaryMapping()
        {
            //table
            ToTable("ProjectMeetingSummarys");
            // Primary key
            HasKey(x => x.MeetingId);
            // Properties
            //  MeetingId is primary key
            Property(x => x.MeetingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.StartTime).IsRequired();
            Property(x => x.Content).HasMaxLength(500);
            Property(x => x.MeetingTypeId).HasMaxLength(10);
            Property(x => x.Result).HasMaxLength(2000).IsRequired();
            Property(x => x.CreateUser).HasMaxLength(100);
            Property(x => x.CreateTime);
            HasRequired(x => x.Project).WithMany(c => c.ProjectMeetingSummaries).Map(m => m.MapKey("ProjectId"));
            HasOptional(x => x.Accessory).WithMany(c => c.ProjectMeetingSummaries).Map(m => m.MapKey("AccessoryId"));
            HasMany(x => x.MeetingAccessories);
            HasMany(x => x.ProjectMeetingEmployees);
        }
    }
}
