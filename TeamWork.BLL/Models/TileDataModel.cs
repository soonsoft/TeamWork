using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamWork.BLL.Models
{
    public class WorkTimeTileModel
    {
        public string DateText { get; set; }

        public string Description { get; set; }

        public decimal Hours { get; set; }
    }

    public class WorkHourTileModel
    {
        public string DateText { get; set; }

        public decimal Hours { get; set; }

        public decimal WorkHours { get; set; }
    }

    public class ProjectTileModel
    {
        public List<ProjectStatistic> TypeStatistic { get; set; }

        public List<ProjectStatistic> StatusStatistic { get; set; }
    }

    public class ProjectStatistic
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
