using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to sys_LoginImages table
    /// </summary>
    public class SysLoginimageMapping : EntityTypeConfiguration<SysLoginimage>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public SysLoginimageMapping()
        {
            //table
            ToTable("sys_LoginImages");
            // Primary key
            HasKey(x => x.BingImageId);
            // Properties
            //  BingImageId is primary key
            Property(x => x.BingImageId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.StartDate);
            Property(x => x.EndDate);
            Property(x => x.Copyright).HasMaxLength(200);
            Property(x => x.ImageName).HasMaxLength(200);
            Property(x => x.BingHost).HasMaxLength(50);
            Property(x => x.ImageUrl).HasMaxLength(200);
            Property(x => x.ImageData);
            Property(x => x.Hotspot).HasColumnName("Hotspots").HasMaxLength(2000);
            Property(x => x.Message).HasColumnName("Messages").HasMaxLength(2000);
            // Navigation properties
        }
    }
}
