using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TeamWork.DAL.Mapping;

namespace TeamWork.DAL
{

    public class TeamDBContext : DbContext
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public TeamDBContext()
            : base("name=TeamWorkDB")
        {
            //default ctor uses app.config connection named DALContext
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TeamDBContext(DbConnection connection) : base(connection,true)
        {
            //ctor for tracing
        }

        public IDbSet<Activity> Activities
        {
            get { return Set<Activity>(); }
        }

        public IDbSet<AttendanceExceptionBill> AttendanceExceptionBills
        {
            get { return Set<AttendanceExceptionBill>(); }
        }

        public IDbSet<AuthMembership> AuthMemberships
        {
            get { return Set<AuthMembership>(); }
        }

        public IDbSet<AuthRole> AuthRoles
        {
            get { return Set<AuthRole>(); }
        }

        public IDbSet<AuthUser> AuthUsers
        {
            get { return Set<AuthUser>(); }
        }

        public IDbSet<AuthUserextend> AuthUserextends
        {
            get { return Set<AuthUserextend>(); }
        }

        public IDbSet<AuthUserprofile> AuthUserprofiles
        {
            get { return Set<AuthUserprofile>(); }
        }

        public IDbSet<Budget> Budgets
        {
            get { return Set<Budget>(); }
        }

        public IDbSet<BusinessTripBill> BusinessTripBills
        {
            get { return Set<BusinessTripBill>(); }
        }

        public IDbSet<Department> Departments
        {
            get { return Set<Department>(); }
        }

        public IDbSet<Employee> Employees
        {
            get { return Set<Employee>(); }
        }

        public IDbSet<HolidaySetting> HolidaySettings
        {
            get { return Set<HolidaySetting>(); }
        }

        public IDbSet<LaborCost> LaborCosts
        {
            get { return Set<LaborCost>(); }
        }

        public IDbSet<MeetingAccessory> MeetingAccessories
        {
            get { return Set<MeetingAccessory>(); }
        }

        public IDbSet<Organization> Organizations
        {
            get { return Set<Organization>(); }
        }

        public IDbSet<Position> Positions
        {
            get { return Set<Position>(); }
        }

        public IDbSet<Project> Projects
        {
            get { return Set<Project>(); }
        }

        public IDbSet<ProjectMeetingEmployee> ProjectMeetingEmployees
        {
            get { return Set<ProjectMeetingEmployee>(); }
        }

        public IDbSet<ProjectMeetingSummary> ProjectMeetingSummaries
        {
            get { return Set<ProjectMeetingSummary>(); }
        }

        public IDbSet<ProjectTeam> ProjectTeams
        {
            get { return Set<ProjectTeam>(); }
        }

        public IDbSet<RequestLeaveBill> RequestLeaveBills
        {
            get { return Set<RequestLeaveBill>(); }
        }

        public IDbSet<Stage> Stages
        {
            get { return Set<Stage>(); }
        }

        public IDbSet<StageTemplete> StageTempletes
        {
            get { return Set<StageTemplete>(); }
        }

        public IDbSet<SysApplication> SysApplications
        {
            get { return Set<SysApplication>(); }
        }

        public IDbSet<SysDictionarygroup> SysDictionarygroups
        {
            get { return Set<SysDictionarygroup>(); }
        }

        public IDbSet<SysDictionaryitem> SysDictionaryitems
        {
            get { return Set<SysDictionaryitem>(); }
        }

        public IDbSet<SysFunction> SysFunctions
        {
            get { return Set<SysFunction>(); }
        }

        public IDbSet<SysLoginimage> SysLoginimages
        {
            get { return Set<SysLoginimage>(); }
        }

        public IDbSet<SysMenu> SysMenus
        {
            get { return Set<SysMenu>(); }
        }

        public IDbSet<WorkTime> WorkTimes
        {
            get { return Set<WorkTime>(); }
        }

        public IDbSet<WorkTimeSetting> WorkTimeSettings
        {
            get { return Set<WorkTimeSetting>(); }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<TeamDBContext>(null);

            modelBuilder.Configurations.Add(new ActivityMapping());
            modelBuilder.Configurations.Add(new AttendanceExceptionBillMapping());
            modelBuilder.Configurations.Add(new AuthMembershipMapping());
            modelBuilder.Configurations.Add(new AuthRoleMapping());
            modelBuilder.Configurations.Add(new AuthUserMapping());
            modelBuilder.Configurations.Add(new AuthUserextendMapping());
            modelBuilder.Configurations.Add(new AuthUserprofileMapping());
            modelBuilder.Configurations.Add(new BudgetMapping());
            modelBuilder.Configurations.Add(new BusinessTripBillMapping());
            modelBuilder.Configurations.Add(new DepartmentMapping());
            modelBuilder.Configurations.Add(new EmployeeMapping());
            modelBuilder.Configurations.Add(new HolidaySettingMapping());
            modelBuilder.Configurations.Add(new LaborCostMapping());
            modelBuilder.Configurations.Add(new MeetingAccessoryMapping());
            modelBuilder.Configurations.Add(new OrganizationMapping());
            modelBuilder.Configurations.Add(new PositionMapping());
            modelBuilder.Configurations.Add(new ProjectMapping());
            modelBuilder.Configurations.Add(new ProjectMeetingEmployeeMapping());
            modelBuilder.Configurations.Add(new ProjectMeetingSummaryMapping());
            modelBuilder.Configurations.Add(new ProjectTeamMapping());
            modelBuilder.Configurations.Add(new RequestLeaveBillMapping());
            modelBuilder.Configurations.Add(new StageMapping());
            modelBuilder.Configurations.Add(new StageTempleteMapping());
            modelBuilder.Configurations.Add(new SysApplicationMapping());
            modelBuilder.Configurations.Add(new SysDictionarygroupMapping());
            modelBuilder.Configurations.Add(new SysDictionaryitemMapping());
            modelBuilder.Configurations.Add(new SysFunctionMapping());
            modelBuilder.Configurations.Add(new SysLoginimageMapping());
            modelBuilder.Configurations.Add(new SysMenuMapping());
            modelBuilder.Configurations.Add(new WorkTimeMapping());
            modelBuilder.Configurations.Add(new WorkTimeSettingMapping());
        }
    }
}
