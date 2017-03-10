using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.Models
{
    public class ProjectModel : Project
    {
        public int ChildrenCount { get; set; }

        public string ProjectTypeName { get; set; }

        public string StatusName { get; set; }

        public string PMName { get; set; }

        public double TotalHour { get; set; }

        public double TotalValue { get; set; }
    }

    public class ProjectSelectItem
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public Guid? ParentId { get; set; }
    }

    public class ProjectStageSelectItem : ProjectSelectItem
    {
        public Guid StageId { get; set; }

        public string StageName { get; set; }
    }

    public class LaborCostSelectItem : LaborCost
    {
        public string TargetName { get; set; }
    }
}
