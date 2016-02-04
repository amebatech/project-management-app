using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using Revoluza.Models;
using System;
using System.Linq;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;

namespace Revoluza.Controllers
{
    public class TasksController : Controller
    {
        private MongoDatabase mongoDatabase;
        public TasksController()
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
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            var _project = mongoDatabase.GetCollection<Project>("Project");
            IList<Task> lstTask = new List<Task>();
            var getTask = _task.FindAs(typeof(Task), Query.NE("Name", "null"));
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
            return PartialView("GetAll", lstTask);
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
            var getAllProjects = collections.FindAs(typeof(Project), Query.NE("Name", "null"));
            foreach (Project proj in getAllProjects)
            {
                listProject.Add(proj);
            }
            var selectList = new SelectList(listProject, "ProjectId", "Name", 1);
            ViewData["Project"] = selectList;

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


        [HttpPost]
        public PartialViewResult Create(Task tsk)
        {
            if (ModelState.IsValid)
            {
                var collections = mongoDatabase.GetCollection<Task>("Task");
                collections.Insert(tsk);
                var id = tsk.TaskId;
                return GetAll();
            }
            else
            {
                return PartialView(tsk);
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
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                var _project = mongoDatabase.GetCollection<Project>("Project");
                IList<Project> listProject = new List<Project>();
                //var collections = mongoDatabase.GetCollection<Project>("Project");
                var getAllProjects = _project.FindAs(typeof(Project), Query.NE("Name", "null"));
                foreach (Project proj in getAllProjects)
                {
                    listProject.Add(proj);
                }
                var selectList = new SelectList(listProject, "ProjectId", "Name", tsk.ProjectId);
                ViewData["Project"] = selectList;

                IList<Employee> listEmployee = new List<Employee>();
                //var empcollection = mongoDatabase.GetCollection<Employee>("Employee");
                var getAllEmployees = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
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
                //existingtask.TotalEmployees = tsk.TotalEmployees;
                //existingtask.AssignedTo = task.AssignedTo;
                collections.Save(existingtask);
                return GetAll();
            }
            else
            {
                return PartialView("Edit");
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
                return PartialView("Details", tsk);
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
    }
}