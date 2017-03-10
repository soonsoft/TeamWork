using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to auth_UserProfile table
    /// </summary>
    public class AuthUserprofileMapping : EntityTypeConfiguration<AuthUserprofile>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthUserprofileMapping()
        {
            //table
            ToTable("auth_UserProfile");
            // Primary key
            HasKey(x => x.UserId);
            // Properties
            //  UserId is primary key
            Property(x => x.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.PropertyName).HasColumnName("PropertyNames").IsMaxLength().IsRequired();
            Property(x => x.PropertyValuesString).IsMaxLength().IsRequired();
            Property(x => x.PropertyValuesBinary).IsRequired();
            Property(x => x.Status).HasMaxLength(40);
            Property(x => x.LastUpdatedDateTime).IsRequired();
            // Navigation properties
        }
    }
}
