using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to AttendanceExceptionBill table
    /// </summary>
    public class AttendanceExceptionBillMapping : EntityTypeConfiguration<AttendanceExceptionBill>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public AttendanceExceptionBillMapping()
        {
            //table
            ToTable("AttendanceExceptionBill");
            // Properties
            //  Id is primary key
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.StartTime).IsRequired();
            Property(x => x.EndTime).IsRequired();
            Property(x => x.Reason).HasMaxLength(500);
            Property(x => x.LeaderSign);
            Property(x => x.LeaderSignDate);
            Property(x => x.CreateTime).IsRequired();
            HasRequired(x => x.Employee).WithMany(c => c.AttendanceExceptionBills).Map(m => m.MapKey("EmployeeId"));
            // Navigation properties
        }
    }
}
