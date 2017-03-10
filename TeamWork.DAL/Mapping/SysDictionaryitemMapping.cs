using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to sys_DictionaryItem table
    /// </summary>
    public class SysDictionaryitemMapping : EntityTypeConfiguration<SysDictionaryitem>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public SysDictionaryitemMapping()
        {
            //table
            ToTable("sys_DictionaryItem");
            // Primary key
            HasKey(x => x.DictItemId);
            // Properties
            //  DictItemId is primary key
            Property(x => x.DictItemId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasMaxLength(40).IsRequired();
            Property(x => x.DictItemKey).HasMaxLength(20);
            Property(x => x.DictItemValue).HasMaxLength(50);
            Property(x => x.TreePath).HasMaxLength(200);
            Property(x => x.ParentItemId).HasMaxLength(40);
            Property(x => x.Enabled).IsRequired();
            HasOptional(x => x.DictGroup).WithMany(c => c.SysDictionaryitems).Map(m => m.MapKey("DictGroupId"));
            // Navigation properties
        }
    }
}
