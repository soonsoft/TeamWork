using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to sys_Application table
    /// </summary>
    public class SysApplicationMapping : EntityTypeConfiguration<SysApplication>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public SysApplicationMapping()
        {
            //table
            ToTable("sys_Application");
            // Primary key
            HasKey(x => x.ApplicationName);
            // Properties
            //  ApplicationName is primary key
            Property(x => x.ApplicationName).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasMaxLength(30).IsRequired();
            Property(x => x.ApplicationText).HasMaxLength(50);
            // Navigation properties
        }
    }
}
