using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to HolidaySettings table
    /// </summary>
    public class HolidaySettingMapping : EntityTypeConfiguration<HolidaySetting>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public HolidaySettingMapping()
        {
            //table
            ToTable("HolidaySettings");
            // Primary key
            HasKey(x => x.HolidayDate);
            // Properties
            //  HolidayDate is primary key
            Property(x => x.HolidayDate).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.IsWeekend);
            Property(x => x.Festival).HasMaxLength(20);
            Property(x => x.SettingYear);
            // Navigation properties
        }
    }
}
