using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to StageTemplete table
    /// </summary>
    public class StageTempleteMapping : EntityTypeConfiguration<StageTemplete>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public StageTempleteMapping()
        {
            //table
            ToTable("StageTemplete");
            // Properties
            //  StageTempleteId is primary key
            Property(x => x.StageTempleteId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.ProjectType).HasMaxLength(40).IsRequired();
            Property(x => x.StageName).HasMaxLength(50).IsRequired();
            Property(x => x.Description).HasMaxLength(200);
            // Navigation properties
        }
    }
}
