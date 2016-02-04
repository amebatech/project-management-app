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
using System.Drawing;
using System.IO;
using System.Web.UI.DataVisualization.Charting;

namespace Revoluza.Controllers
{
    public class ChartsController : Controller
    {
        private MongoDatabase mongoDatabase;

        public ChartsController()
        {
            // create connection string
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
            IList<Project> listProject = new List<Project>();
            var collections = mongoDatabase.GetCollection<Project>("Project");
            var getAllProjects = collections.FindAs(typeof(Project), Query.NE("Name", "null"));
            foreach (Project proj in getAllProjects)
            {
                listProject.Add(proj);
            }
            var selectList = new SelectList(listProject, "ProjectId", "Name", 1);
            ViewData["Project"] = selectList;

            return View();
        }

        public PartialViewResult GetAll()
        {
            var collections = mongoDatabase.GetCollection<Users>("Users");
            IList<Users> listUsers = new List<Users>();
            var getUsers = collections.FindAs(typeof(Users), Query.NE("Name", "null"));
            foreach (Users user in getUsers)
            {
                listUsers.Add(user);
            }
            return PartialView("GetAll", listUsers);
        }

        [HttpGet]
        public JsonResult TaskSummary()
        {
            int cntTask = 0;
            int cntProj = 0;

            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");
            var _projects = mongoDatabase.GetCollection<Project>("Project");

            var getProjects = _projects.FindAs(typeof(Project), Query.NE("Name", "null"));
            var getEmployees = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));

            IList<Empty> lstEmpty = new List<Empty>();

            foreach (Employee emp in getEmployees)
            {
                foreach (Project proj in getProjects)
                {
                    lstEmpty.Add(new Empty() { Name = emp.Fullname });
                    lstEmpty[cntProj].AssignedTo = proj.Name;
                    var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(proj.ProjectId.ToString(), "i")));
                    foreach (Task tsk in getTasks)
                    {
                        if (tsk.AssignedTo != null)
                        {
                            foreach (Employee empty in tsk.AssignedTo)
                            {
                                if (empty.EmpId.ToString().Equals(emp.EmpId.ToString()))
                                {
                                    lstEmpty[cntProj].NoOfTasksAssigned = ++cntTask;
                                }
                            }
                        }
                        else
                        {
                            lstEmpty[cntProj].NoOfTasksAssigned = 0;
                        }
                    }
                    cntProj++;
                    cntTask = 0;
                }
            }
            return Json(lstEmpty.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CalcWorkDays(string pId)
        {
            int cntTask = 0;
            //int cntPayTask = 0;
            //int cntSubTask = 0;

            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");
            var _projects = mongoDatabase.GetCollection<Project>("Project");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(pId, out projectId);
            Project proj = new Project();
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                proj = _projects.FindOne(getQuery);
            }

            //var getProjects = _projects.FindAs(typeof(Project), Query.NE("Name", "null"));
            //var getEmployees = _employee.FindAs(typeof(Employee), Query.NE("FirstName", "null"));

            IList<Hours> lstEmpHours = new List<Hours>();

            var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(proj.ProjectId.ToString(), "i")));
            foreach (Task tsk in getTasks)
            {
                lstEmpHours.Add(new Hours() { Task = tsk.Name });
                foreach (Employee emp in tsk.AssignedTo)
                {
                    lstEmpHours[cntTask].Employee = emp.Fullname;
                    if (tsk.EndDate != "" && tsk.EndDate != null && Convert.ToDateTime(tsk.EndDate) < DateTime.Now)
                    {
                        lstEmpHours[cntTask].Days = Convert.ToDateTime(tsk.EndDate).Subtract(Convert.ToDateTime(tsk.StartDate)).Days;
                    }
                    else
                    {
                        lstEmpHours[cntTask].Days = DateTime.Now.Subtract(Convert.ToDateTime(tsk.StartDate)).Days;
                    }
                }
                cntTask++;
            }

            return Json(lstEmpHours.ToList(), JsonRequestBehavior.AllowGet);
        }

        public class seriesData
        {
            public string name;
            public long y;
        }

        [HttpGet]
        public JsonResult GetPieCharts(string pId)
        {
            int cntTask = 0;

            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");
            var _projects = mongoDatabase.GetCollection<Project>("Project");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(pId, out projectId);
            Project proj = new Project();
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                proj = _projects.FindOne(getQuery);
            }

            IList<seriesData> lstseriesData = new List<seriesData>();
            var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(pId, "i")));

            foreach (Task tsk in getTasks)
            {
                int ntaCnt = 0;

                var getDBase = _devBase.FindAs(typeof(DevBase), Query.Matches("TaskId", new BsonRegularExpression(tsk.TaskId.ToString(), "i")));

                foreach (DevBase dbase in getDBase)
                {
                    if (dbase.Status.Equals("Not Started"))
                    {
                        foreach (seriesData mtv in lstseriesData)
                        {
                            if (mtv.name == dbase.Status)
                            {
                                int cntl = lstseriesData.IndexOf(mtv);
                                lstseriesData[cntl].y = ++(lstseriesData[cntl].y);
                                ntaCnt++;
                            }
                        }
                        
                    }
                    if (dbase.Status.Equals("In-Process"))
                    {
                        foreach (seriesData mtv in lstseriesData)
                        {
                            if (mtv.name == dbase.Status)
                            {
                                int cntl = lstseriesData.IndexOf(mtv);
                                lstseriesData[cntl].y = ++(lstseriesData[cntl].y);
                                ntaCnt++;
                            }
                        }
                    }
                    if (dbase.Status.Equals("Complete"))
                    {
                        foreach (seriesData mtv in lstseriesData)
                        {
                            if (mtv.name == dbase.Status)
                            {
                                int cntl = lstseriesData.IndexOf(mtv);
                                lstseriesData[cntl].y = ++(lstseriesData[cntl].y);
                                ntaCnt++;
                            }
                        }
                    }
                    if (dbase.Status.Equals("Waiting Approval"))
                    {
                        foreach (seriesData mtv in lstseriesData)
                        {
                            if (mtv.name == dbase.Status)
                            {
                                int cntl = lstseriesData.IndexOf(mtv);
                                lstseriesData[cntl].y = ++(lstseriesData[cntl].y);
                                ntaCnt++;
                            }
                        }
                    }
                    if (dbase.Status.Equals("On Hold"))
                    {
                        foreach (seriesData mtv in lstseriesData)
                        {
                            if (mtv.name == dbase.Status)
                            {
                                int cntl = lstseriesData.IndexOf(mtv);
                                lstseriesData[cntl].y = ++(lstseriesData[cntl].y);
                                ntaCnt++;
                            }
                        }
                    }
                    if (dbase.Status.Equals("Cancelled"))
                    {
                        foreach (seriesData mtv in lstseriesData)
                        {
                            if (mtv.name == dbase.Status)
                            {
                                int cntl = lstseriesData.IndexOf(mtv);
                                lstseriesData[cntl].y = ++(lstseriesData[cntl].y);
                                ntaCnt++;
                            }
                        }
                    }
                    if (ntaCnt <= 0)
                    {
                        seriesData mtv = new seriesData();
                        lstseriesData.Add(mtv);
                        lstseriesData[cntTask].name = dbase.Status;
                        lstseriesData[cntTask++].y = ++ntaCnt;
                    }
                }
            }

            return Json(lstseriesData.ToList(), JsonRequestBehavior.AllowGet);
        }  
    }
}