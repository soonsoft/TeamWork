using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Department table
    /// </summary>
    public class DepartmentMapping : EntityTypeConfiguration<Department>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public DepartmentMapping()
        {
            //table
            ToTable("Department");
            // Properties
            //  DepartmentId is primary key
            Property(x => x.DepartmentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.DepartmentName).HasMaxLength(50).IsRequired();
            Property(x => x.DepartmentType).HasMaxLength(40);
            Property(x => x.ParentId);
            HasRequired(x => x.Organization).WithMany(c => c.Departments).Map(m => m.MapKey("OrganizationId"));
            // Navigation properties
            //Foreign key to Position (Position)
            HasMany(x => x.Positions);
        }
    }
}
