using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to WorkTimeSettings table
    /// </summary>
    public class WorkTimeSettingMapping : EntityTypeConfiguration<WorkTimeSetting>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public WorkTimeSettingMapping()
        {
            //table
            ToTable("WorkTimeSettings");
            // Properties
            //  WorkTimeSettingId is primary key
            Property(x => x.WorkTimeSettingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.AMStartTime);
            Property(x => x.AMEndTime);
            Property(x => x.PMStartTime);
            Property(x => x.PMEndTime);
            Property(x => x.Enabled).IsRequired();
            Property(x => x.WorkHour).HasColumnName("WorkHours").HasPrecision(5, 2);
            Property(x => x.Description).HasMaxLength(200);
            // Navigation properties
        }
    }
}
