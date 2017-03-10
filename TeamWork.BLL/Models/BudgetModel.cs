using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.Models
{
    public class BudgetModel : Budget
    {
        public Guid ProjectId { get; set; }

        public string BudgetCategoryName { get; set; }
    }
}
