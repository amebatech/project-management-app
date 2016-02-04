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
    public class ReportsController : Controller
    {
        private MongoDatabase mongoDatabase;

        public ReportsController()
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
            var _projects = mongoDatabase.GetCollection<Project>("Project");
            IList<Project> listProjects = new List<Project>();
            var getProjects = _projects.FindAs(typeof(Project), Query.NE("Name", "null"));
            foreach (Project proj in getProjects)
            {
                listProjects.Add(proj);
            }
            return PartialView("GetAll", listProjects);
        }

        public ActionResult GetPieChart()
        {
            int ntaCnt = 0;
            int inpCnt = 0;
            int comCnt = 0;
            int waCnt = 0;
            int ohCnt = 0;
            int canCnt = 0;
            int arCnt = 0;
            int cntChat = 0;

            var _projects = mongoDatabase.GetCollection<Project>("Project");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");
            var getProjects = _projects.FindAs(typeof(Project), Query.NE("Name", "null"));
            Chart[] arrChart = new Chart[getProjects.Count()];
            
            foreach (Project proj in getProjects)
            {
                ntaCnt = 0;
                inpCnt = 0;
                comCnt = 0;
                waCnt = 0;
                ohCnt = 0;
                canCnt = 0;
                string strChartTitle = "Project-wise Task Status: ["+proj.Name+"]";
                var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(proj.ProjectId.ToString(), "i")));
                foreach (Task tsk in getTasks)
                {
                    var getDBase = _devBase.FindAs(typeof(DevBase), Query.Matches("TaskId", new BsonRegularExpression(tsk.TaskId.ToString(), "i")));
                    foreach (DevBase dbase in getDBase)
                    {
                        if(dbase.Status.Equals("Not Started"))
                        {
                            ntaCnt++;
                        }
                        if(dbase.Status.Equals("In-Process"))
                        {
                            inpCnt++;
                        }
                        if(dbase.Status.Equals("Complete"))
                        {
                            comCnt++;
                        }
                        if(dbase.Status.Equals("Waiting Approval"))
                        {
                            waCnt++;
                        }
                        if (dbase.Status.Equals("On Hold"))
                        {
                            ohCnt++;
                        }
                        if (dbase.Status.Equals("Cancelled"))
                        {
                            canCnt++;
                        }
                    }
                }

                if(ntaCnt > 0)
                {
                    arCnt++;
                }
                if(inpCnt > 0)
                {
                    arCnt++;
                }
                if(comCnt > 0)
                {
                    arCnt++;
                }
                if(waCnt > 0)
                {
                    arCnt++;
                }
                if(ohCnt > 0)
                {
                    arCnt++;
                }
                if(canCnt > 0)
                {
                    arCnt++;
                }

                String [] arrTaskCount = new string[arCnt];
                String [] arrTaskName = new string[arCnt];
                arCnt = 0;

                if(ntaCnt > 0)
                {
                    arrTaskName[arCnt]="Not Assigned";
                    arrTaskCount[arCnt++] = ntaCnt.ToString();
                }
                if(inpCnt > 0)
                {
                    arrTaskName[arCnt] = "In-Process";
                    arrTaskCount[arCnt++] = inpCnt.ToString();
                }
                if(comCnt > 0)
                {
                    arrTaskName[arCnt] = "Complete";
                    arrTaskCount[arCnt++] = comCnt.ToString();
                }
                if(waCnt > 0)
                {
                    arrTaskName[arCnt] = "Waiting Approval";
                    arrTaskCount[arCnt++] = waCnt.ToString();
                }
                if(ohCnt > 0)
                {
                    arrTaskName[arCnt] = "On Hold";
                    arrTaskCount[arCnt++] = ohCnt.ToString();
                }
                if(canCnt > 0)
                {
                    arrTaskName[arCnt] = "Cancelled";
                    arrTaskCount[arCnt++] = canCnt.ToString();
                }

                arrChart[cntChat] = new Chart(width: 500, height: 350, theme: ChartTheme.Green)
                .AddSeries(
                    chartType: "pie",
                    legend: "Task Status",
                    xValue: arrTaskName,
                    yValues: arrTaskCount)
                .AddTitle(strChartTitle) 
                .Write();
            }
            return null;
        }

        public ActionResult GetRainfallChart()
        {
            int cntTask = 0;
            int cntPC = 0;
            int cntChat = 0;
            
            var _projects = mongoDatabase.GetCollection<Project>("Project");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");
            var getProjects = _projects.FindAs(typeof(Project), Query.NE("Name", "null"));
            Chart [] arrChart = new Chart[getProjects.Count()];

            foreach (Project proj in getProjects)
            {
                var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(proj.ProjectId.ToString(), "i")));
                String[] arrTask = new String[getTasks.Count()];
                String[] arrPC = new String[getTasks.Count()];
                String strChartTitle = "Project-wise Task / (% Complete) [" + proj.Name + "]";
                cntTask = 0;
                cntPC = 0;

                foreach (Task tsk in getTasks)
                {
                    arrTask[cntTask++] = tsk.Name;
                    var getQuery = Query<DevBase>.EQ(e => e.TaskId, tsk.TaskId.ToString());
                    var dBase = _devBase.FindOne(getQuery);
                    if (dBase != null)
                    {
                        arrPC[cntPC++] = dBase.PercentComplete.ToString();
                    }
                    else
                    {
                        arrPC[cntPC++] = "0";
                    }
                }

                arrChart[cntChat] = new Chart(width: 500, height: 350, theme: ChartTheme.Green)
                .AddSeries(
                    chartType: "bar",
                    chartArea: "Default",
                    axisLabel: "% Complete (Task)",
                    legend: "Tasks",
                    xValue: arrTask,
                    yValues: arrPC)
                .AddTitle(strChartTitle)
                .SetXAxis("Tasks", 0, 10)
                .SetYAxis("(% Complete)", 0, 100)
                .Write();
                
                //GetPieChartPTE(proj.ProjectId.ToString());
                //GetMonthsPTE(proj.ProjectId.ToString());
            }
            return null;
        }

        public ActionResult GetMonths()
        {
            int cntEmp = 0;
            int cntTemp = 0;

            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");
            var _projects = mongoDatabase.GetCollection<Project>("Project");
            
            var getProjects = _projects.FindAs(typeof(Project), Query.NE("Name", "null"));
            var getEmployees = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
                        
            IList<Empty> lstEmpty = new List<Empty>();

            foreach (Employee emp in getEmployees)
            {
                lstEmpty.Add(new Empty() { Name = emp.Fullname });
                foreach (Project proj in getProjects)
                {
                    var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(proj.ProjectId.ToString(), "i")));
                    foreach (Task tsk in getTasks)
                    {
                        if (tsk.AssignedTo != null)
                        {
                            foreach (Employee empty in tsk.AssignedTo)
                            {
                                if (empty.EmpId.ToString().Equals(emp.EmpId.ToString()))
                                {
                                    lstEmpty[cntEmp].NoOfTasksAssigned = ++cntTemp;
                                }
                            }
                        }
                        else
                        {
                            lstEmpty[cntEmp].NoOfTasksAssigned = 0;
                        }
                    }
                }
                cntEmp++;
            }

            var key = new Chart(width: 500, height: 350, theme: ChartTheme.Green)
                .AddSeries(chartType: "column",chartArea: "Default",axisLabel: "No Of Tasks / Employees")
                .DataBindTable(lstEmpty, xField: "Name")
                .AddTitle("Employee-wise No. Of Tasks Assigned")
                .SetXAxis("Employee",0,10)
                .SetYAxis("No Of Tasks",0,10)
                .Write(format: "gif");
            
            return null;
        }

        public ActionResult GetPieChartPT(string Id)
        {
            int ntaCnt = 0;
            int inpCnt = 0;
            int comCnt = 0;
            int waCnt = 0;
            int ohCnt = 0;
            int canCnt = 0;
            int arCnt = 0;

            var _project = mongoDatabase.GetCollection<Project>("Project");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");

            ObjectId projectId;
            var isValid = ObjectId.TryParse(Id, out projectId);
            if (isValid)
            {
                var getQuery = Query<Project>.EQ(e => e.ProjectId, projectId);
                var proj = _project.FindOne(getQuery);

                if (proj != null)
                {
                    ntaCnt = 0;
                    inpCnt = 0;
                    comCnt = 0;
                    waCnt = 0;
                    ohCnt = 0;
                    canCnt = 0;
                    var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(proj.ProjectId.ToString(), "i")));
                    foreach (Task tsk in getTasks)
                    {
                        var getDBase = _devBase.FindAs(typeof(DevBase), Query.Matches("TaskId", new BsonRegularExpression(tsk.TaskId.ToString(), "i")));
                        foreach (DevBase dbase in getDBase)
                        {
                            if (dbase.Status.Equals("Not Started"))
                            {
                                ntaCnt++;
                            }
                            if (dbase.Status.Equals("In-Process"))
                            {
                                inpCnt++;
                            }
                            if (dbase.Status.Equals("Complete"))
                            {
                                comCnt++;
                            }
                            if (dbase.Status.Equals("Waiting Approval"))
                            {
                                waCnt++;
                            }
                            if (dbase.Status.Equals("On Hold"))
                            {
                                ohCnt++;
                            }
                            if (dbase.Status.Equals("Cancelled"))
                            {
                                canCnt++;
                            }
                        }
                    }

                    if (ntaCnt > 0)
                    {
                        arCnt++;
                    }
                    if (inpCnt > 0)
                    {
                        arCnt++;
                    }
                    if (comCnt > 0)
                    {
                        arCnt++;
                    }
                    if (waCnt > 0)
                    {
                        arCnt++;
                    }
                    if (ohCnt > 0)
                    {
                        arCnt++;
                    }
                    if (canCnt > 0)
                    {
                        arCnt++;
                    }

                    String[] arrTaskCount = new string[arCnt];
                    String[] arrTaskName = new string[arCnt];

                    arCnt = 0;

                    if (ntaCnt > 0)
                    {
                        arrTaskName[arCnt] = "Not Assigned";
                        arrTaskCount[arCnt++] = ntaCnt.ToString();
                    }
                    if (inpCnt > 0)
                    {
                        arrTaskName[arCnt] = "In-Process";
                        arrTaskCount[arCnt++] = inpCnt.ToString();
                    }
                    if (comCnt > 0)
                    {
                        arrTaskName[arCnt] = "Complete";
                        arrTaskCount[arCnt++] = comCnt.ToString();
                    }
                    if (waCnt > 0)
                    {
                        arrTaskName[arCnt] = "Waiting Approval";
                        arrTaskCount[arCnt++] = waCnt.ToString();
                    }
                    if (ohCnt > 0)
                    {
                        arrTaskName[arCnt] = "On Hold";
                        arrTaskCount[arCnt++] = ohCnt.ToString();
                    }
                    if (canCnt > 0)
                    {
                        arrTaskName[arCnt] = "Cancelled";
                        arrTaskCount[arCnt++] = canCnt.ToString();
                    }

                    var key = new Chart(width: 400, height: 400, theme: ChartTheme.Green)
                    .AddSeries(
                        chartType: "pie",
                        legend: "Rainfall",
                        xValue: arrTaskName,
                        yValues: arrTaskCount)
                    .AddTitle("Task Status")
                    .Write();
                }
            }
            return null;
        }

        public ActionResult GetRainfallChartPT(string Id)
        {
            int cntTask = 0;
            int cntPC = 0;

            var _projects = mongoDatabase.GetCollection<Project>("Project");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");

            if (Id != null)
            {
                var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(Id, "i")));
                String[] arrTask = new String[getTasks.Count()];
                String[] arrPC = new String[getTasks.Count()];

                foreach (Task tsk in getTasks)
                {
                    arrTask[cntTask++] = tsk.Name;
                    var getQuery = Query<DevBase>.EQ(e => e.TaskId, tsk.TaskId.ToString());
                    var dBase = _devBase.FindOne(getQuery);
                    if (dBase != null)
                    {
                        arrPC[cntPC++] = dBase.PercentComplete.ToString();
                    }
                    else
                    {
                        arrPC[cntPC++] = "0";
                    }
                }
                var key = new Chart(width: 400, height: 400, theme: ChartTheme.Green)
                .AddSeries(
                    chartType: "bar",
                    legend: "Rainfall",
                    xValue: arrTask,
                    yValues: arrPC)
                .AddTitle("Tasks(% Complete)")
                .SetXAxis("Tasks", 0, 10)
                .SetYAxis("% Complete", 0, 100)
                .Write();
            }
            return null;
        }

        public ActionResult GetMonthsPT(string Id)
        {
            int cntEmp = 0;
            int cntTemp = 0;

            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            var _task = mongoDatabase.GetCollection<Task>("Task");
            var _devBase = mongoDatabase.GetCollection<DevBase>("DevBase");
            var _projects = mongoDatabase.GetCollection<Project>("Project");

            //var getProjects = _projects.FindAs(typeof(Project), Query.NE("Name", "null"));
            var getEmployees = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));

            IList<Empty> lstEmpty = new List<Empty>();

            foreach (Employee emp in getEmployees)
            {
                lstEmpty.Add(new Empty() { Name = emp.Fullname });
                var getTasks = _task.FindAs(typeof(Task), Query.Matches("ProjectId", new BsonRegularExpression(Id, "i")));
                foreach (Task tsk in getTasks)
                {
                    if (tsk.AssignedTo != null)
                    {
                        foreach (Employee empty in tsk.AssignedTo)
                        {
                            if (empty.EmpId.ToString().Equals(emp.EmpId.ToString()))
                            {
                                lstEmpty[cntEmp].NoOfTasksAssigned = ++cntTemp;
                            }
                        }
                    }
                    else
                    {
                        lstEmpty[cntEmp].NoOfTasksAssigned = 0;
                    }
                }
                cntEmp++;
            }

            var key = new Chart(width: 400, height: 400, theme: ChartTheme.Green)
                .AddSeries(chartType: "column", chartArea: "Default", axisLabel: "No Of Tasks / Employees")
                .DataBindTable(lstEmpty, xField: "Name")
                .AddTitle("No. Of Tasks Assigned per Employee")
                .SetXAxis("Employee", 0, 10)
                .SetYAxis("No Of Tasks", 0, 10)
                .Write(format: "gif");

            return null;
        }

        //public ActionResult GetMonths()
        //{
        //    var d = new DateTimeFormatInfo();
        //    var key = new Chart(width: 600, height: 400)
        //        .AddSeries(chartType: "column")
        //        .DataBindTable(d.MonthNames)
        //        .Write(format: "gif");
        //    return null;
        //}
    }
}