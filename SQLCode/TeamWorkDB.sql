--用户表
CREATE TABLE auth_User
(
	UserId uniqueidentifier PRIMARY KEY NOT NULL,
	UserName nvarchar(100) NOT NULL,
	[Enabled] bit NOT NULL,
	ApplicationName varchar(30) NOT NULL,
	CONSTRAINT UQ_UserName UNIQUE(UserName)
);
Go

--用户密码表
CREATE TABLE auth_Membership
(
	UserId uniqueidentifier PRIMARY KEY NOT NULL,
	CreateDate datetime NULL,
	Password nvarchar(256) NOT NULL,
	PasswordChangedDate datetime NULL,
	PasswordSalt nvarchar(128) NULL
);
Go

--用户信息表
CREATE TABLE auth_UserExtend
(
	UserId uniqueidentifier PRIMARY KEY NOT NULL,
	UserCode varchar(50) NULL,
	RealName nvarchar(100) NULL,
	Organization nvarchar(50) NULL,
	OrganizationName nvarchar(100) NULL,
	Department nvarchar(50) NULL,
	DepartmentName nvarchar(100) NULL,
	Position nvarchar(50) NULL,
	PositionName nvarchar(100) NULL,
	[Status] varchar(40) NULL
);
Go

CREATE TABLE auth_UserProfile
(
	UserId uniqueidentifier PRIMARY KEY NOT NULL,
	PropertyNames nvarchar(max) NOT NULL,
	PropertyValuesString nvarchar(max) NOT NULL,
	PropertyValuesBinary varbinary(max) NOT NULL,
	[Status] varchar(40) NULL,
	LastUpdatedDateTime datetime NOT NULL
);
Go

--DROP TABLE auth_Role
CREATE TABLE auth_Role
(
	RoleId uniqueidentifier PRIMARY KEY NOT NULL,
	RoleName nvarchar(50) NOT NULL,
	RoleText nvarchar(100) NULL,
	ApplicationName varchar(30) NOT NULL,
	CONSTRAINT UQ_RoleKey UNIQUE(RoleName)
);
Go

--用户角色表
CREATE TABLE auth_UsersInRoles
(
	UserId uniqueidentifier NOT NULL,
	RoleId uniqueidentifier NOT NULL,
	CONSTRAINT PK_UserId_RoleId PRIMARY KEY(UserId, RoleId),
	CONSTRAINT FK_UserId_User FOREIGN KEY(UserId) REFERENCES auth_User(UserId),
	CONSTRAINT FK_RoleId_Role FOREIGN KEY(RoleId) REFERENCES auth_Role(RoleId)
);
Go

--子系统表
CREATE TABLE sys_Application
(
	ApplicationName varchar(30) PRIMARY KEY NOT NULL,
	ApplicationText nvarchar(50) NULL
);
Go

--字典组
CREATE TABLE sys_DictionaryGroup
(
	DictGroupId varchar(20) PRIMARY KEY NOT NULL,
	DictGroupName nvarchar(50) NOT NULL,
	IsTree bit NOT NULL,
	ExtensionTableName nvarchar(30) NULL
);
Go

--字典项
CREATE TABLE sys_DictionaryItem
(
	DictItemId varchar(40) PRIMARY KEY NOT NULL,
	DictItemKey varchar(20) NULL,
	DictItemValue nvarchar(50) NULL,
	TreePath varchar(200) NULL,
	ParentItemId varchar(40) NULL,
	DictGroupId varchar(20) NULL,
	[Enabled] bit NOT NULL,
	CONSTRAINT FK_DictionaryItem_DictItemId FOREIGN KEY(DictGroupId) REFERENCES sys_DictionaryGroup(DictGroupId)
);
Go

--登陆背景图片
CREATE TABLE sys_LoginImages
(
	BingImageId uniqueidentifier PRIMARY KEY NOT NULL,
	StartDate datetime NULL,
	EndDate datetime NULL,
	Copyright nvarchar(200) NULL,
	ImageName nvarchar(200) NULL,
	BingHost nvarchar(50) NULL,
	ImageUrl nvarchar(200) NULL,
	ImageData varbinary(max) NULL,
	Hotspots nvarchar(2000) NULL,
	[Messages] nvarchar(2000) NULL
)
Go

--系统功能表
CREATE TABLE sys_Function
(
	FunctionId uniqueidentifier PRIMARY KEY NOT NULL,
	FunctionCode varchar(40) NULL,
	FunctionName nvarchar(50) NULL,
	ParentFuncId uniqueidentifier NULL,
	[Enabled] bit NOT NULL,
	ApplicationName varchar(30) NOT NULL
	--CONSTRAINT AK_FunctionCode UNIQUE(FunctionCode) 
)
Go

--角色功能表
CREATE TABLE auth_RolesInFunctions
(
	RoleId uniqueidentifier NOT NULL,
	FunctionId uniqueidentifier NOT NULL,
	CONSTRAINT PK_RoleId_FunctionId PRIMARY KEY(RoleId, FunctionId),
	CONSTRAINT FK_RolesInFunctions_RoleId FOREIGN KEY(RoleId) REFERENCES auth_Role(RoleId),
	CONSTRAINT FK_RolesInFunctions_FunctionId FOREIGN KEY(FunctionId) REFERENCES sys_Function(FunctionId)
);
Go

--系统菜单表
CREATE TABLE sys_Menu
(
	MenuId uniqueidentifier PRIMARY KEY NOT NULL,
	FunctionId uniqueidentifier NULL,
	MenuKey nvarchar(50) NULL,
	Size varchar(10) NULL,
	Icon nvarchar(500) NULL,
	BGColor varchar(10) NULL,
	Theme nvarchar(20) NULL,
	Url nvarchar(500) NULL,
	IsTile bit NOT NULL,
	CONSTRAINT FK_Menu_FunctionId FOREIGN KEY(FunctionId) REFERENCES sys_Function(FunctionId)
)
Go

--机构表
CREATE TABLE Organization
(
	OrganizationId uniqueidentifier PRIMARY KEY NOT NULL,
	--机构名称
	OrganizationName nvarchar(50) NULL,
	--全称
	FullName nvarchar(200) NULL,
	--电子
	[Address] nvarchar(300) NULL,
	--电话
	LinePhoneNumber varchar(30) NULL,
	--电话2
	LinePhoneNumber2 varchar(30) NULL,
	--总经理
	CEO nvarchar(50) NULL,
	--总经理用户编号
	CEOId uniqueidentifier NULL,
	--机构类型 [字典: 2]
	OrganizationType varchar(40) NOT NULL,
	--从属关系
	ParentId uniqueidentifier NULL
)
Go

--部门表
CREATE TABLE Department
(
	DepartmentId uniqueidentifier PRIMARY KEY NOT NULL,
	OrganizationId uniqueidentifier NOT NULL,
	DepartmentName nvarchar(50) NOT NULL,
	--部门类型 [字典: 4]
	DepartmentType varchar(40) NULL,
	--部门的从属关系
	ParentId uniqueidentifier NULL,

	--外键 -> 机构表
	CONSTRAINT FK_Department_OrganizationId FOREIGN KEY(OrganizationId) REFERENCES Organization(OrganizationId)
)
Go

--职位表
CREATE TABLE Position
(
	PositionId uniqueidentifier PRIMARY KEY NOT NULL,
	--所属部门
	DepartmentId uniqueidentifier NULL,
	--职位名称
	PositionName nvarchar(50) NOT NULL,
	--职级
	PositionLevel varchar(40) NULL,

	--外键 -> 部门表
	CONSTRAINT FK_Position_DepartmentId FOREIGN KEY(DepartmentId) REFERENCES Department(DepartmentId)
)
Go

--员工表
CREATE TABLE Employee
(
	--员工ID 与UserId值相同
	EmployeeId uniqueidentifier PRIMARY KEY NOT NULL,
	--性别
	Gender bit NOT NULL,
	--生日
	Birthday date NULL,
	--手机号码
	CellPhoneNumber varchar(20) NULL,
	--固定电话
	LinePhoneNumber varchar(30) NULL,
	--邮箱1
	Email nvarchar(100) NULL,
	--邮箱2
	Email2 nvarchar(100) NULL,
	QQ varchar(20) NULL,
	--身份证号码
	ID varchar(30) NULL,
	--通信地址
	[Address] nvarchar(200) NULL,
	--照片
	Photo nvarchar(300) NULL
)
Go

--人力成本
CREATE TABLE LaborCost
(
	LaborCostId uniqueidentifier PRIMARY KEY NOT NULL,
	--目标ID
	TargetId uniqueidentifier NOT NULL,
	--目标类型，可以是职位也可以是具体某个人
	TargetType int NOT NULL,
	--报价
	UnitPrice decimal(10, 2) NULL,
	--成本
	UnitValue decimal(10, 2) NULL
)
Go

--项目
CREATE TABLE Project
(
	ProjectId uniqueidentifier PRIMARY KEY NOT NULL,
	ProjectName nvarchar(100) NOT NULL,
	--项目类型 [字典: 101]
	ProjectType varchar(40) NOT NULL,
	--开始时间
	BeginTime datetime NULL,
	--结束时间
	EndTime datetime NULL,
	--项目状态 [字典: 102]
	[Status] varchar(40) NULL,
	--项目经理
	PM uniqueidentifier NULL,
	--项目从属关系
	ParentId uniqueidentifier NULL,
	[Description] nvarchar(500) NULL
)
Go

--项目阶段模板
CREATE TABLE StageTemplete
(
	StageTempleteId uniqueidentifier PRIMARY KEY NOT NULL,
	--项目类型 [字典: 101]
	ProjectType varchar(40) NOT NULL,
	--阶段名称
	StageName nvarchar(50) NOT NULL,
	--阶段描述
	[Description] nvarchar(200) NULL,
	--默认顺序
	SortValue int NULL
)
Go

--项目阶段
CREATE TABLE Stage
(
	StageId uniqueidentifier PRIMARY KEY NOT NULL,
	--阶段名称
	StageName nvarchar(50) NOT NULL,
	--上一个阶段
	PrevStage uniqueidentifier NULL,
	--下一个阶段
	NextStage uniqueidentifier NULL,
	--所属项目
	ProjectId uniqueidentifier NOT NULL,
	--阶段负责人
	Manager uniqueidentifier NULL,
	--预计开始时间
	BeginTime datetime NULL,
	--预计结束时间
	EndTime datetime NULL,
	--状态 [字典: 103]
	[Status] varchar(40) NOT NULL,
	--阶段描述
	[Description] nvarchar(200) NULL,
	--顺序
	SortValue int NULL,

	--外键 -> 项目
	CONSTRAINT FK_Stage_ProjectId FOREIGN KEY(ProjectId) REFERENCES Project(ProjectId)
)
Go

--项目阶段活动
CREATE TABLE Activity
(
	ActivityId uniqueidentifier PRIMARY KEY NOT NULL,
	--所属阶段
	StageId uniqueidentifier NOT NULL,
	--计划
	ActivityText nvarchar(100) NULL,
	--从属
	ParentId uniqueidentifier NULL,
	--计划描述
	[Description] nvarchar(300) NULL,

	--外键 -> 项目
	CONSTRAINT FK_Activity_StageId FOREIGN KEY(StageId) REFERENCES Stage(StageId)
)
Go

--项目团队
CREATE TABLE ProjectTeam
(
	TeamId uniqueidentifier PRIMARY KEY NOT NULL,
	ProjectId uniqueidentifier NOT NULL,
	EmployeeId uniqueidentifier NOT NULL,

	--外键 -> 项目表
	CONSTRAINT FK_ProjectTeam_ProjectId FOREIGN KEY(ProjectId) REFERENCES Project(ProjectId),
	--外键 -> 员工表
	CONSTRAINT FK_ProjectTeam_EmployeeId FOREIGN KEY(EmployeeId) REFERENCES Employee(EmployeeId)
)
Go

--项目预算
CREATE TABLE Budget
(
	BudgetId uniqueidentifier PRIMARY KEY NOT NULL,
	ProjectId uniqueidentifier NOT NULL,
	--预算种类 [字典: 204]
	BudgetCategory varchar(40) NOT NULL,
	--预算资金
	Capital decimal(18, 2) NULL,
	--人月时间预算 (research & development)
	RDTime decimal(10, 2) NULL

	--外键 -> 项目表
	CONSTRAINT FK_Budget_ProjectId FOREIGN KEY(ProjectId) REFERENCES Project(ProjectId)
)
Go

--工时表
CREATE TABLE WorkTime
(
	WorkTimeId uniqueidentifier PRIMARY KEY NOT NULL,
	--记录人
	EmployeeId uniqueidentifier NOT NULL,
	--开始时间
	BeginTime datetime NULL,
	--结束时间
	EndTime datetime NULL,
	--有效工时
	[Hours] decimal(5, 2) NULL,
	--所属项目
	ProjectId uniqueidentifier NULL,
	--所属阶段
	StageId uniqueidentifier NULL,
	--对应计划
	ActivityId uniqueidentifier NULL,
	--工作内容
	JobContent nvarchar(500) NULL,
	--出勤类型 [字典: 101]
	AttendanceType varchar(40) NULL,
	--是否确认
	Approved bit NULL,
	--确认时间
	ApprovedTime datetime NULL,
	--确认人
	ApprovedManager uniqueidentifier NULL,
	--错误标记
	FaultFlag bit NULL,
	--错误原因
	FaultReason nvarchar(100) NULL,
	--对应的工作时间设置项
	WorkTimeSettingId uniqueidentifier NULL
)
Go

--工作时间设置
CREATE TABLE WorkTimeSettings
(
	WorkTimeSettingId uniqueidentifier PRIMARY KEY NOT NULL,
	--上午上班时间
	AMStartTime time NULL,
	--上午下班时间
	AMEndTime time NULL,
	--下午上班时间
	PMStartTime time NULL,
	--下午下班时间
	PMEndTime time NULL,
	[Enabled] bit NOT NULL,
	WorkHours decimal(5, 2) NULL,
	[Description] nvarchar(200) NULL
)
Go

--休息日设置
CREATE TABLE HolidaySettings
(
	--日期
	HolidayDate date PRIMARY KEY NOT NULL,
	--是否为周末
	IsWeekend bit NULL,
	--节日名称
	Festival nvarchar(20) NULL,
	--年份
	SettingYear int NULL
)
Go

CREATE TABLE ProjectMeetingSummarys
(
	MeetingId uniqueidentifier PRIMARY KEY NOT NULL,
	--开始时间
	StartTime datetime NOT NULL,
	--议题
	Content nvarchar(500) NULL,
	--类型
	MeetingTypeId nvarchar(10) NULL,
	--结论
	Result nvarchar(2000) NOT NULL,
	CreateUser nvarchar(100) NULL,
	CreateTime datetime NULL,
	ProjectId uniqueidentifier NOT NULL,
	--StageId uniqueidentifier NOT NULL,
	--MettingEmployeeId uniqueidentifier NOT NUll,
	AccessoryId uniqueidentifier NULL,

	--项目
	CONSTRAINT FK_Meeting_ProjectId FOREIGN KEY(ProjectId) REFERENCES Project(ProjectId),
	--项目阶段
	--CONSTRAINT FK_Stage_StageId FOREIGN KEY(StageId) REFERENCES Stage(StageId),
	--参与人员
	--CONSTRAINT FK_ProjectMeetingEmployees_EmployeeId FOREIGN KEY(MettingEmployeeId) REFERENCES ProjectMeetingEmployees(MettingEmployeeId),
	--附件
	CONSTRAINT FK_MeetingAccessory_AccessoryId FOREIGN KEY(AccessoryId) REFERENCES MeetingAccessory(AccessoryId)
)
Go


CREATE TABLE ProjectMeetingEmployees
(
	MettingEmployeeId uniqueidentifier PRIMARY KEY NOT NULL,
	EmployeeName nvarchar(50) NULL,
	MeetingId uniqueidentifier NOT NULL,
	EmployeeId uniqueidentifier NOT NULL,

	CONSTRAINT FK_ProjectMeetingSummarys_MeetingId FOREIGN KEY(MeetingId) REFERENCES ProjectMeetingSummarys(MeetingId),
	CONSTRAINT FK_Employee_EmployeeId FOREIGN KEY(EmployeeId) REFERENCES Employee(EmployeeId)
)
Go

CREATE TABLE MeetingAccessory
(
	AccessoryId uniqueidentifier PRIMARY KEY NOT NULL,
	Url nvarchar(500) NULL,
	MeetingId uniqueidentifier NOT NULL,

	CONSTRAINT FK_MeetingAccessory_MeetingId FOREIGN KEY(MeetingId) REFERENCES ProjectMeetingSummarys(MeetingId)
)
Go

CREATE TABLE AttendanceInfo
(
	AttendanceInfoId uniqueidentifier PRIMARY KEY NOT NULL,
	DeviceId varchar(50) NULL,
	OutsideUserId nvarchar(50) NULL,
	Clock datetime NULL,
	Remark nvarchar(100) NULL
);
Go

CREATE TABLE AttendanceUsers
(
	UserId uniqueidentifier NOT NULL,
	OutsideUserId varchar(50) NOT NULL,
	CONSTRAINT PK_UserId_OutsideUserId PRIMARY KEY(UserId, OutsideUserId),
);
Go

--请假申请单
CREATE TABLE RequestLeaveBill
(
	Id uniqueidentifier PRIMARY KEY NOT NULL,
	EmployeeId uniqueidentifier NOT NULL,
	--请假类型 [字典: 6]
	LeaveType varchar(40) NULL,
	LeaveStart datetime NOT NULL,
	LeaveEnd datetime NOT NULL,
	WorkDayCount decimal(8, 2) NULL,
	LeaveReason nvarchar(500) NULL,
	--项目经理是否同意
	PMSign uniqueidentifier NULL,
	PMSignDate datetime NULL,
	--部门经理是否同意
	DLSign uniqueidentifier NULL,
	DLSignDate datetime NULL,
	--总经理是否同意
	CEOSign uniqueidentifier NULL,
	CEOSignDate datetime NULL,
	--申请时间
	CreateTime datetime NOT NULL,

	CONSTRAINT FK_ReqestLeaveBill_EmployeeId FOREIGN KEY(EmployeeId) REFERENCES Employee(EmployeeId)
);
Go

--考勤异常单
CREATE TABLE AttendanceExceptionBill
(
	Id uniqueidentifier PRIMARY KEY NOT NULL,
	EmployeeId uniqueidentifier NOT NULL,
	StartTime datetime NOT NULL,
	EndTime datetime NOT NULL,
	Reason nvarchar(500) NULL,
	--领导签字 正/副部门经理；项目经理
	LeaderSign uniqueidentifier NULL,
	LeaderSignDate datetime NULL,
	--申请时间
	CreateTime datetime NOT NULL

	CONSTRAINT FK_AttendanceExceptionBill_EmployeeId FOREIGN KEY(EmployeeId) REFERENCES Employee(EmployeeId)
);
Go

--出差申请单
CREATE TABLE BusinessTripBill
(
	Id uniqueidentifier PRIMARY KEY NOT NULL,
	EmployeeId uniqueidentifier NOT NULL,
	--开始时间
	StartTime datetime NOT NULL,
	--结束时间
	EndTime datetime NOT NULL,
	--目的地
	Destination nvarchar(100) NULL,
	--线路
	[Path] nvarchar(200) NULL,
	--出差地点
	TripReason nvarchar(500) NULL,
	Persons nvarchar(200) NULL,
	RealPersons nvarchar(200) NULL,
	--项目经理是否同意
	PMSign uniqueidentifier NULL,
	PMSignDate datetime NULL,
	--部门经理是否同意
	DLSign uniqueidentifier NULL,
	DLSignDate datetime NULL,
	CreateTime datetime NOT NULL,

	CONSTRAINT FK_BusinessTripBill_EmployeeId FOREIGN KEY(EmployeeId) REFERENCES Employee(EmployeeId)
);
Go

ALTER TABLE ProjectMeetingSummarys
	ADD	CONSTRAINT FK_ProjectMeetingEmployees_EmployeeId FOREIGN KEY(MettingEmployeeId) REFERENCES ProjectMeetingEmployees(MettingEmployeeId),
	--附件
	ADD CONSTRAINT FK_MeetingAccessory_AccessoryId FOREIGN KEY(AccessoryId) REFERENCES MeetingAccessory(AccessoryId)
)



SELECT pms.MeetingId, pms.ProjectId, p.ProjectName, pms.StageId, s.StageName, pms.Content, pms.Result, pms.CreateUser, pms.StartTime, pme.EmployeeName, may.Url 
	FROM ProjectMeetingSummarys pms
INNER JOIN ProjectMeetingEmployees pme ON pms.MeetingId = pme.MeetingId
LEFT JOIN MeetingAccessory may ON pms.MeetingId = may.MeetingId

INNER JOIN Project p ON pms.ProjectId = p.ProjectId
INNER JOIN Stage s ON pms.StageId = s.StageId


alter table ProjectMeetingSummarys drop constraint FK_Stage_StageId

ALTER TABLE ProjectMeetingSummarys drop COLUMN	StageId

alter table MeetingAccessory
add constraint FK_MeetingAccessory_MeetingId foreign key (MeetingId) references ProjectMeetingSummarys(MeetingId)

alter table ProjectMeetingSummarys alter column StartTime datetime

alter table ProjectMeetingSummarys alter column CreateTime datetime