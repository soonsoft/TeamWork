using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Activity table
    /// </summary>
    public class ActivityMapping : EntityTypeConfiguration<Activity>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ActivityMapping()
        {
            //table
            ToTable("Activity");
            // Properties
            //  ActivityId is primary key
            Property(x => x.ActivityId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.ActivityText).HasMaxLength(100);
            Property(x => x.ParentId);
            Property(x => x.Description).HasMaxLength(300);
            HasRequired(x => x.Stage).WithMany(c => c.Activities).Map(m => m.MapKey("StageId"));
            // Navigation properties
        }
    }
}
