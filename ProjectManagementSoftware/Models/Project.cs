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
    public class Project
    {
        [BsonId]
        public ObjectId ProjectId { get; set; }
        [DisplayName("Name:")]
        public string Name { get; set; }
        [DisplayName("Description:")]
        public string Description { get; set; }
        [DisplayName("Start Date:")]
        public String StartDate { get; set; }
        [DisplayName("End Date:")]
        public String EndDate { get; set; }
        [DisplayName("Url:")]
        public string Url { get; set; }
        [DisplayName("Client:")]
        public string Client { get; set; }
        [DisplayName("Project Manager:")]
        public string Manager { get; set; }
        [DisplayName("Project Manager:")]
        public string ProjectManager { get; set; }
        [DisplayName("Project Completed:")]
        public Boolean IsComplete { get; set; }
        [DisplayName("Is Active:")]
        public Boolean IsActive { get; set; }
        [DisplayName("Total Employees:")]
        public int TotalEmployees { get; set; }
        //public IList<Employee> AssignedTo { get; set; }
    }
}
