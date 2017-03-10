using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to sys_Function table
    /// </summary>
    public class SysFunctionMapping : EntityTypeConfiguration<SysFunction>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public SysFunctionMapping()
        {
            //table
            ToTable("sys_Function");
            // Primary key
            HasKey(x => x.FunctionId);
            // Properties
            //  FunctionId is primary key
            Property(x => x.FunctionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.FunctionCode).HasMaxLength(40);
            Property(x => x.FunctionName).HasMaxLength(50);
            Property(x => x.ParentFuncId);
            Property(x => x.Enabled).IsRequired();
            Property(x => x.ApplicationName).HasMaxLength(30).IsRequired();
            // Navigation properties
            // Many to many foreign key to auth_Role
            HasMany(x => x.AuthRoles).WithMany(z => z.SysFunctions)
            .Map(map => 
            {
                map.ToTable("auth_RolesInFunctions");
                map.MapLeftKey("FunctionId");
                map.MapRightKey("RoleId");
            }
            );
            //Foreign key to sys_Menu (SysMenu)
            HasMany(x => x.SysMenus);
        }
    }
}
