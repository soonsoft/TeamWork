using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamWork.BLL.Models
{
    public class AttendanceInfoModel
    {
        public Guid AttendanceInfoId { get; set; }

        public string DeviceId { get; set; }

        public string OutsideUserId { get; set; }

        public DateTime Clock { get; set; }

        public string Remark { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
