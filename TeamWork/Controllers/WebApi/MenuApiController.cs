using Newtonsoft.Json;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TeamWork.Controllers.WebApi
{
    public class MenuApiController : ApiController
    {
        public IList<IFunction> Get(Guid userId)
        {
            IList<IFunction> functions = Functions.Provider.GetFunctions(userId);
            if (functions == null)
            {
                return new List<IFunction>();
            }
            return functions;
        }
    }
}
