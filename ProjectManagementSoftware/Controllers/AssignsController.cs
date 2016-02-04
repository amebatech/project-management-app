using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using Revoluza.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;

namespace Revoluza.Controllers
{
    public class AssignsController : Controller
    {
        private MongoDatabase mongoDatabase;
        
        public AssignsController()
        {
            // create connectionstring
            var connect = "mongodb://localhost";
            var Client = new MongoClient(connect);

            // get Reference of server
            var Server = Client.GetServer();

            // get Reference of Database
            mongoDatabase = Server.GetDatabase("School");
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetAll()
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");

            IList<Project> lstProject = new List<Project>();
            var getProject = _project.FindAs(typeof(Project), Query.NE("Name", "null"));
            IList<Task> lstTask = new List<Task>();
            var getTask = _task.FindAs(typeof(Task), Query.NE("Name", "null"));
            foreach (Project proj in getProject)
            {
                string empId = proj.Manager;
                ObjectId pmId;
                var isValid = ObjectId.TryParse(empId, out pmId);
                if (isValid)
                {
                    var getQuery = Query<Employee>.EQ(e => e.EmpId, pmId);
                    var emp = _employee.FindOne(getQuery);
                    if (emp != null)
                    {
                        proj.ProjectManager = emp.Fullname;
                    }
                }
                lstProject.Add(proj);
            }

            foreach (Task tsk in getTask)
            {
                string empId = tsk.ManagerId;
                ObjectId pmId;
                var isValid = ObjectId.TryParse(empId, out pmId);
                if (isValid)
                {
                    var getQuery = Query<Employee>.EQ(e => e.EmpId, pmId);
                    var emp = _employee.FindOne(getQuery);
                    if (emp != null)
                    {
                        tsk.ManagerName = emp.Fullname;
                    }
                }
                string projectId = tsk.ProjectId;
                ObjectId projId;
                isValid = ObjectId.TryParse(projectId, out projId);
                if (isValid)
                {
                    var getQuery = Query<Project>.EQ(e => e.ProjectId, projId);
                    var proj = _project.FindOne(getQuery);
                    if (proj != null)
                    {
                        tsk.ProjectName = proj.Name;
                    }
                }

                lstTask.Add(tsk);
            }

            ViewData["Task"] = lstTask;

            return PartialView("GetAll", lstProject);
        }

        public PartialViewResult Search(string searchVal)
        {
            var collections = mongoDatabase.GetCollection<Task>("Task");
            IList<Task> listSearchTask = new List<Task>();
            if (searchVal == "")
            {
                var getTask = collections.FindAs(typeof(Task), Query.NE("Name", "null"));
                foreach (Task task in getTask)
                {
                    listSearchTask.Add(task);
                }
            }
            else
            {
                var queryTask = Query.Or(
                    Query.Matches("Name", new BsonRegularExpression(searchVal, "i")), 
                    Query.Matches("Description", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("StartDate", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("EndDate", new BsonRegularExpression(searchVal, "i")));
                var getTask = collections.Find(queryTask);

                foreach (Task task in getTask)
                {
                    if (listSearchTask.Contains(task) == false)
                    {
                        listSearchTask.Add(task);
                    }
                }
            }
            return PartialView("GetAll", listSearchTask);
        }

        #region create new record
        public PartialViewResult Create()
        {
            IList<Project> listProject = new List<Project>();
            var collections = mongoDatabase.GetCollection<Project>("Project");
            var getAllProjects = collections.FindAs(typeof(Project), Query.EQ("IsActive", true));
            foreach (Project proj in getAllProjects)
            {
                listProject.Add(proj);
            }
            var selectList = new SelectList(listProject, "ProjectId", "Name", 1);
            ViewData["Project"] = selectList;

            IList<Employee> listEmployee = new List<Employee>();
            var empcollection = mongoDatabase.GetCollection<Employee>("Employee");
            var getAllEmployees = empcollection.FindAs(typeof(Employee), Query.EQ("IsReportingHead", true));
            foreach (Employee emp in getAllEmployees)
            {
                listEmployee.Add(emp);
            }
            var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", 1);
           
            ViewData["Employee"] = selectEmp;
            
            return PartialView();
        }

        #region create new record
        public PartialViewResult AddTask(string Id)
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(Id, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                IList<Project> listProj = new List<Project>();
                var proj = _project.FindOne(getQuery);
                listProj.Add(proj);
                var selectProj = new SelectList(listProj, "ProjectId", "Name", proj.ProjectId);
                ViewData["Project"] = selectProj;
            }

            IList<Employee> listEmployee = new List<Employee>();
            var empcollection = mongoDatabase.GetCollection<Employee>("Employee");
            var getAllEmployees = empcollection.FindAs(typeof(Employee), Query.NE("IsActive", false));
            foreach (Employee emp in getAllEmployees)
            {
                listEmployee.Add(emp);
            }
            var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", 1);

            ViewData["Employee"] = selectEmp;

            return PartialView();
        }

        public PartialViewResult AddSubTask(string Id)
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");

            string projId = "";
            string managerId = "";

            ObjectId taskId;
            var isValid = ObjectId.TryParse(Id, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                IList<Task> listTask = new List<Task>();
                var tsk = _task.FindOne(getQuery);
                projId = tsk.ProjectId.ToString();
                managerId = tsk.ManagerId; 
                listTask.Add(tsk);
                var selectTask = new SelectList(listTask, "TaskId", "Name", tsk.TaskId);
                ViewData["Task"] = selectTask;
            }

            ObjectId projectId;
            isValid = ObjectId.TryParse(projId, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                IList<Project> listProj = new List<Project>();
                var proj = _project.FindOne(getQuery);
                listProj.Add(proj);
                var selectProj = new SelectList(listProj, "ProjectId", "Name", proj.ProjectId);
                ViewData["Project"] = selectProj;
            }

            ObjectId EmpId;
            isValid = ObjectId.TryParse(managerId, out EmpId);
            if (isValid)
            {
                var getQuery = Query<Employee>.EQ(e => e.EmpId, EmpId);
                IList<Employee> listEmp = new List<Employee>();
                var emp = _employee.FindOne(getQuery);
                listEmp.Add(emp);
                var selectEmp = new SelectList(listEmp, "EmpId", "FullName", managerId);

                ViewData["Employee"] = selectEmp;
            }

            return PartialView();
        }

        [HttpPost]
        public PartialViewResult Create(Task tsk)
        {
            if (ModelState.IsValid)
            {
                var _task = mongoDatabase.GetCollection<Task>("Task");
                ObjectId taskId;
                var isValid = ObjectId.TryParse(tsk.ParentId, out taskId);
                if (isValid)
                {
                    var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                    var stsk = _task.FindOne(getQuery);
                    tsk.ParentName = stsk.Name;
                }
                _task.Insert(tsk);
                var id = tsk.TaskId;
                return GetAll();
            }
            else
            {
                return PartialView(tsk);
            }
        }

        [HttpPost]
        public PartialViewResult SaveMonitor(DevBase monti)
        {
            if (ModelState.IsValid)
            {
                var _monitor = mongoDatabase.GetCollection<DevBase>("DevBase");
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                var _task = mongoDatabase.GetCollection<Task>("Task");

                ObjectId EmpId;
                var isValid = ObjectId.TryParse(monti.EmpId, out EmpId);
                if (isValid)
                {
                    var getQuery = Query<Employee>.EQ(e => e.EmpId, EmpId);
                    var emp = _employee.FindOne(getQuery);
                    monti.EmpName = emp.Fullname;
                }

                ObjectId taskId;
                isValid = ObjectId.TryParse(monti.TaskId, out taskId);
                if (isValid)
                {
                    var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                    var tsk = _task.FindOne(getQuery);
                    monti.TaskName = tsk.Name; 
                }

                _monitor.Insert(monti);
                var id = monti.Id;
                return AddEmployee(monti.TaskId);
            }
            else
            {
                return AddEmployee(monti.TaskId);
            }
        }
        #endregion

        #region Edit Record
        public PartialViewResult Edit(string Id)
        {
            var collections = mongoDatabase.GetCollection<Task>("Task");

            ObjectId taskId;
            var isValid = ObjectId.TryParse(Id, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                var tsk = collections.FindOne(getQuery);
                if (tsk.ParentId != null)
                {
                    return EditSub(Id);
                }
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                var _project = mongoDatabase.GetCollection<Project>("Project");
                IList<Project> listProject = new List<Project>();
                var getAllProjects = _project.FindAs(typeof(Project), Query.EQ("IsActive", true));
                foreach (Project proj in getAllProjects)
                {
                    listProject.Add(proj);
                }
                var selectList = new SelectList(listProject, "ProjectId", "Name", tsk.ProjectId);
                ViewData["Project"] = selectList;

                IList<Employee> listEmployee = new List<Employee>();
                var getAllEmployees = _employee.FindAs(typeof(Employee), Query.EQ("IsReportingHead", true));
                foreach (Employee emp in getAllEmployees)
                {
                    listEmployee.Add(emp);
                }
                var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", tsk.ManagerId);

                ViewData["Employee"] = selectEmp;

                return PartialView("Edit", tsk);
            }
            else
            {
                return PartialView("Edit");
            }
        }

        public PartialViewResult EditSub(string Id)
        {
            var _task = mongoDatabase.GetCollection<Task>("Task");
            string parentTaskId = "";

            ObjectId taskId;
            var isValid = ObjectId.TryParse(Id, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                var tsk = _task.FindOne(getQuery);
                parentTaskId = tsk.ParentId; 
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                var _project = mongoDatabase.GetCollection<Project>("Project");
                IList<Project> listProject = new List<Project>();
                var getAllProjects = _project.FindAs(typeof(Project), Query.EQ("IsActive", true));
                foreach (Project proj in getAllProjects)
                {
                    listProject.Add(proj);
                }
                var selectList = new SelectList(listProject, "ProjectId", "Name", tsk.ProjectId);
                ViewData["Project"] = selectList;

                IList<Employee> listEmployee = new List<Employee>();
                var getAllEmployees = _employee.FindAs(typeof(Employee), Query.EQ("IsReportingHead", true));
                foreach (Employee emp in getAllEmployees)
                {
                    listEmployee.Add(emp);
                }
                var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", tsk.ManagerId);

                ViewData["Employee"] = selectEmp;

                ObjectId parentId;
                isValid = ObjectId.TryParse(parentTaskId, out parentId);
                if (isValid)
                {
                    getQuery = Query<Task>.EQ(e => e.TaskId, parentId);
                    IList<Task> listTask = new List<Task>();
                    var ptsk = _task.FindOne(getQuery);
                    listTask.Add(ptsk);
                    var selectTask = new SelectList(listTask, "TaskId", "Name", ptsk.TaskId);
                    ViewData["Task"] = selectTask;
                }

                return PartialView("EditSub", tsk);
            }
            else
            {
                return PartialView("Error");
            }
        }

        public PartialViewResult EditMonitor(string Id)
        {
            var _monitor = mongoDatabase.GetCollection<DevBase>("DevBase");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");

            ObjectId monId;
            var isValid = ObjectId.TryParse(Id, out monId);
            if (isValid)
            {
                var getQuery = Query<DevBase>.EQ(e => e.Id, monId);
                var mon = _monitor.FindOne(getQuery);

                ObjectId taskId;
                isValid = ObjectId.TryParse(mon.TaskId, out taskId);
                if (isValid)
                {
                    getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                    IList<Task> listTask = new List<Task>();
                    var tsk = _task.FindOne(getQuery);
                    listTask.Add(tsk);
                    var selectTask = new SelectList(listTask, "TaskId", "Name", tsk.TaskId);
                    ViewData["Task"] = selectTask;
                }

                ObjectId empId;
                isValid = ObjectId.TryParse(mon.EmpId, out empId);
                if (isValid)
                {
                    getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
                    IList<Employee> listEmp = new List<Employee>();
                    var emp = _employee.FindOne(getQuery);
                    if (emp != null && emp.IsActive == true)
                    {
                        listEmp.Add(emp);
                        var selectEmp = new SelectList(listEmp, "EmpId", "FullName", emp.EmpId);
                        ViewData["Employee"] = selectEmp;
                    }
                }

                List<Status> listStatus = new List<Status>();

                Status stT3 = new Status() { Name = "Not Started" };
                listStatus.Add(stT3);
                Status stT1 = new Status() { Name = "In-Process" };
                listStatus.Add(stT1);
                Status stT2 = new Status() { Name = "Complete" };
                listStatus.Add(stT2);
                Status stT5 = new Status() { Name = "Waiting Approval" };
                listStatus.Add(stT5);
                Status stT6 = new Status() { Name = "On Hold" };
                listStatus.Add(stT6);
                Status stT4 = new Status() { Name = "Cancelled" };
                listStatus.Add(stT4);

                var selectSt = new SelectList(listStatus, "Name", "Name", mon.Status);
                ViewData["Status"] = selectSt;

                return PartialView("EditMonitor", mon);
            }
            else
            {
                return PartialView("Error");
            }
        }
        #endregion

        [HttpPost]
        public PartialViewResult Edit(Task tsk, string TaskId)
        {
            ObjectId taskId;
            var isValid = ObjectId.TryParse(TaskId, out taskId);
            if (isValid)
            {
                var collections = mongoDatabase.GetCollection<Task>("Task");
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                var existingtask = collections.FindOne(getQuery);
                existingtask.Name = tsk.Name;
                existingtask.StartDate = tsk.StartDate;
                existingtask.EndDate = tsk.EndDate;
                existingtask.Description = tsk.Description;
                existingtask.ManagerId = tsk.ManagerId;
                existingtask.ProjectId = tsk.ProjectId;
                existingtask.IsComplete = tsk.IsComplete;
                existingtask.IsActive = tsk.IsActive;
                collections.Save(existingtask);
                return GetAll();
            }
            else
            {
                return PartialView("Edit");
            }
        }

        [HttpPost]
        public PartialViewResult EditSubTask(Task tsk, string TaskId)
        {
            ObjectId taskId;
            var isValid = ObjectId.TryParse(TaskId, out taskId);
            if (isValid)
            {
                var collections = mongoDatabase.GetCollection<Task>("Task");
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                var existingtask = collections.FindOne(getQuery);
                existingtask.Name = tsk.Name;
                existingtask.StartDate = tsk.StartDate;
                existingtask.EndDate = tsk.EndDate;
                existingtask.Description = tsk.Description;
                existingtask.ManagerId = tsk.ManagerId;
                existingtask.ProjectId = tsk.ProjectId;
                existingtask.IsComplete = tsk.IsComplete;
                existingtask.IsActive = tsk.IsActive;
                collections.Save(existingtask);
                return GetAll();
            }
            else
            {
                return PartialView("Error");
            }
        }

        [HttpPost]
        public PartialViewResult UpdateMonitor(DevBase monti, string Id)
        {
            if(Id != null)
            {
                var _task = mongoDatabase.GetCollection<Task>("Task");
                var _monitor = mongoDatabase.GetCollection<DevBase>("DevBase");
                var getQuery = Query<DevBase>.EQ(e => e.TaskId, Id);
                var existingmon = _monitor.FindOne(getQuery);
                existingmon.EmpId = monti.EmpId;
                existingmon.TaskId = monti.TaskId;
                existingmon.Status = monti.Status;
                existingmon.AsOnDate = monti.AsOnDate;
                existingmon.CompleteByDate = monti.CompleteByDate;
                existingmon.CompletedDate = monti.CompletedDate;
                existingmon.PercentComplete = monti.PercentComplete;
                existingmon.Remarks = monti.Remarks;
                existingmon.ClientRemarks = monti.ClientRemarks;
                _monitor.Save(existingmon);
                return AddEmployee(monti.TaskId);
            }
            else
            {
                return PartialView("Error");
            }
        }
        #endregion

        #region Get Details By Id
        public PartialViewResult Details(string Id)
        {
            var collections = mongoDatabase.GetCollection<Task>("Task");

            ObjectId taskId;
            var isValid = ObjectId.TryParse(Id, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                Task tsk = collections.FindOne(getQuery);
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                var _project = mongoDatabase.GetCollection<Project>("Project");
                string empId = tsk.ManagerId;
                ObjectId pmId;
                isValid = ObjectId.TryParse(empId, out pmId);
                if (isValid)
                {
                    getQuery = Query<Employee>.EQ(e => e.EmpId, pmId);
                    var emp = _employee.FindOne(getQuery);
                    if (emp != null)
                    {
                        tsk.ManagerName = emp.Fullname;
                    }
                }
                string projectId = tsk.ProjectId;
                ObjectId projId;
                isValid = ObjectId.TryParse(projectId, out projId);
                if (isValid)
                {
                    getQuery = Query<Project>.EQ(e => e.ProjectId, projId);
                    var proj = _project.FindOne(getQuery);
                    if (proj != null)
                    {
                        tsk.ProjectName = proj.Name;
                    }
                }
                if (tsk.ParentId != null)
                {
                    if (tsk.ParentName != "")
                    {
                        return PartialView("Details", tsk);
                    }
                    else
                    {
                        ObjectId tId;
                        isValid = ObjectId.TryParse(tsk.ParentId, out tId);
                        if (isValid)
                        {
                            getQuery = Query<Task>.EQ(e => e.TaskId, tId);
                            var ptask = collections.FindOne(getQuery);
                            if (ptask != null)
                            {
                                tsk.ParentName = ptask.Name;
                            }
                        }
                        return PartialView("DetailSub", tsk);
                    }
                }
                else
                {
                    tsk.ParentName = "-";
                }
                return PartialView("Details", tsk);
            }
            else
            {
                return PartialView("Error");
            }
        }
        #endregion

        #region Get Project Details By Id
        public PartialViewResult ProjectDetails(string Id)
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(Id, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                Project proj = _project.FindOne(getQuery);
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                string empId = proj.Manager;
                ObjectId pmId;
                isValid = ObjectId.TryParse(empId, out pmId);
                if (isValid)
                {
                    getQuery = Query<Employee>.EQ(e => e.EmpId, pmId);
                    var emp = _employee.FindOne(getQuery);
                    if (emp != null)
                    {
                        proj.ProjectManager = emp.Fullname;
                    }
                }
                return PartialView("ProjectDetails", proj);
            }
            else
            {
                return PartialView("Error");
            }
        }
        #endregion

        #region Delete Records
        public PartialViewResult Delete(string Id)
        {
            var collections = mongoDatabase.GetCollection<Task>("Task");

            ObjectId taskId;
            var isValid = ObjectId.TryParse(Id, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                collections.Remove(getQuery);
                return GetAll();
            }
            else
            {
                return PartialView("Error");
            }
        }
        #endregion

        public PartialViewResult AddEmployee(string Id)
        {
            Boolean flag = false;
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Employee> listEmployee = new List<Employee>();
            IList<Employee> listEmpTask = new List<Employee>();

            var _task = mongoDatabase.GetCollection<Task>("Task");
            ObjectId taskId;
            var isValid = ObjectId.TryParse(Id, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                var tsk = _task.FindOne(getQuery);
                ViewData["Task"] = tsk;
                if (tsk.AssignedTo != null)
                {
                    foreach (Employee empTask in tsk.AssignedTo)
                    {
                        getQuery = Query<Employee>.EQ(e => e.EmpId, empTask.EmpId);
                        var rtemp = _employee.FindOne(getQuery);
                        if (rtemp != null && rtemp.IsActive == true)
                        {
                            listEmpTask.Add(empTask);
                        }
                    }
                    ViewData["Employee"] = listEmpTask;
                }
            }

            var getEmployee = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
            foreach (Employee emp in getEmployee)
            {
                string empId = emp.ReportingTo;
                ObjectId pmId;
                isValid = ObjectId.TryParse(empId, out pmId);
                if (isValid)
                {
                    var getQuery = Query<Employee>.EQ(e => e.EmpId, pmId);
                    var rtemp = _employee.FindOne(getQuery);
                    if (rtemp != null)
                    {
                        emp.ReportingHead = rtemp.Fullname;
                    }
                }
                int count = -1;
                count = listEmpTask.IndexOf(emp);
                foreach (Employee temp in listEmpTask)
                {
                    if (temp.EmpId.ToString().Equals(emp.EmpId.ToString()))
                    {
                        flag = true;
                    }
                }
                if (flag == false)
                {
                    listEmployee.Add(emp);
                }
                else
                {
                    flag = false;
                }
            }
            return PartialView("AddEmployee", listEmployee);
        }

        public PartialViewResult AddManager(string Id)
        {
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Employee> listEmployee = new List<Employee>();

            var _project = mongoDatabase.GetCollection<Project>("Project");
            IList<Project> listProject = new List<Project>();
            ObjectId projectId;
            var isValid = ObjectId.TryParse(Id, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                var proj = _project.FindOne(getQuery);
                if (proj != null)
                {
                    ViewData["Project"] = proj;
                }
            }

            var getEmployee = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
            foreach (Employee emp in getEmployee)
            {
                string empId = emp.ReportingTo;
                ObjectId pmId;
                isValid = ObjectId.TryParse(empId, out pmId);
                if (isValid)
                {
                    var getQuery = Query<Employee>.EQ(e => e.EmpId, pmId);
                    var rtemp = _employee.FindOne(getQuery);
                    if (rtemp != null)
                    {
                        emp.ReportingHead = rtemp.Fullname;
                    }
                }
                listEmployee.Add(emp);
            }
            return PartialView("AddManager", listEmployee);
        }

        public PartialViewResult AssignManager(string Id, string pId)
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Project> listProject = new List<Project>();
            ObjectId projectId;
            var isValid = ObjectId.TryParse(pId, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                var proj = _project.FindOne(getQuery);
                if (proj != null)
                {
                    ObjectId EmpId;
                    isValid = ObjectId.TryParse(Id, out EmpId);
                    if (isValid)
                    {
                        getQuery = Query<Employee>.EQ(e => e.EmpId, EmpId);
                        var emp = _employee.FindOne(getQuery);
                        proj.Manager = emp.EmpId.ToString();
                        _project.Save(proj);
                        return AddManager(pId);
                    }
                }
            }
            return PartialView();
        }

        public PartialViewResult AssignTask(string Id, string tId)
        {
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Task> listTask = new List<Task>();
            ObjectId taskId;
            var isValid = ObjectId.TryParse(tId, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                Task tsk = _task.FindOne(getQuery);
                if (tsk != null)
                {
                    ObjectId EmpId;
                    isValid = ObjectId.TryParse(Id, out EmpId);
                    if (isValid)
                    {
                        getQuery = Query<Employee>.EQ(e => e.EmpId, EmpId);
                        Employee emp = _employee.FindOne(getQuery);
                        tsk.AssignedTo.Add(emp);
                        _task.Save(tsk);
                        return AddEmployee(tId);
                    }
                }
            }
            return PartialView();
        }

        public PartialViewResult DeleteManager(string Id, string pId)
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Project> listProject = new List<Project>();
            ObjectId projectId;
            var isValid = ObjectId.TryParse(pId, out projectId);
            if (isValid)
            {
                var getProject = Query<Project>.EQ(e => e.ProjectId, projectId);
                var proj = _project.FindOne(getProject);
                if (proj != null)
                {
                    proj.Manager = "";
                    proj.ProjectManager = "";
                    _project.Save(proj);
                    return AddManager(pId);
                }
            }
            return PartialView("AddManager");
        }

        public PartialViewResult DeleteEmployee(string Id, string tId)
        {
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Task> listTask = new List<Task>();
            ObjectId taskId;
            var isValid = ObjectId.TryParse(tId, out taskId);
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                var tsk = _task.FindOne(getQuery);
                if (tsk != null)
                {
                    ObjectId EmpId;
                    isValid = ObjectId.TryParse(Id, out EmpId);
                    if (isValid)
                    {
                        getQuery = Query<Employee>.EQ(e => e.EmpId, EmpId);
                        Employee emp = _employee.FindOne(getQuery);
                        int cntE = 0;
                        foreach (Employee rmvEmp in tsk.AssignedTo)
                        {
                            if (rmvEmp != null && rmvEmp.EmpId.ToString().Equals(emp.EmpId.ToString()))
                            {
                                tsk.AssignedTo.RemoveAt(cntE);
                                _task.Save(tsk);
                                return AddEmployee(tId);
                            }
                            else if (rmvEmp == null && emp != null)
                            {
                                tsk.AssignedTo.RemoveAt(cntE);
                                _task.Save(tsk);
                                return AddEmployee(tId);
                            }
                            else
                            {
                                cntE++;
                            }
                        }
                        
                    }
                }
            }
            return PartialView();
        }

        public PartialViewResult Monitor(string Id)
        {

            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            var _monitor = mongoDatabase.GetCollection<DevBase>("DevBase");

            var getBase = _monitor.FindAs(typeof(DevBase), Query.EQ("TaskId", Id));

            if (getBase.Count() > 0)
            {
                foreach (DevBase mon in getBase)
                {
                    if (mon.EmpId.Equals(Id) && mon != null)
                    {
                        return EditMonitor(mon.Id.ToString());
                    }
                }
            }

            ObjectId taskId;
            var isValid = ObjectId.TryParse(Id, out taskId);
            string EmpId = "";
            if (isValid)
            {
                var getQuery = Query<Task>.EQ(e => e.TaskId, taskId);
                IList<Task> listTask = new List<Task>();
                var tsk = _task.FindOne(getQuery);
                EmpId = tsk.AssignedTo[0].EmpId.ToString();
                listTask.Add(tsk);
                var selectTask = new SelectList(listTask, "TaskId", "Name", tsk.TaskId);
                ViewData["Task"] = selectTask;
            }

            ObjectId empId;
            isValid = ObjectId.TryParse(EmpId, out empId);
            if (isValid)
            {
                var getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
                IList<Employee> listEmp = new List<Employee>();
                var emp = _employee.FindOne(getQuery);
                listEmp.Add(emp);
                var selectEmp = new SelectList(listEmp, "EmpId", "FullName", emp.EmpId);
                ViewData["Employee"] = selectEmp;
            }

            List<Status> listStatus = new List<Status>();

            Status stT3 = new Status() { Name = "Not Started" };
            listStatus.Add(stT3);
            Status stT1 = new Status() { Name = "In-Process" };
            listStatus.Add(stT1);
            Status stT2 = new Status() { Name = "Complete" };
            listStatus.Add(stT2);
            Status stT5 = new Status() { Name = "Waiting Approval" };
            listStatus.Add(stT5);
            Status stT6 = new Status() { Name = "On Hold" };
            listStatus.Add(stT6);
            Status stT4 = new Status() { Name = "Cancelled" };
            listStatus.Add(stT4);

            var selectSt = new SelectList(listStatus, "Name", "Name", listStatus[1].Name);
            ViewData["Status"] = selectSt;

            return PartialView();
        }
    }
}