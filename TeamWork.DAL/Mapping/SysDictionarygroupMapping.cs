using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to sys_DictionaryGroup table
    /// </summary>
    public class SysDictionarygroupMapping : EntityTypeConfiguration<SysDictionarygroup>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public SysDictionarygroupMapping()
        {
            //table
            ToTable("sys_DictionaryGroup");
            // Primary key
            HasKey(x => x.DictGroupId);
            // Properties
            //  DictGroupId is primary key
            Property(x => x.DictGroupId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasMaxLength(20).IsRequired();
            Property(x => x.DictGroupName).HasMaxLength(50).IsRequired();
            Property(x => x.IsTree).IsRequired();
            Property(x => x.ExtensionTableName).HasMaxLength(30);
            // Navigation properties
            //Foreign key to sys_DictionaryItem (SysDictionaryitem)
            HasMany(x => x.SysDictionaryitems);
        }
    }
}
