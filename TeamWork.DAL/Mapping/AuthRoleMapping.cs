using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to auth_Role table
    /// </summary>
    public class AuthRoleMapping : EntityTypeConfiguration<AuthRole>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthRoleMapping()
        {
            //table
            ToTable("auth_Role");
            // Primary key
            HasKey(x => x.RoleId);
            // Properties
            //  RoleId is primary key
            Property(x => x.RoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.RoleName).HasMaxLength(50).IsRequired();
            Property(x => x.RoleText).HasMaxLength(100);
            Property(x => x.ApplicationName).HasMaxLength(30).IsRequired();
            // Navigation properties
            // Many to many foreign key to sys_Function
            HasMany(x => x.SysFunctions).WithMany(z => z.AuthRoles)
            .Map(map => 
            {
                map.ToTable("auth_RolesInFunctions");
                map.MapLeftKey("RoleId");
                map.MapRightKey("FunctionId");
            }
            );
            // Many to many foreign key to auth_User
            HasMany(x => x.AuthUsers).WithMany(z => z.AuthRoles)
            .Map(map => 
            {
                map.ToTable("auth_UsersInRoles");
                map.MapLeftKey("RoleId");
                map.MapRightKey("UserId");
            }
            );
        }
    }
}
