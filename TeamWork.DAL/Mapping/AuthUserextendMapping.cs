using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to auth_UserExtend table
    /// </summary>
    public class AuthUserextendMapping : EntityTypeConfiguration<AuthUserextend>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthUserextendMapping()
        {
            //table
            ToTable("auth_UserExtend");
            // Primary key
            HasKey(x => x.UserId);
            // Properties
            //  UserId is primary key
            Property(x => x.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.UserCode).HasMaxLength(50);
            Property(x => x.RealName).HasMaxLength(100);
            Property(x => x.Organization).HasMaxLength(50);
            Property(x => x.OrganizationName).HasMaxLength(100);
            Property(x => x.Department).HasMaxLength(50);
            Property(x => x.DepartmentName).HasMaxLength(100);
            Property(x => x.Position).HasMaxLength(50);
            Property(x => x.PositionName).HasMaxLength(100);
            Property(x => x.Status).HasMaxLength(40);
            // Navigation properties
        }
    }
}
