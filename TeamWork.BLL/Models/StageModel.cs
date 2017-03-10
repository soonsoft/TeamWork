using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.Models
{
    public class StageModel : Stage
    {
        public string StatusName { get; set; }

        public string ManagerName { get; set; }
    }
}
