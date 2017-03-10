using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Stage table
    /// </summary>
    public class StageMapping : EntityTypeConfiguration<Stage>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public StageMapping()
        {
            //table
            ToTable("Stage");
            // Properties
            //  StageId is primary key
            Property(x => x.StageId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.StageName).HasMaxLength(50).IsRequired();
            Property(x => x.PrevStage);
            Property(x => x.NextStage);
            Property(x => x.Manager);
            Property(x => x.BeginTime);
            Property(x => x.EndTime);
            Property(x => x.Status).HasMaxLength(40).IsRequired();
            Property(x => x.Description).HasMaxLength(200);
            Property(x => x.SortValue);
            HasRequired(x => x.Project).WithMany(c => c.Stages).Map(m => m.MapKey("ProjectId"));
            // Navigation properties
            //Foreign key to Activity (Activity)
            HasMany(x => x.Activities);
        }
    }
}
