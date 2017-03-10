using STIM.Modules.Dictionary;
using STIM.Modules.Personalize;
using STIM.Web.ErrorHandling;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TeamWork.Models;

namespace TeamWork
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            AuthenticateRequest += MvcApplication_AuthenticateRequest;
            Error += MvcApplication_Error;
        }

        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //系统字典数据初始化
            DictionaryData.InitailDictionaryData();
            //读取主题文件
            ThemeConfig.ThemesInitialization();
            UIManager.Current = UIManager.Create().DarkSourceUrl().LightSourceUrl();
            UIManager.CtrlsInitialization(UIManager.Current);
        }

        void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
        {
            //阻止所有没有通过身份验证的ajax请求
            AjaxAuthenticateRequest.AjaxAuthenticated();
        }
        
        void MvcApplication_Error(object sender, EventArgs e)
        {
            //全局系统异常处理
            ApplicationErrorHandler.ApplicationError(this);
        }
    }
}
