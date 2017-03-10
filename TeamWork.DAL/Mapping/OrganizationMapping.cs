using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Organization table
    /// </summary>
    public class OrganizationMapping : EntityTypeConfiguration<Organization>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public OrganizationMapping()
        {
            //table
            ToTable("Organization");
            // Properties
            //  OrganizationId is primary key
            Property(x => x.OrganizationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.OrganizationName).HasMaxLength(50);
            Property(x => x.FullName).HasMaxLength(200);
            Property(x => x.Address).HasMaxLength(300);
            Property(x => x.LinePhoneNumber).HasMaxLength(30);
            Property(x => x.LinePhoneNumber2).HasMaxLength(30);
            Property(x => x.CEO).HasMaxLength(50);
            Property(x => x.CEOId);
            Property(x => x.OrganizationType).HasMaxLength(40).IsRequired();
            Property(x => x.ParentId);
            // Navigation properties
            //Foreign key to Department (Department)
            HasMany(x => x.Departments);
        }
    }
}
