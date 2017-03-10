using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to ReqestLeaveBill table
    /// </summary>
    public class RequestLeaveBillMapping : EntityTypeConfiguration<RequestLeaveBill>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public RequestLeaveBillMapping()
        {
            //table
            ToTable("RequestLeaveBill");
            // Properties
            //  Id is primary key
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.LeaveType).HasMaxLength(40);
            Property(x => x.LeaveStart).IsRequired();
            Property(x => x.LeaveEnd).IsRequired();
            Property(x => x.WorkDayCount);
            Property(x => x.LeaveReason).HasMaxLength(500);
            Property(x => x.PMSign);
            Property(x => x.PMSignDate);
            Property(x => x.DLSign);
            Property(x => x.DLSignDate);
            Property(x => x.CEOSign);
            Property(x => x.CEOSignDate);
            Property(x => x.CreateTime).IsRequired();
            HasRequired(x => x.Employee).WithMany(c => c.ReqestLeaveBills).Map(m => m.MapKey("EmployeeId"));
            // Navigation properties
        }
    }
}
