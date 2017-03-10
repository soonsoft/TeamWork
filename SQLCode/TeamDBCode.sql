SELECT * FROM Project

SELECT * FROM sys_DictionaryGroup

--DELETE FROM Project

SELECT projects.*, ue.RealName AS PMName, di1.DictItemValue AS ProjectTypeName, di2.DictItemValue AS StatusName FROM (
	SELECT p.*, ChildrenCount FROM Project p 
		INNER JOIN (SELECT p.ParentId AS ProjectId, COUNT(1) AS ChildrenCount 
			FROM Project p
			GROUP BY p.ParentId HAVING p.ParentId IS NOT NULL) parent
			ON p.ProjectId = parent.ProjectId
		WHERE ChildrenCount > 1
	UNION ALL
	SELECT p.*, 0 AS ChildrenCount FROM Project p 
		INNER JOIN (SELECT p.ParentId AS ProjectId, COUNT(1) AS ChildrenCount 
			FROM Project p
			GROUP BY p.ParentId HAVING p.ParentId IS NOT NULL) parent
			ON p.ParentId = parent.ProjectId
		WHERE ChildrenCount = 1) projects
	LEFT JOIN auth_UserExtend ue ON projects.PM = ue.UserId
	INNER JOIN sys_DictionaryItem di1 ON projects.ProjectType = di1.DictItemId
	LEFT JOIN sys_DictionaryItem di2 ON projects.[Status] = di2.DictItemId

SELECT p.*, ue.RealName AS PMName, di1.DictItemValue AS ProjectTypeName, di2.DictItemValue AS StatusName FROM Project p
	LEFT JOIN auth_UserExtend ue ON p.PM = ue.UserId
	INNER JOIN sys_DictionaryItem di1 ON p.ProjectType = di1.DictItemId
	LEFT JOIN sys_DictionaryItem di2 ON p.[Status] = di2.DictItemId
	WHERE p.ParentId = '3E790733-DF77-4370-9A72-B54E012280CE'

SELECT ue.* FROM auth_Role r
	INNER JOIN auth_UsersInRoles ur ON r.RoleId = ur.RoleId
	INNER JOIN auth_UserExtend ue ON ur.UserId = ue.UserId
	--INNER JOIN Employee e ON e.EmployeeId = ue.UserId
	WHERE r.RoleName = 'PM'

SELECT u.UserName, u.[Enabled], ue.*, e.* FROM auth_User u
	INNER JOIN auth_UserExtend ue ON u.UserId = ue.UserId
	LEFT JOIN Employee e ON ue.UserId = e.EmployeeId

SELECT * FROM Employee

--DELETE [Employee]

SELECT * FROM Organization
SELECT NEWID()
--INSERT INTO Organization VALUES('69CDBA6A-58CB-4BC7-AFF6-3DA2E10FA8B2', '南京思创', '南京思创信息技术有限公司', '南京鼓楼区中央路399号天正国际广场六号楼506室/801室/802室', '025-8223-0530', NULL, '王迅', NULL, '2-02', NULL);

SELECT * FROM Department
--INSERT INTO Department VALUES('C75FF911-DDC7-463D-AA1E-E769A09C9E18', '69CDBA6A-58CB-4BC7-AFF6-3DA2E10FA8B2', '研发中心一部', NULL, NULL);
--INSERT INTO Department VALUES('BD6B9184-F939-45FB-9782-62772BEF3CEA', '69CDBA6A-58CB-4BC7-AFF6-3DA2E10FA8B2', '研发中心二部', NULL, NULL);

SELECT * FROM Position
/*
INSERT INTO Position VALUES('BA90DC77-68C9-40C9-BE5E-66C14FDE7024', NULL, '分析顾问', NULL);
INSERT INTO Position VALUES('6C4800FE-765C-42BB-B7F6-8ADBDF550F79', NULL, '运营顾问', NULL);
INSERT INTO Position VALUES('3D9481CE-5157-48D3-B16A-0C071E402AFB', NULL, '系统架构师', NULL);
INSERT INTO Position VALUES('90E17554-50C9-4737-A6DD-248B42A2771A', NULL, '数据库架构师', NULL);
INSERT INTO Position VALUES('BF300B8E-4753-407A-86E1-0C56DFDFF687', NULL, 'UI设计师', NULL);
INSERT INTO Position VALUES('C2BC7565-B904-476E-9FDC-5A808FAB5C08', NULL, 'Web开发工程师', NULL);
INSERT INTO Position VALUES('28DE6EA5-B223-4211-9C60-E1B252064D2D', NULL, '软件开发工程师', NULL);
INSERT INTO Position VALUES('ED6EBAE3-9369-4B30-A28E-A0CF35CC2E67', NULL, '高级软件开发工程师', NULL);
INSERT INTO Position VALUES('EFD717B2-26B5-4F09-8546-CAD061447A60', NULL, 'GIS开发工程师', NULL);
INSERT INTO Position VALUES('F7CA1CA8-B872-444B-A43C-7F17CA74C293', NULL, '数据库开发工程师', NULL);
INSERT INTO Position VALUES('D88EDF3F-15AB-4995-8580-39508AF429B5', NULL, '文档工程师', NULL);
INSERT INTO Position VALUES('4DE0C576-0D80-4762-895B-7F22A28EEF66', NULL, '测试经理', NULL);
INSERT INTO Position VALUES('9E2C824D-ACD3-405A-A518-268C688D2806', NULL, '测试工程师', NULL);
INSERT INTO Position VALUES('66860AC2-186E-4A87-9518-DE105F725524', NULL, '培训经理', NULL);
INSERT INTO Position VALUES('CD58F3A9-98BE-41B9-A0AC-D8A84E2391E6', NULL, '项目经理', NULL);
INSERT INTO Position VALUES('87B6B6C0-C7AE-4718-8F26-1B4AB6A8FE67', NULL, '部门经理', NULL);

*/

SELECT * FROM StageTemplete;
/*
INSERT INTO StageTemplete VALUES('57FAE712-E633-4FE2-96C8-3026C3DDD430', '101-01', '售前', NULL);
INSERT INTO StageTemplete VALUES('1E6FCED0-C33B-4650-B139-FCC0D0FFC40A', '101-01', '需求调研', NULL);
INSERT INTO StageTemplete VALUES('E7D09B6B-C5B5-4A01-B9D8-9FD5FF837230', '101-01', '设计', NULL);
INSERT INTO StageTemplete VALUES('6A0D6503-9800-4A8D-B7C9-D5E8242EB645', '101-01', '开发', NULL);
INSERT INTO StageTemplete VALUES('166E0D82-087C-456B-AEAC-19E9FC261A21', '101-01', '实施', NULL);
INSERT INTO StageTemplete VALUES('C42C50B2-7D00-43E8-895A-BF032096F56A', '101-01', '维护', NULL);
*/

SELECT u.*, r.RoleId, r.RoleName, r.RoleText FROM 
(SELECT TOP(100) u.UserName, u.[Enabled], ue.*, e.* 
		FROM auth_User u 
		INNER JOIN auth_UserExtend ue ON u.UserId = ue.UserId 
		LEFT JOIN Employee e ON ue.UserId = e.EmployeeId WHERE 1 = 1 ORDER BY ue.RealName, u.UserName) u 
LEFT JOIN auth_UsersInRoles ur ON u.UserId = ur.UserId 
LEFT JOIN auth_Role r ON ur.RoleId = r.RoleId 
LEFT JOIN sys_DictionaryItem di ON u.[Status] = di.DictItemId
ORDER BY u.RealName, u.UserName, r.RoleName

SELECT * FROM Employee
SELECT * FROM auth_UserExtend

SELECT SUBSTRING(CONVERT(NVARCHAR, GETDATE(), 120), 0, 5)
SELECT CASE(GETDATE() AS date)

SELECT t.DateText FROM(
SELECT SUBSTRING(CONVERT(varchar, BeginTime, 120), 0, 11) AS DateText FROM WorkTime
	WHERE YEAR(BeginTime) = 2014) t
	GROUP BY t.DateText

SELECT * FROM WorkTime

SELECT w.WorkTimeId, w.BeginTime, w.EndTime, w.ProjectId, p.ProjectName, w.FaultFlag FROM WorkTime w
	INNER JOIN Project p ON w.ProjectId = p.ProjectId
	WHERE YEAR(w.BeginTime) = 2014 AND MONTH(w.BeginTime) = 6

SELECT p.ProjectId, p.ProjectName, s.StageId, s.StageName FROM Project p
	INNER JOIN Stage s ON p.ProjectId = s.ProjectId AND s.[Status] = '103-01'
	INNER JOIN ProjectTeam pt ON p.ProjectId = pt.ProjectId
	WHERE pt.EmployeeId

SELECT * FROM WorkTimeSettings

SELECT * FROM WorkTime
--DELETE FROM WorkTime


SELECT w.*, p.ProjectName, s.StageName, a.ActivityText, di.DictItemValue AS AttendanceTypeName, ue.RealName AS ApprovedManagerName FROM WorkTime w
	LEFT JOIN Project p ON w.ProjectId = p.ProjectId
	LEFT JOIN Stage s ON w.StageId = s.StageId
	LEFT JOIN Activity a ON w.ActivityId = a.ActivityId
	LEFT JOIN auth_UserExtend ue ON w.ApprovedManager = ue.UserId
	INNER JOIN sys_DictionaryItem di ON w.AttendanceType = di.DictItemId









