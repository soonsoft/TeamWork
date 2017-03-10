using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to auth_User table
    /// </summary>
    public class AuthUserMapping : EntityTypeConfiguration<AuthUser>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthUserMapping()
        {
            //table
            ToTable("auth_User");
            // Primary key
            HasKey(x => x.UserId);
            // Properties
            //  UserId is primary key
            Property(x => x.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.UserName).HasMaxLength(100).IsRequired();
            Property(x => x.Enabled).IsRequired();
            Property(x => x.ApplicationName).HasMaxLength(30).IsRequired();
            // Navigation properties
            // Many to many foreign key to auth_Role
            HasMany(x => x.AuthRoles).WithMany(z => z.AuthUsers)
            .Map(map => 
            {
                map.ToTable("auth_UsersInRoles");
                map.MapLeftKey("UserId");
                map.MapRightKey("RoleId");
            }
            );
        }
    }
}
