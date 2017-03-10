using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to LaborCost table
    /// </summary>
    public class LaborCostMapping : EntityTypeConfiguration<LaborCost>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public LaborCostMapping()
        {
            //table
            ToTable("LaborCost");
            // Properties
            //  LaborCostId is primary key
            Property(x => x.LaborCostId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.TargetId).IsRequired();
            Property(x => x.TargetType).IsRequired();
            Property(x => x.UnitPrice).HasPrecision(10, 2);
            Property(x => x.UnitValue).HasPrecision(10, 2);
            // Navigation properties
        }
    }
}
