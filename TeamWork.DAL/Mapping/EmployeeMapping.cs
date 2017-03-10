using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to Employee table
    /// </summary>
    public class EmployeeMapping : EntityTypeConfiguration<Employee>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public EmployeeMapping()
        {
            //table
            ToTable("Employee");
            // Properties
            //  EmployeeId is primary key
            Property(x => x.EmployeeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.Gender).IsRequired();
            Property(x => x.Birthday);
            Property(x => x.CellPhoneNumber).HasMaxLength(20);
            Property(x => x.LinePhoneNumber).HasMaxLength(30);
            Property(x => x.Email).HasMaxLength(100);
            Property(x => x.Email2).HasMaxLength(100);
            Property(x => x.QQ).HasMaxLength(20);
            Property(x => x.ID).HasMaxLength(30);
            Property(x => x.Address).HasMaxLength(200);
            Property(x => x.Photo).HasMaxLength(300);
            // Navigation properties
            //Foreign key to AttendanceExceptionBill (AttendanceExceptionBill)
            HasMany(x => x.AttendanceExceptionBills);
            //Foreign key to BusinessTripBill (BusinessTripBill)
            HasMany(x => x.BusinessTripBills);
            //Foreign key to ProjectMeetingEmployees (ProjectMeetingEmployee)
            HasMany(x => x.ProjectMeetingEmployees);
            //Foreign key to ProjectTeam (ProjectTeam)
            HasMany(x => x.ProjectTeams);
            //Foreign key to ReqestLeaveBill (ReqestLeaveBill)
            HasMany(x => x.ReqestLeaveBills);
        }
    }
}
