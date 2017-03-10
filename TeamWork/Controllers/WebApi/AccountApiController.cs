using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using TeamWork.Models;

namespace TeamWork.Controllers.WebApi
{
    public class AccountApiController : ApiController
    {
        public ResultData<IUserData> Post([FromBody]JObject jsonValue)
        {
            ResultData<IUserData> resultData = new ResultData<IUserData>();

            string userName = jsonValue.Value<string>("userName");
            string password = jsonValue.Value<string>("password");
            if(String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                resultData.Message = "用户名和密码不能为空";
                return resultData;
            }

            try
            {
                resultData.Result = Membership.ValidateUser(userName, password);
            }
            catch(Exception e)
            {
                resultData.Message = "验证用户名密码发生错误";
            }
            
            if(resultData.Result)
            {
                resultData.Data = WebSecurity.GetUser(userName);
            }
            else
            {
                resultData.Message = "用户名或密码不正确";
            }
            
            return resultData;
        }
    }
}
