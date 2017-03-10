using STIM.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using STIM.Modules.Widget.Weathering;

namespace TeamWork.Controllers
{
    [AllowAnonymous]
    public class WeatherController : AjaxAsyncController
    {
        private const string WeatherCacheName = "WeatherInfo";

        public void WeatherDataAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            WeatherInfo weatherInfo = HttpContext.Cache[WeatherCacheName] as WeatherInfo;
            if (weatherInfo == null)
            {
                Weather weather = WeatherFactory.CreateWeather();
                weather.GetWeatherInfoAsync(GotWeatherInfo, 
                    error =>
                    {
                        SetError(error);
                        GotWeatherInfo(null, true);
                    }
                );
            }
            else
            {
                GotWeatherInfo(weatherInfo, true);
            }
        }

        public ActionResult WeatherDataCompleted(WeatherInfo weatherInfo, bool isCache)
        {
            JsonResult result = null;
            if (HasError())
            {
                return ProcessError();
            }

            if (!isCache)
            {
                lock (WeatherCacheName)
                {
                    if (HttpContext.Cache[WeatherCacheName] == null)
                    {
                        DateTime now = DateTime.Now;
                        int hour = 1;
                        DateTime timePoint = DateTime.Parse(now.ToString("yyyy-MM-dd HH:00:00")).AddHours(hour);
                        HttpContext.Cache.Insert(WeatherCacheName, weatherInfo, null, timePoint, Cache.NoSlidingExpiration);
                    }
                }
            }
            if (weatherInfo == null)
                result = new JsonResult();
            else
                result = Json(weatherInfo);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        private void GotWeatherInfo(WeatherInfo info)
        {
            GotWeatherInfo(info, false);
        }

        private void GotWeatherInfo(WeatherInfo info, bool isCache)
        {
            AsyncManager.Parameters["weatherInfo"] = info;
            AsyncManager.Parameters["isCache"] = isCache;
            AsyncManager.OutstandingOperations.Decrement();
        }
    }
}