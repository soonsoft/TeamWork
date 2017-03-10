using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamWork.Models
{
    public class ResultData<T>
    {
        public ResultData()
        {
            Result = false;
            Data = default(T);
            Message = null;
        }

        public bool Result { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }
    }
}