using STIM.Modules.Dictionary;
using STIM.Modules.Menu;
using STIM.Modules.Widget;
using STIM.Web;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TeamWork.BLL;
using TeamWork.DAL;

namespace TeamWork.Controllers
{
    public class SettingController : Controller
    {

        #region Dictionary

        [SystemFunction(FunctionId = "099_02", FunctionName = "字典维护")]
        public ActionResult DictionarySetting()
        {
            return View();
        }

        public ActionResult DictionarySettingAdmin()
        {
            ViewBag.IsAdminPage = true;
            return View("DictionarySetting");
        }

        public ActionResult GetDictionaryGroup()
        {
            DictionaryLogicBase logic = new DictionaryLogicBase();
            List<DictionaryGroup> groups = logic.GetAllGroups();
            return Json(groups);
        }

        public ActionResult AddDictionaryGroup(string dictGroupId, string dictGroupName, bool isTree)
        {
            DictionaryLogicBase logic = new DictionaryLogicBase();
            DictionaryGroup group = new DictionaryGroup
            {
                DictGroupId = dictGroupId,
                DictGroupName = dictGroupName,
                IsTree = isTree
            };
            var result = logic.CreateDictionaryGroup(group);
            return Json(new { Result = result, Data = group });
        }

        public ActionResult DeleteDictionaryGroup(string dictGroupId)
        {
            DictionaryLogicBase logic = new DictionaryLogicBase();
            var result = logic.DeleteDictionaryGroup(dictGroupId);
            return Json(new { Result = result });
        }

        public ActionResult GetDictionaryItems(string groupId)
        {
            DictionaryLogicBase logic = new DictionaryLogicBase();
            List<DictionaryItem> items = logic.GetDictionaryItems(groupId);
            return Json(items);
        }

        public ActionResult SaveDictionaryItem(DictionaryItem item)
        {
            DictionaryLogicBase logic = new DictionaryLogicBase();
            bool result = logic.SaveDictionaryItem(item);
            return Json(new { Result = result, Data = item });
        }

        public ActionResult DeleteDictionaryItems(List<string> idList)
        {
            DictionaryLogicBase logic = new DictionaryLogicBase();
            bool result = logic.DeleteDictionaryItems(idList);
            return Json(new { Result = result });
        }

        public ActionResult RebuildCacheData()
        {
            DictionaryData.InitailDictionaryData();
            var result = DictionaryData.Enabled;
            return Json(new { Result = result });
        }

        #endregion

        #region Menu Setting

        [SystemFunction(FunctionId = "90_03", FunctionName = "菜单管理")]
        public ActionResult MenuSetting()
        {
            return View("Menu");
        }

        /*
        public ActionResult MenuSettingAdmin()
        {
            IList<IApplication> applications = Applications.LoadApplications();
            ViewBag.Applications = new DropDownList(applications, "ApplicationName", "ApplicationText", false);
            return View("MenuSetting");
        }
        */

        public ActionResult LoadMenus(string applicationName)
        {
            return Json(GetSystemMenus(applicationName));
        }

        public ActionResult CreateMenu(SystemMenu menu, string applicationName)
        {
            SystemMenuLogic logic = new SystemMenuLogic();
            bool result = logic.CreateMenu(menu, applicationName);
            if (result)
            {
                if (String.IsNullOrEmpty(applicationName)
                        || applicationName == Applications.ApplicationNameOfRole)
                {
                    //重置菜单缓存
                    Menu.ResetMenuCache();
                }
            }
            return Json(new { Result = result, Data = menu });
        }

        public ActionResult UpdateMenu(SystemMenu menu)
        {
            SystemMenuLogic logic = new SystemMenuLogic();
            bool result = logic.UpdateMenu(menu);
            if (result)
            {
                SystemMenu oldMenu = Menu.GetMenu(menu.FunctionId);
                if (oldMenu != null && oldMenu.FunctionCode != menu.FunctionCode)
                {
                    Functions.RemoveFunctionCache(oldMenu.FunctionCode);
                }
                //重置菜单缓存
                Menu.ResetMenuCache();
            }
            return Json(new { Result = result, Data = menu });
        }

        public ActionResult UpdateFunctionCode(List<object[]> newFunctionCodes, string applicationName)
        {
            SystemMenuLogic logic = new SystemMenuLogic();
            bool result = logic.UpdateFunctionCode(newFunctionCodes);
            IList<SystemMenu> menus = null;
            if (result)
            {
                //重置菜单缓存
                Menu.ResetMenuCache();
                menus = GetSystemMenus(applicationName);
            }
            return Json(new { Result = result, Menus = menus });
        }

        public ActionResult DeleteMenu(Guid functionId, Guid menuId)
        {
            SystemMenuLogic logic = new SystemMenuLogic();
            bool result = logic.DeleteMenu(functionId, menuId);
            if (result)
            {
                SystemMenu oldMenu = Menu.GetMenu(functionId);
                if (oldMenu != null)
                {
                    Functions.RemoveFunctionCache(oldMenu.FunctionCode);
                }
                //重置菜单缓存
                Menu.ResetMenuCache();
            }
            return Json(new { Result = result });
        }

        IList<SystemMenu> GetSystemMenus(string applicationName)
        {
            if (String.IsNullOrEmpty(applicationName))
            {
                return Menu.Menus;
            }
            else
            {
                SystemMenuLogic mlogic = new SystemMenuLogic();
                var result = mlogic.GetMenus(applicationName);
                return result;
            }
        }

        public ActionResult SaveMenuSortValues(List<SystemMenu> menus)
        {
            TeamWorkMenuLogic logic = new TeamWorkMenuLogic();
            bool result = logic.SortFunctions(menus);
            if(result)
            {
                //重置菜单缓存
                Menu.ResetMenuCache();
            }
            return Json(new { Result = result });
        }

        #endregion

        #region Login Image Setting

        public ActionResult LoginImageSetting()
        {
            return View();
        }

        public ActionResult QueryImages(int pageIndex, int pageSize)
        {
            LoginImageManager imageManager = new LoginImageManager();
            int rowCount = 0;
            var data = imageManager.QueryLoginImage(pageIndex, pageSize, out rowCount);

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(
                new { Result = data, RowCount = rowCount });
            return Content(jsonData, "application/json");
        }

        public ActionResult SaveImage(LoginImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            if (!String.IsNullOrEmpty(image.ImageUrl))
            {
                string imagePath = HostingEnvironment.ApplicationPhysicalPath + Path.Combine(image.ImageUrl.Split('/'));
                image.ImageUrl = null;
                if (System.IO.File.Exists(imagePath))
                {
                    using (Image tempImage = Image.FromFile(imagePath))
                    {
                        using (Bitmap bitmap = new Bitmap(tempImage, new Size(1366, 768)))
                        {
                            using (MemoryStream mem = new MemoryStream())
                            {
                                bitmap.Save(mem, ImageFormat.Jpeg);
                                image.ImageData = mem.ToArray();
                            }
                        }
                    }
                    System.IO.File.Delete(imagePath);
                }
            }

            bool flag = image.BingImageId == Guid.Empty;
            LoginImageManager lim = new LoginImageManager();
            var result = lim.SaveLoginImage(image);
            if (flag && result)
            {
                AccountController.RemoveLoginImageCache(HttpContext);
            }

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(
                new { Result = result, Data = image });
            return Content(jsonData, "application/json");
        }

        public ActionResult SetTop(Guid imageId)
        {
            LoginImageManager lim = new LoginImageManager();
            var result = lim.SetTopImage(imageId);
            if (result)
            {
                AccountController.RemoveLoginImageCache(HttpContext);
            }
            return Json(new { Result = result });
        }

        public ActionResult DeleteImages(Guid[] imageIdList)
        {
            LoginImageManager imageManager = new LoginImageManager();
            var result = imageManager.DeleteLoginImages(imageIdList);
            return Json(new { Result = result });
        }

        #endregion

        #region FestivalSetting

        public ActionResult FestivalSetting()
        {
            return View();
        }

        public ActionResult GetWorkTimeSetting()
        {
            SettingsLogic logic = new SettingsLogic();
            var result = logic.GetWorkTimeSetting();
            return Json(result);
        }

        public ActionResult SaveWorkTimeSetting(WorkTimeSetting workTime)
        {
            SettingsLogic logic = new SettingsLogic();
            var result = logic.SaveWorkTime(workTime);
            return Json(result);
        }

        public ActionResult DeleteWorkTimeSetting(Guid timeId)
        {
            SettingsLogic logic = new SettingsLogic();
            var result = logic.DeleteWorkTime(timeId);
            return Json(result);
        }

        public ActionResult GetHolidaySetting(int year)
        {
            SettingsLogic logic = new SettingsLogic();
            var result = logic.GetHolidaySetting(year);
            return Json(result);
        }

        public ActionResult SaveHolidaySetting(HolidaySetting[] holidays, bool isInit)
        {
            SettingsLogic logic = new SettingsLogic();
            var result = logic.SaveHolidaySetting(holidays, isInit);
            return Json(result);
        }

        public ActionResult DeleteHolidaySetting(HolidaySetting[] holidays)
        {
            SettingsLogic logic = new SettingsLogic();
            var result = logic.DeleteHolidaySetting(holidays);
            return Json(result);
        }
        #endregion
    }
}
