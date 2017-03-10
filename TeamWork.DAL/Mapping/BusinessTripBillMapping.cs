using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to BusinessTripBill table
    /// </summary>
    public class BusinessTripBillMapping : EntityTypeConfiguration<BusinessTripBill>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public BusinessTripBillMapping()
        {
            //table
            ToTable("BusinessTripBill");
            // Properties
            //  Id is primary key
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.StartTime).IsRequired();
            Property(x => x.EndTime).IsRequired();
            Property(x => x.Destination).HasMaxLength(100);
            Property(x => x.Path).HasMaxLength(200);
            Property(x => x.TripReason).HasMaxLength(500);
            Property(x => x.Person).HasColumnName("Persons").HasMaxLength(200);
            Property(x => x.RealPerson).HasColumnName("RealPersons").HasMaxLength(200);
            Property(x => x.PMSign);
            Property(x => x.PMSignDate);
            Property(x => x.DLSign);
            Property(x => x.DLSignDate);
            Property(x => x.CreateTime).IsRequired();
            HasRequired(x => x.Employee).WithMany(c => c.BusinessTripBills).Map(m => m.MapKey("EmployeeId"));
            // Navigation properties
        }
    }
}
