using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to sys_Menu table
    /// </summary>
    public class SysMenuMapping : EntityTypeConfiguration<SysMenu>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public SysMenuMapping()
        {
            //table
            ToTable("sys_Menu");
            // Primary key
            HasKey(x => x.MenuId);
            // Properties
            //  MenuId is primary key
            Property(x => x.MenuId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.MenuKey).HasMaxLength(50);
            Property(x => x.Size).HasMaxLength(10);
            Property(x => x.Icon).HasMaxLength(500);
            Property(x => x.BGColor).HasMaxLength(10);
            Property(x => x.Theme).HasMaxLength(20);
            Property(x => x.Url).HasMaxLength(500);
            Property(x => x.IsTile).IsRequired();
            HasOptional(x => x.Function).WithMany(c => c.SysMenus).Map(m => m.MapKey("FunctionId"));
            // Navigation properties
        }
    }
}
