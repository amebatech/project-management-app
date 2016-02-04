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
using System.Web.Helpers;

namespace Revoluza.Controllers
{
    public class TicketsController : Controller
    {
        private MongoDatabase mongoDatabase;
        public TicketsController()
        {
            // create connectionstring
            var connect = "mongodb://localhost";
            var Client = new MongoClient(connect);

            // get Reference of server
            var Server = Client.GetServer();

            // get Reference of Database
            mongoDatabase = Server.GetDatabase("School");
        }
        //
        // GET: /Tickets/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetAll()
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");
            var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");

            IList<TicketBase> lstTickets = new List<TicketBase>();
            var getTicks = _tickets.FindAs(typeof(TicketBase), Query.NE("Status", "Cancelled"));

            foreach (TicketBase tick in getTicks)
            {
                string empId = tick.AssignedToId;
                ObjectId assId;
                var isValid = ObjectId.TryParse(empId, out assId);
                if (isValid)
                {
                    var getQuery = Query<Employee>.EQ(e => e.EmpId, assId);
                    var rtemp = _employee.FindOne(getQuery);
                    if (rtemp != null)
                    {
                        tick.AssignedTo = rtemp.Fullname;
                    }
                }

                string projId = tick.ProjectId;
                ObjectId projectId;
                isValid = ObjectId.TryParse(projId, out projectId);
                if (isValid)
                {
                    var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                    var proj = _project.FindOne(getQuery);
                    if (proj != null)
                    {
                        tick.ProjectName = proj.Name;
                    }
                }
                lstTickets.Add(tick);
            }
            return PartialView("GetAll", lstTickets);
        }

        #region create new record
        public PartialViewResult Create()
        {
            var _project = mongoDatabase.GetCollection<Project>("Project");
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");

            IList<Project> listProject = new List<Project>();
            var getAllProjects = _project.FindAs(typeof(Project), Query.NE("Name", "null"));
            foreach (Project proj in getAllProjects)
            {
                listProject.Add(proj);
            }
            var selectProj = new SelectList(listProject, "ProjectId", "Name", listProject[0].ProjectId);
            ViewData["Project"] = selectProj;

            IList<Employee> listEmployee = new List<Employee>();
            var getAllEmployees = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
            foreach (Employee emp in getAllEmployees)
            {
                listEmployee.Add(emp);
            }
            var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", 1);
            ViewData["Employee"] = selectEmp;

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

            var selectSt = new SelectList(listStatus, "Name", "Name", listStatus[0].Name);
            ViewData["Status"] = selectSt;

            List<Status> listPriority = new List<Status>();

            Status stP3 = new Status() { Name = "Very Urgent" };
            listPriority.Add(stP3);
            Status stP1 = new Status() { Name = "Urgent" };
            listPriority.Add(stP1);
            Status stP2 = new Status() { Name = "Normal" };
            listPriority.Add(stP2);

            var selectPriority = new SelectList(listPriority, "Name", "Name", listPriority[0].Name);
            ViewData["Priority"] = selectPriority;

            return PartialView();
        }

        [HttpPost]
        public PartialViewResult SaveTicket(TicketBase tick)
        {
            if (ModelState.IsValid)
            {
                var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");
                _tickets.Insert(tick);
                var id = tick.Id;
                return GetAll();
            }
            else
            {
                return PartialView("Error");
            }
        }

        [HttpPost]
        public PartialViewResult Create(TicketBase tick)
        {
            if (ModelState.IsValid)
            {
                var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");
                _tickets.Insert(tick);
                var id = tick.Id;
                return GetAll();
            }
            else
            {
                return PartialView("Error");
            }
        }
        #endregion

        #region Edit Record
        public PartialViewResult Edit(string Id)
        {
            var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");
            
            ObjectId tickId;
            var isValid = ObjectId.TryParse(Id, out tickId);

            if (isValid)
            {
                var getQuery = Query<TicketBase>.EQ(e => e.Id, tickId);
                var tick = _tickets.FindOne(getQuery);

                var _project = mongoDatabase.GetCollection<Project>("Project");
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");

                ObjectId projId;
                isValid = ObjectId.TryParse(tick.ProjectId, out projId);
                IList<Project> listProject = new List<Project>();
                if (isValid)
                {
                    getQuery = Query<Project>.EQ(e => e.ProjectId, projId);
                    Project proj = _project.FindOne(getQuery);
                    listProject.Add(proj);
                    var selectProj = new SelectList(listProject, "ProjectId", "Name", proj.ProjectId);
                    ViewData["Project"] = selectProj;
                }

                ObjectId empId;
                isValid = ObjectId.TryParse(tick.AssignedToId, out empId);
                IList<Employee> listEmployee = new List<Employee>();
                if (isValid)
                {
                    getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
                    Employee emp = _employee.FindOne(getQuery);
                    listEmployee.Add(emp);
                    var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", tick.AssignedToId);
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

                var selectSt = new SelectList(listStatus, "Name", "Name", tick.Status);
                ViewData["Status"] = selectSt;

                List<Status> listPriority = new List<Status>();
                Status stP3 = new Status() { Name = "Very Urgent" };
                listPriority.Add(stP3);
                Status stP1 = new Status() { Name = "Urgent" };
                listPriority.Add(stP1);
                Status stP2 = new Status() { Name = "Normal" };
                listPriority.Add(stP2);

                var selectPriority = new SelectList(listPriority, "Name", "Name", tick.Priority);
                ViewData["Priority"] = selectPriority;

                return PartialView("Edit",tick);
            }
            else
            {
                return PartialView("Error");
            }
        }

        [HttpPost]
        public PartialViewResult Edit(TicketBase tick,string Id)
        {
            ObjectId tickId;
            var isValid = ObjectId.TryParse(Id, out tickId);
            if (isValid)
            {
                var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");
                var getQuery = Query<TicketBase>.EQ(e => e.Id, tickId);
                var existingtick = _tickets.FindOne(getQuery);
                existingtick.AssignedToId = tick.AssignedToId;
                existingtick.CloseDate = tick.CloseDate;
                existingtick.CreateDate = tick.CreateDate;
                existingtick.CreatedBy = tick.CreatedBy;
                existingtick.Issue = tick.Issue;
                existingtick.PercentComplete = tick.PercentComplete;
                existingtick.ProjectName = tick.ProjectName;
                existingtick.Remarks = tick.Remarks;
                existingtick.Status = tick.Status;

                _tickets.Save(existingtick);
                return GetAll();
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
            var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");

            ObjectId userId;
            var isValid = ObjectId.TryParse(Id, out userId);
            if (isValid)
            {
                var getQuery = Query<TicketBase>.EQ(e => e.Id, userId);
                var tick = _tickets.FindOne(getQuery);
                var _project = mongoDatabase.GetCollection<Project>("Project");
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");

                ObjectId projId;
                isValid = ObjectId.TryParse(tick.ProjectId, out projId);
                if (isValid)
                {
                    getQuery = Query<Project>.EQ(e => e.ProjectId, projId);
                    Project proj = _project.FindOne(getQuery);
                    tick.ProjectName = proj.Name; 
                }

                ObjectId empId;
                isValid = ObjectId.TryParse(tick.AssignedToId, out empId);
                if (isValid)
                {
                    getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
                    Employee emp = _employee.FindOne(getQuery);
                    tick.AssignedTo = emp.Fullname; 
                }
                return PartialView("Details", tick);
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
            var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");

            ObjectId tickId;
            var isValid = ObjectId.TryParse(Id, out tickId);
            if (isValid)
            {
                var getQuery = Query<TicketBase>.EQ(e => e.Id, tickId);
                _tickets.Remove(getQuery);
                return GetAll();
            }
            else
            {
                return PartialView("Error");
            }
        }
        #endregion

        public PartialViewResult Search(string searchVal)
        {
            var _tickets = mongoDatabase.GetCollection<TicketBase>("TicketBase");
            IList<TicketBase> listTickets = new List<TicketBase>();
            if (searchVal == "")
            {
                var getTicket = _tickets.FindAs(typeof(Task), Query.NE("Status", "Cancelled"));
                foreach (TicketBase tick in getTicket)
                {
                    listTickets.Add(tick);
                }
            }
            else
            {
                var queryTick = Query.Or(
                    Query.Matches("CreatedBy", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Issue", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Status", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("DateCreated", new BsonRegularExpression(searchVal, "i")));

                var getTicket = _tickets.Find(queryTick);

                foreach (TicketBase tick in getTicket)
                {
                    if (listTickets.Contains(tick) == false)
                    {
                        listTickets.Add(tick);
                    }
                }
            }
            return PartialView("GetAll", listTickets);
        }

        
    }
}
