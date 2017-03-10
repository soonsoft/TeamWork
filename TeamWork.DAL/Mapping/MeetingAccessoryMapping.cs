using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to MeetingAccessory table
    /// </summary>
    public class MeetingAccessoryMapping : EntityTypeConfiguration<MeetingAccessory>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public MeetingAccessoryMapping()
        {
            //table
            ToTable("MeetingAccessory");
            // Primary key
            HasKey(x => x.AccessoryId);
            // Properties
            //  AccessoryId is primary key
            Property(x => x.AccessoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.Url).HasMaxLength(500).IsRequired();
            HasRequired(x => x.Meeting).WithMany(c => c.MeetingAccessories).Map(m => m.MapKey("MeetingId"));
            // Navigation properties
            //Foreign key to ProjectMeetingSummarys (ProjectMeetingSummary)
            HasMany(x => x.ProjectMeetingSummaries);
        }
    }
}
