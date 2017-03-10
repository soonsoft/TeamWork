using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to auth_Membership table
    /// </summary>
    public class AuthMembershipMapping : EntityTypeConfiguration<AuthMembership>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthMembershipMapping()
        {
            //table
            ToTable("auth_Membership");
            // Primary key
            HasKey(x => x.UserId);
            // Properties
            //  UserId is primary key
            Property(x => x.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.CreateDate);
            Property(x => x.Password).HasMaxLength(256).IsRequired();
            Property(x => x.PasswordChangedDate);
            Property(x => x.PasswordSalt).HasMaxLength(128);
            // Navigation properties
        }
    }
}
