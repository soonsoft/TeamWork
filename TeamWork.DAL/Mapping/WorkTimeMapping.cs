using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TeamWork.DAL.Mapping
{

    /// <summary>
    /// Class mapping to WorkTime table
    /// </summary>
    public class WorkTimeMapping : EntityTypeConfiguration<WorkTime>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public WorkTimeMapping()
        {
            //table
            ToTable("WorkTime");
            // Properties
            //  WorkTimeId is primary key
            Property(x => x.WorkTimeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).IsRequired();
            Property(x => x.EmployeeId).IsRequired();
            Property(x => x.BeginTime);
            Property(x => x.EndTime);
            Property(x => x.Hour).HasColumnName("Hours").HasPrecision(5, 2);
            Property(x => x.ProjectId);
            Property(x => x.StageId);
            Property(x => x.ActivityId);
            Property(x => x.JobContent).HasMaxLength(500);
            Property(x => x.AttendanceType).HasMaxLength(40);
            Property(x => x.Approved);
            Property(x => x.ApprovedTime);
            Property(x => x.ApprovedManager);
            Property(x => x.FaultFlag);
            Property(x => x.FaultReason).HasMaxLength(100);
            Property(x => x.WorkTimeSettingId);
            // Navigation properties
        }
    }
}
