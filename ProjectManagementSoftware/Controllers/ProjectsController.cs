using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using Revoluza.Models;
using System;
using System.Linq;

namespace Revoluza.Controllers
{
    public class ProjectsController : Controller
    {
        private MongoDatabase mongoDatabase;
        public ProjectsController()
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
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Project> listProject = new List<Project>();
            var getProject = _project.FindAs(typeof(Project), Query.NE("Name", "null"));
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

                listProject.Add(proj);
            }
            return PartialView("GetAll", listProject);
        }

        public PartialViewResult Search(string searchVal)
        {
            var collections = mongoDatabase.GetCollection<Project>("Project");
            IList<Project> listSearchProject = new List<Project>();
            if (searchVal == "")
            {
                var getProject = collections.FindAs(typeof(Project), Query.NE("Name", "null"));
                foreach (Project proj in getProject)
                {
                    listSearchProject.Add(proj);
                }
            }
            else
            {
                var queryProject = Query.Or(
                    Query.Matches("Name", new BsonRegularExpression(searchVal, "i")), 
                    Query.Matches("Description", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Url", new BsonRegularExpression(searchVal, "i")), 
                    Query.Matches("Client", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("StartDate", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("EndDate", new BsonRegularExpression(searchVal, "i")));
                
                var getProject = collections.Find(queryProject);

                foreach (Project proj in getProject)
                {
                    if (listSearchProject.Contains(proj) == false)
                    {
                        listSearchProject.Add(proj);
                    }
                }
            }
            return PartialView("GetAll", listSearchProject);
        }

        #region create new record
        public PartialViewResult Create()
        {
            IList<Employee> listEmployee = new List<Employee>();
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            var getAllEmployees = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
            foreach (Employee emp in getAllEmployees)
            {
                listEmployee.Add(emp);
            }
            var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", 1);
            ViewData["Employee"] = selectEmp;

            return PartialView();
        }

        [HttpPost]
        public PartialViewResult Create(Project proj)
        {
            if (ModelState.IsValid)
            {
                var collections = mongoDatabase.GetCollection<Project>("Project");
                collections.Insert(proj);
                var id = proj.ProjectId;
                return GetAll();
            }
            else
            {
                return PartialView(proj);
            }
        }
        #endregion

        #region Edit Record
        public PartialViewResult Edit(string Id)
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(Id, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                var proj = _project.FindOne(getQuery);
                IList<Employee> listEmployee = new List<Employee>();
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                var getAllEmployees = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
                foreach (Employee emp in getAllEmployees)
                {
                    listEmployee.Add(emp);
                }
                var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", proj.Manager);
                ViewData["Employee"] = selectEmp;

                return PartialView("Edit", proj);
            }
            else
            {
                return PartialView("Edit");
            }
        }

        [HttpPost]
        public PartialViewResult Edit(Project proj, string ProjectId)
        {
            ObjectId projectId;
            var isValid = ObjectId.TryParse(ProjectId, out projectId);
            if (isValid)
            {
                var collections = mongoDatabase.GetCollection<Project>("Project");
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                var existingproject = collections.FindOne(getQuery);
                existingproject.Name = proj.Name;
                existingproject.StartDate = proj.StartDate;
                existingproject.EndDate = proj.EndDate;
                existingproject.Manager = proj.Manager;
                existingproject.Description = proj.Description;
                existingproject.Client = proj.Client;
                //existingproject.TotalEmployees = proj.TotalEmployees;
                existingproject.IsComplete = proj.IsComplete;
                existingproject.IsActive = proj.IsActive;
                existingproject.Url = proj.Url;
                // existingproject.AssignedTo = project.AssignedTo;
                collections.Save(existingproject);
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
            var collections = mongoDatabase.GetCollection<Project>("Project");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(Id, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                Project proj = collections.FindOne(getQuery);
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
                return PartialView("Details", proj);
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
            var collections = mongoDatabase.GetCollection<Project>("Project");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(Id, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
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