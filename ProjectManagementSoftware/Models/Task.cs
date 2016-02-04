using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;

namespace Revoluza.Models
{
    public class Task
    {
        private IList<Employee> _employee = new List<Employee>();
        
        [BsonId]
        public ObjectId TaskId { get; set; }
        [DisplayName("For Project:")]
        public string ProjectId { get; set; }
        [DisplayName("For Project:")]
        public string ProjectName { get; set; }
        [DisplayName("Task Name:")]
        public string Name { get; set; }
        [DisplayName("Task Description:")]
        public string Description { get; set; }
        [DisplayName("Start Date:")]
        public String StartDate { get; set; }
        [DisplayName("End Date:")]
        public String EndDate { get; set; }
        [DisplayName("Project Manager:")]
        public string ManagerId { get; set; }
        [DisplayName("Project Manager:")]
        public string ManagerName { get; set; }
        [DisplayName("Parent Task:")]
        public string ParentId { get; set; }
        [DisplayName("Parent Task:")]
        public string ParentName { get; set; }
        [DisplayName("Task Completed:")]
        public Boolean IsComplete { get; set; }
        [DisplayName("Is Active:")]
        public Boolean IsActive { get; set; }
        [DisplayName("Employees Assigned:")]
        public IList<Employee> AssignedTo
        {
            get { return _employee; }
            set { _employee = value; }
        }
    }
}
