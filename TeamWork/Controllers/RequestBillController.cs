using STIM.Modules.Dictionary;
using STIM.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamWork.BLL;
using TeamWork.BLL.Models;
using TeamWork.DAL;

namespace TeamWork.Controllers
{
    public class RequestBillController : Controller
    {
        #region 请假申请

        public ActionResult Leave()
        {
            DictionaryLogic dictionary = new DictionaryLogic();
            ViewBag.LeaveTypeList = dictionary.GetEnabledDictionaryItems(DictionaryLogic.LeaveType);

            EmployeeLogic elogic = new EmployeeLogic();
            //获取在职员工列表
            ViewBag.Employees = elogic.GetAllEmployees(false);

            IUserData user = UserContext.CurrentUser;
            ViewBag.EmployeeId = user.UserId.ToString();

            ViewBag.IsHighLevel = false;
            string paramPrint = Request.Params["_mode"];
            if(paramPrint == "print" && user.IsAdmin())
            {
                ViewBag.IsHighLevel = true;
            } 

            return View();
        }

        public ActionResult GetLeaveBills(string employeeID, DateTime? beginTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            RequestLeaveLogic leaveLogic = new RequestLeaveLogic();
            int rowCount;
            List<LeaveBillModel> bills = leaveLogic.QueryLeaveBills(employeeID, beginTime, endTime, pageIndex, pageSize, out rowCount);
            return Json(new { Result = bills, RowCount = rowCount });
        }

        public ActionResult AddLeaveData(
            string leaveType, 
            DateTime leaveStart, 
            bool amStart,
            DateTime leaveEnd,
            bool amEnd,
            string leaveReason)
        {
            RequestLeaveLogic leaveLogic = new RequestLeaveLogic();

            RequestLeaveBill leaveData = new RequestLeaveBill();
            leaveData.LeaveType = leaveType;
            leaveData.LeaveStart = leaveStart;
            leaveData.LeaveEnd = leaveEnd;
            leaveData.LeaveReason = leaveReason;

            IUserData user = UserContext.CurrentUser;

            bool result = leaveLogic.InsertRequestLeaveBill(leaveData, user, amStart, amEnd);
            return Json(new { Result = result });
        }

        public ActionResult EditLeaveData(
            Guid billId,
            string leaveType,
            DateTime leaveStart,
            bool amStart,
            DateTime leaveEnd,
            bool amEnd,
            string leaveReason)
        {
            RequestLeaveLogic leaveLogic = new RequestLeaveLogic();

            RequestLeaveBill leaveData = new RequestLeaveBill();
            leaveData.Id = billId;
            leaveData.LeaveType = leaveType;
            leaveData.LeaveStart = leaveStart;
            leaveData.LeaveEnd = leaveEnd;
            leaveData.LeaveReason = leaveReason;

            IUserData user = UserContext.CurrentUser;

            bool result = leaveLogic.UpdateRequestLeaveBill(leaveData, user, amStart, amEnd);
            return Json(new { Result = result, Data = leaveData });
        }

        public ActionResult DeleteLeaveData(Guid billId)
        {
            RequestLeaveLogic leaveLogic = new RequestLeaveLogic();
            bool result = leaveLogic.DeleteRequestLeaveBillById(billId);
            return Json(new { Result = result });
        }

        [HttpPost]
        public ActionResult LeavePrint(string billIdList)
        {
            List<LeaveBillModel> bills = null;
            if (!String.IsNullOrWhiteSpace(billIdList))
            {
                RequestLeaveLogic leaveLogic = new RequestLeaveLogic();
                bills = leaveLogic.GetLeaveBillsByIdArray(billIdList.Split(','));
            }
            ViewBag.PrintBills = bills;
            return View();
        }

        #endregion
    }
}