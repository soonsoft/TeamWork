using Newtonsoft.Json;
using STIM.Modules.Widget;
using STIM.Web;
using STIM.Web.MobileSupport;
using STIM.Web.Security;
using STIM.Web.Security.Simple;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using TeamWork.BLL;
using TeamWork.BLL.Models;
using TeamWork.DAL;

namespace TeamWork.Controllers
{
    public class AccountController : AccountBaseController
    {
        private const string ImageCacheName = "TodayImage";
        private const string DefaultPassword = "1";

        //
        // GET: /Account/

        [Mobile]
        [AllowAnonymous]
        public override ActionResult Login(string returnUrl)
        {
            ViewData["Image"] = GetLoginImage();
            return base.Login(returnUrl);
        }

        [Mobile]
        [AllowAnonymous]
        [HttpPost]
        public override ActionResult Login(string userName, string password, string returnUrl)
        {
            ViewData["Image"] = GetLoginImage();
            return base.Login(userName, password, returnUrl);
        }


        #region UserManage

        [SystemFunction(FunctionId = "099_01", FunctionName = "用户管理")]
        public ActionResult UserManage()
        {
            GetUserInfo(new DictionaryLogic());

            return View();
        }

        public ActionResult UserManageAdmin()
        {
            IList<IApplication> applications = Applications.LoadApplications();
            ViewBag.Applications = new DropDownList(applications, "ApplicationName", "ApplicationText");

            GetUserInfo(new DictionaryLogic());

            return View("UserManage");
        }

        protected virtual void GetUserInfo(DictionaryLogic dictLogic)
        {
            List<OrganizationSelectItem> oList;
            List<DepartmentSelectItem> dList;
            List<PositionSelectItem> pList;
            dictLogic.OrganizationDictionary(out oList, out dList, out pList);
            ViewBag.Organizations = new SelectList(oList, "OrganizationId", "OrganizationName");
            ViewBag.Departments = new SelectList(dList, "DepartmentId", "DepartmentName");
            ViewBag.Positions = new SelectList(pList, "PositionId", "PositionName");

            ViewBag.Status = new SelectList(
                dictLogic.GetEnabledDictionaryItems(DictionaryLogic.EmployeeStatus),
                DictionaryLogic.ValueField, DictionaryLogic.TextField);
        }

        public ActionResult GetAllUsers(string name, string status, int pageIndex, int pageSize, bool? loadedRoles, string applicationName)
        {
            int rowCount;
            IList<EmployeeModel> employees = null;
            IList<IRoleInfo> roles = null;

            EmployeeLogic eLogic = new EmployeeLogic();
            employees = eLogic.QueryEmployeeInfo(name, status, pageIndex, pageSize, out rowCount); 
            //WebSecurity.GetFullUsers(pageIndex, pageSize, out rowCount);
            if (loadedRoles != null && !loadedRoles.Value)
            {
                roles = WebSecurity.GetRoles();
            }
            return Json(new { Result = employees, RowCount = rowCount, Roles = roles });
        }

        public ActionResult SaveUser(Guid? userId,
                                    string userName,
                                    bool enabled,
                                    string userCode,
                                    string realName,
                                    string organization,
                                    string organizationName,
                                    string department,
                                    string departmentName,
                                    string position,
                                    string positionName,
                                    string status,
                                    Employee employeeInfo,
                                    Guid[] roleIds)
        {
            bool result = false;
            EmployeeLogic empLogic = new EmployeeLogic();
            EmployeeModel user = null;
            bool isCreate = userId == null;
            if(isCreate)
            {
                if (WebSecurity.UserNameExists(userName))
                {
                    return Json(new { Result = false, ErrorMessage = String.Format("用户名[{0}]已存在", userName) });
                }
                else
                {
                    user = empLogic.CreateEmployeeModel(Guid.NewGuid(), userName);
                }
            }
            else
            {
                user = empLogic.GetEmployeeInfoById(userId.Value);
                if (user == null)
                {
                    return Json(new { Result = false, ErrorMessage = "用户不存在" });
                }
            }

            user.UserCode = userCode;
            user.RealName = realName;
            user.Organization = organization;
            user.OrganizationName = organizationName;
            user.Department = department;
            user.DepartmentName = departmentName;
            user.Position = position;
            user.PositionName = positionName;
            user.Status = status;

            user.Gender = employeeInfo.Gender;
            user.Birthday = employeeInfo.Birthday;
            user.CellPhoneNumber = employeeInfo.CellPhoneNumber;
            user.LinePhoneNumber = employeeInfo.LinePhoneNumber;
            user.Email = employeeInfo.Email;
            user.Email2 = employeeInfo.Email2;
            user.QQ = employeeInfo.QQ;
            user.ID = employeeInfo.ID;
            user.Address = employeeInfo.Address;

            if(isCreate)
            {
                user.Enabled = enabled;
                WebSecurity.CreateUser(user, DefaultPassword, null, empLogic.InsertEmployeeHandler);
                result = true;
            }
            else
            {
                if(user.Enabled != enabled)
                {
                    WebSecurity.EnabledUser(user.UserName, enabled);
                    user.Enabled = enabled;
                }
                result = WebSecurity.UpdateUser(user, empLogic.UpdateEmployeeHandler);
            }

            if (roleIds != null)
            {
                result = WebSecurity.UpdateRolesForUser(user.UserId, roleIds);
                Functions.RemoveFunctionCache(new Guid[] { user.UserId });
            }

            //加载该员工的成本信息
            empLogic.LoadEmployeeCost(user);

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(
                new { Result = result, Data = user });
            return Content(jsonData);
        }

        public ActionResult ResetPassword(Guid userId)
        {
            bool result = WebSecurity.ChangePassword(userId, DefaultPassword);
            return Json(new { Result = result });
        }

        public ActionResult DeleteUser(Guid userId)
        {
            IUserData user = UserContext.CurrentUser;
            if (!user.IsSuperUser())
            {
                throw new HttpException(403, "只有超级用户可以该方法");
            }
            EmployeeLogic empLogic = new EmployeeLogic();
            var result = WebSecurity.DeleteUser(userId, empLogic.DeleteEmployeeHandler);
            return Json(new { Result = result });
        }

        public ActionResult GetAllRoles(string applicationName)
        {
            IList<IRoleInfo> roles = null;
            IList<IFunction> functions = null;
            if (String.IsNullOrEmpty(applicationName))
            {
                roles = WebSecurity.GetFullRoles();
                functions = Functions.Provider.GetFunctions(enabled: true);
            }
            else
            {
                roles = WebSecurity.GetFullRolesByApplication(applicationName);
                functions = Functions.Provider.GetFunctionsByApplication(applicationName, enabled: true);
            }

            return Json(new { Result = roles, Functions = functions });
        }

        public ActionResult SaveRole(Guid? roleId, string roleName, string roleText, string[] functions, string applicationName)
        {
            bool result;
            IRoleInfo roleInfo = null;
            if (roleId == null)
            {
                roleInfo = WebSecurity.CreateRole(roleName, roleText);
                result = roleInfo != null;
            }
            else
            {
                result = WebSecurity.UpdateRoleText(roleText, roleId.Value);
                roleInfo = new SimpleRole(roleId.Value, roleName)
                {
                    RoleText = roleText
                };
            }
            if (result)
            {
                //更新角色功能列表
                if (functions != null && functions.Length > 0)
                {
                    result = WebSecurity.UpdateFunctionsForRole(roleInfo.RoleId, functions);
                    if (result)
                    {
                        //移除当前功能列表的缓存
                        WebSecurity.RemoveFunctionCache(roleInfo.RoleId);
                    }
                }
            }
            return Json(new { Result = result, Data = roleInfo });
        }

        #endregion

        #region LoginImage

        [HttpGet]
        [AllowAnonymous]
        public ActionResult BGImage()
        {
            LoginImage image = HttpContext.Cache[ImageCacheName] as LoginImage;
            if (image != null)
                return File(image.ImageData, "image/jpeg");
            else
                return File("~/Content/images/bingImage.jpg", "image/jpeg");
        }

        LoginImage GetLoginImage()
        {
            LoginImage image = HttpContext.Cache[ImageCacheName] as LoginImage;
            if (image == null)
            {
                lock (ImageCacheName)
                {
                    if (image == null)
                    {
                        LoginImageManager manager = new LoginImageManager();
                        image = manager.GetLastLoginImage();
                        if (image != null)
                        {
                            DateTime overdueTime;
                            if (image.EndDate.HasValue && image.EndDate.Value.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                            {
                                overdueTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 01:00:00")).AddDays(1);
                            }
                            else
                            {
                                overdueTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1);
                            }
                            HttpContext.Cache.Insert(ImageCacheName, image, null, overdueTime, Cache.NoSlidingExpiration);
                        }
                    }
                }
            }
            return image;
        }

        internal static void RemoveLoginImageCache(HttpContextBase httpContext)
        {
            httpContext.Cache.Remove(ImageCacheName);
        }

        #endregion

        #region 用户信息

        public ActionResult Information()
        {
            return View();
        }

        #endregion
    }
}
