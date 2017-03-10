using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Position table
    /// </summary>
    public class PositionMapping : EntityTypeConfiguration<Position>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public PositionMapping()
        {
            //table
            ToTable("Position");
            // Properties
            //  PositionId is primary key
            Property(x => x.PositionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.PositionName).HasMaxLength(50).IsRequired();
            Property(x => x.PositionLevel).HasMaxLength(40);
            HasOptional(x => x.Department).WithMany(c => c.Positions).Map(m => m.MapKey("DepartmentId"));
            // Navigation properties
        }
    }
}
