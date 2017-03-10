using System;

using TeamWork.DAL;

namespace TeamWork.BLL.Models
{
    public class OrganizationSelectItem
    {
        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }
    }

    public class DepartmentSelectItem
    {
        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; }
    }

    public class PositionSelectItem
    {
        public Guid PositionId { get; set; }

        public string PositionName { get; set; }
    }

    public class OrganizationModel : Organization
    {
        public String OrganizationTypeName { get; set; }
    }

    public class DepartmentModel : Department
    {

        public Guid? OrganizationId { get; set; }

        public String OrganizationName { get; set; }

        public String ParentName { get; set; }
    }

    public class PositionModel : Position
    {

        public String PositionLevelName { get; set; }

        public Guid? DepartmentId { get; set; }

        public String DepartmentName { get; set; }
    }
}
