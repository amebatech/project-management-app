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
    public class TicketBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [DisplayName("Project Name:")]
        public string ProjectId { get; set; }
        [DisplayName("Project Name:")]
        public string ProjectName { get; set; }
        [DisplayName("Date Created:")]
        public String CreateDate { get; set; }
        [DisplayName("Created By:")]
        public String CreatedBy { get; set; }
        [DisplayName("Bug / Issue:")]
        public string Issue { get; set; }
        [DisplayName("Priority:")]
        public string Priority { get; set; }
        [DisplayName("Assigned To:")]
        public string AssignedToId { get; set; }
        [DisplayName("Assigned To:")]
        public string AssignedTo { get; set; }
        [DisplayName("Status:")]
        public string Status { get; set; }
        [DisplayName("As-On-Date:")]
        public String AsOnDate { get; set; }
        [DisplayName("Percent Fixed:")]
        public int PercentComplete { get; set; }
        [DisplayName("Date Closed:")]
        public String CloseDate { get; set; }
        [DisplayName("Remarks:")]
        public string Remarks { get; set; }
    }
}