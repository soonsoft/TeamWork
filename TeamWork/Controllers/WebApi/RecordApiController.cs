using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamWork.BLL;
using TeamWork.BLL.Models;

namespace TeamWork.Controllers.WebApi
{
    public class RecordApiController : ApiController
    {
        // GET: api/RecordApi?userId=[Guid]&beginTime=yyyy-MM-dd&endTime=yyyy-MM-dd
        public IEnumerable<WorkTimeModel> Get(Guid userId, DateTime beginTime, DateTime endTime)
        {
            WorkTimeLogic wtLogic = new WorkTimeLogic();
            IEnumerable<WorkTimeModel> data = wtLogic.GetWorkTime(beginTime, endTime, userId);
            return data;
        }

        // GET: api/RecordApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/RecordApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/RecordApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RecordApi/5
        public void Delete(int id)
        {
        }
    }
}
