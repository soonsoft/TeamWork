using STIM.Modules.Personalize;
using STIM.Web;
using STIM.Web.MobileSupport;
using STIM.Web.Profile;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamWork.BLL;
using TeamWork.BLL.Models;

namespace TeamWork.Controllers
{
    public class HomeController : Controller
    {
        [Mobile]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ThemeColors()
        {
            return JavaScript(ThemeConfig.GetThemeJson());
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserProtrait()
        {
            IUserData user = UserContext.CurrentUser;
            PersonalizeSettings userPersonalize = user.PersonalizeSettings();
            byte[] protrait = userPersonalize.Portrait;
            if (protrait == null)
            {
                return File("~/Content/images/protrait.png", "image/jpeg");
            }
            else
            {
                return File(protrait, "image/jpeg");
            }
        }

        public ActionResult ChangeTheme(string themeId)
        {
            IUserData user = UserContext.CurrentUser;
            PersonalizeSettings userPersonalize = user.PersonalizeSettings();
            string id = userPersonalize.ThemeId;
            JsonDataResult result = new JsonDataResult();
            if (id != themeId)
            {
                userPersonalize.ThemeId = themeId;
                result.Result = user.UpdatePersonalize(userPersonalize);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改界面的背景主题
        /// </summary>
        /// <param name="background"></param>
        /// <returns></returns>
        public ActionResult ChangeBackground(string background)
        {
            IUserData user = UserContext.CurrentUser;
            PersonalizeSettings userPersonalize = user.PersonalizeSettings();
            string bg = userPersonalize.Background;
            JsonDataResult result = new JsonDataResult();
            if (bg != background)
            {
                userPersonalize.Background = background;
                result.Result = user.UpdatePersonalize(userPersonalize);
            }
            return Json(result);
        }

        [AllowAnonymous]
        public ActionResult Theme(string themeId)
        {
            string cssText = ThemeConfig.ReadTheme(themeId);
            return Content(cssText, "text/css");
        }

        [AllowAnonymous]
        public ActionResult Building()
        {
            return View();
        }

        public ActionResult UpdateTiles(string key1, string key2, string key3)
        {
            Dictionary<string, object> dynamicDict = new Dictionary<string, object>();
            using(TilesLogic tLogic = new TilesLogic())
            {
                IUserData user = UserContext.CurrentUser;
                tLogic.Open();
                if(!String.IsNullOrEmpty(key1))
                {
                    DateTime today = DateTime.Now;
                    DateTime begin = today.AddDays(-today.Day + 1);
                    DateTime end = today;
                    var workHours = tLogic.GetWorkHoursByMonth(user.UserId, begin, end);
                    var holidays = tLogic.GetHolidayDays(today);
                    var data = FindWorkHours(workHours, holidays, begin, end);
                    dynamicDict[key1] = data;
                }
                if(!String.IsNullOrEmpty(key2))
                {
                    var data = tLogic.UnapprovedWorkTime(user.UserId);
                    dynamicDict[key2] = data;
                }
                if(!String.IsNullOrEmpty(key3))
                {
                    var data = tLogic.ProjectCount();
                    dynamicDict[key3] = data;
                }
            }
            return Json(dynamicDict);
        }

        List<WorkHourTileModel> FindWorkHours(List<WorkHourTileModel> data, string[] holidays, DateTime begin, DateTime end)
        {
            Dictionary<string, WorkHourTileModel> monthData = new Dictionary<string, WorkHourTileModel>(31);
            string formatStr = "yyyy-MM-dd";
            for (int i = 1, len = end.Day; i <= len; i++)
            {
                monthData[new DateTime(begin.Year, begin.Month, i).ToString(formatStr)] = null;
            }
            string todayDateText = DateTime.Now.ToString(formatStr);
            foreach (WorkHourTileModel m in data)
            {
                if (monthData.ContainsKey(m.DateText))
                {
                    if(m.DateText != todayDateText)
                    {
                        if(m.Hours >= m.WorkHours)
                        {
                            monthData.Remove(m.DateText);
                            continue;
                        }
                    }
                    monthData[m.DateText] = m;
                }
            }
            foreach(string holiday in holidays)
            {
                monthData.Remove(holiday);
            }
            List<WorkHourTileModel> result = monthData.Values.ToList();
            return result;
        }
    }
}
