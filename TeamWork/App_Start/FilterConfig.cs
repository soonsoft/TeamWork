using STIM.Web.ErrorHandling;
using System.Web;
using System.Web.Mvc;

namespace TeamWork
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new WebApplicationErrorAttribute());
        }
    }
}
