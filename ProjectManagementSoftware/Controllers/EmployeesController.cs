using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using Revoluza.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Revoluza.Controllers
{
    public class EmployeesController : Controller
    {
        private MongoDatabase mongoDatabase;
        public EmployeesController()
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
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Employee> listEmployee = new List<Employee>();
            var getEmployee = _employee.FindAs(typeof(Employee), Query.NE("IsActive", false));
            foreach (Employee emp in getEmployee)
            {
                string empId = emp.ReportingTo;
                ObjectId pmId;
                var isValid = ObjectId.TryParse(empId, out pmId);
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
            return PartialView("GetAll", listEmployee);
        }

        public PartialViewResult Search(string searchVal)
        {
            var collections = mongoDatabase.GetCollection<Employee>("Employee");
            IList<Employee> listEmployee = new List<Employee>();

            if (searchVal == "")
            {
                var getEmployee = collections.FindAs(typeof(Employee), Query.NE("IsActive", false));
                foreach (Employee emp in getEmployee)
                {
                    listEmployee.Add(emp);
                }
            }
            else
            {
                var queryEmployee = Query.Or(
                    Query.Matches("FirstName", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("LastName", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Department", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Designation", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("DateOfBirth", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Email", new BsonRegularExpression(searchVal, "i")));

                var getEmployee = collections.Find(queryEmployee);

                foreach (Employee emp in getEmployee)
                {
                    if (!listEmployee.Contains(emp))
                    {
                        listEmployee.Add(emp);
                    }
                }
            }
            return PartialView("GetAll", listEmployee);
        }

        #region create new record
        public PartialViewResult Create()
        {
            IList<Employee> listEmployee = new List<Employee>();
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");
            var getReportHead = _employee.FindAs(typeof(Employee), Query.EQ("IsReportingHead", true));
            foreach (Employee emp in getReportHead)
            {
                listEmployee.Add(emp);
            }
            if (listEmployee.Count > 0)
            {
                var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", 0);
                ViewData["Employee"] = selectEmp;
            }

            return PartialView();
        }

        [HttpPost]
        public PartialViewResult Create(Employee emp)
        {
            if (ModelState.IsValid)
            {
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                string empId = emp.ReportingTo;
                ObjectId pmId;
                var isValid = ObjectId.TryParse(empId, out pmId);
                if (isValid)
                {
                    var getQuery = Query<Employee>.EQ(e => e.EmpId, pmId);
                    var rtemp = _employee.FindOne(getQuery);
                    if (rtemp != null)
                    {
                        emp.ReportingHead = rtemp.Fullname;
                    }
                }
                _employee.Insert(emp);
                var id = emp.EmpId;
                return GetAll();
            }
            else
            {
                return PartialView(emp);
            }
        }
        #endregion

        #region Edit Record
        public PartialViewResult Edit(string Id)
        {
            var collections = mongoDatabase.GetCollection<Employee>("Employee");

            ObjectId empId;
            var isValid = ObjectId.TryParse(Id, out empId);
            if (isValid)
            {
                var getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
                var emp = collections.FindOne(getQuery);
                var _employee = mongoDatabase.GetCollection<Employee>("Employee");
                IList<Employee> listEmployee = new List<Employee>();
                var getReportingHead = _employee.FindAs(typeof(Employee), Query.EQ("IsReportingHead", true));
                foreach (Employee rthead in getReportingHead)
                {
                    listEmployee.Add(rthead);
                }
                if (listEmployee.Count > 0)
                {
                    var selectEmp = new SelectList(listEmployee, "EmpId", "FullName", emp.ReportingTo);
                    ViewData["Employee"] = selectEmp;
                }
                return PartialView("Edit", emp);
            }
            else
            {
                return PartialView("Edit");
            }

        }

        [HttpPost]
        public PartialViewResult Edit(Employee emp, string EmpId)
        {
            ObjectId empId;
            var isValid = ObjectId.TryParse(EmpId, out empId);
            if (isValid)
            {
                var collections = mongoDatabase.GetCollection<Employee>("Employee");
                var getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
                var existingemp = collections.FindOne(getQuery);
                existingemp.LastName = emp.LastName;
                existingemp.FirstName = emp.FirstName;
                existingemp.Email = emp.Email;
                existingemp.Designation = emp.Designation;
                existingemp.ReportingTo = emp.ReportingTo;
                existingemp.Department = emp.Department;
                existingemp.DateOfBirth = emp.DateOfBirth;
                existingemp.Gender = emp.Gender;
                existingemp.Salary = emp.Salary;
                existingemp.IsReportingHead = emp.IsReportingHead;
                existingemp.IsActive = emp.IsActive;
                collections.Save(existingemp);
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
            var _employee = mongoDatabase.GetCollection<Employee>("Employee");

            ObjectId empId;
            var isValid = ObjectId.TryParse(Id, out empId);
            if (isValid)
            {
                var getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
                Employee emp = _employee.FindOne(getQuery);
                if (emp.ReportingTo != null && emp.ReportingHead == null)
                {
                    string revoluzaId = emp.ReportingTo;
                    ObjectId pleasurebuilderId;
                    isValid = ObjectId.TryParse(revoluzaId, out pleasurebuilderId);
                    if (isValid)
                    {
                        getQuery = Query<Employee>.EQ(e => e.EmpId, pleasurebuilderId);
                        var rtemp = _employee.FindOne(getQuery);
                        if (rtemp != null)
                        {
                            emp.ReportingHead = rtemp.Fullname;
                        }
                    }
                }
                return PartialView("Details", emp);
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
            var collections = mongoDatabase.GetCollection<Employee>("Employee");

            ObjectId empId;
            var isValid = ObjectId.TryParse(Id, out empId);
            if (isValid)
            {
                var getQuery = Query<Employee>.EQ(e => e.EmpId, empId);
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