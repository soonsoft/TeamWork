using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Budget table
    /// </summary>
    public class BudgetMapping : EntityTypeConfiguration<Budget>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public BudgetMapping()
        {
            //table
            ToTable("Budget");
            // Properties
            //  BudgetId is primary key
            Property(x => x.BudgetId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.BudgetCategory).HasMaxLength(40).IsRequired();
            Property(x => x.Capital).HasPrecision(18, 2);
            Property(x => x.RDTime).HasPrecision(10, 2);
            HasRequired(x => x.Project).WithMany(c => c.Budgets).Map(m => m.MapKey("ProjectId"));
            // Navigation properties
        }
    }
}
