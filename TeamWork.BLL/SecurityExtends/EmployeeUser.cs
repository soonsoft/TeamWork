using STIM.Core;
using STIM.Web.Security;
using STIM.Web.Security.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork.DAL;

namespace TeamWork.BLL.SecurityExtends
{
    public class EmployeeUser : Employee, IUserData
    {
        private IUserData _userData = null;
        public EmployeeUser(IUserData userData)
        {
            Guard.ArgumentNotNull(userData, "userData");
            _userData = userData;
        }

        public IUserData UserDataSource
        {
            get
            {
                return _userData;
            }
            set
            {
                Guard.ArgumentNotNull(value, "value");
                _userData = value;
            }
        }

        #region IUserData

        public string Department
        {
            get
            {
                return _userData.Department;
            }
            set
            {
                _userData.Department = value;
            }
        }

        public string DepartmentName
        {
            get
            {
                return _userData.DepartmentName;
            }
            set
            {
                _userData.DepartmentName = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _userData.Enabled;
            }
            set
            {
                _userData.Enabled = value;
            }
        }

        public string Organization
        {
            get
            {
                return _userData.Organization;
            }
            set
            {
                _userData.Organization = value;
            }
        }

        public string OrganizationName
        {
            get
            {
                return _userData.OrganizationName;
            }
            set
            {
                _userData.OrganizationName = value;
            }
        }

        public string Position
        {
            get
            {
                return _userData.Position;
            }
            set
            {
                _userData.Position = value;
            }
        }

        public string PositionName
        {
            get
            {
                return _userData.PositionName;
            }
            set
            {
                _userData.PositionName = value;
            }
        }

        public string RealName
        {
            get
            {
                return _userData.RealName;
            }
            set
            {
                _userData.RealName = value;
            }
        }

        public IList<IRoleInfo> Roles
        {
            get 
            {
                return _userData.Roles;
            }
        }

        public string Status
        {
            get
            {
                return _userData.Status;
            }
            set
            {
                _userData.Status = value;
            }
        }

        public string ToJson()
        {
            return _userData.ToJson();
        }

        public string UserCode
        {
            get
            {
                return _userData.UserCode;
            }
            set
            {
                _userData.UserCode = value;
            }
        }

        public Guid UserId
        {
            get 
            {
                return _userData.UserId;
            }
        }

        public string UserName
        {
            get 
            {
                return _userData.UserName;
            }
        }

        #endregion
    }
}
