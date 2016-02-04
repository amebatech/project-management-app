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
    public class DevBase
    {
        public DevBase()
        {
            PercentComplete = 0;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        [DisplayName("Task:")]
        public String TaskId { get; set; }
        [DisplayName("Task:")]
        public String TaskName { get; set; }
        [DisplayName("Assigned To:")]
        public String EmpId { get; set; }
        [DisplayName("Assigned To:")]
        public String EmpName { get; set; }
        [DisplayName("Date Completed:")]
        public String CompletedDate { get; set; }
        [DisplayName("Complete By Date:")]
        public String CompleteByDate { get; set; }
        [DisplayName("As-On-Date:")]
        public String AsOnDate { get; set; }
        [DisplayName("Status:")]
        public string Status { get; set; }
        [DisplayName("Percent Complete:")]
        public int PercentComplete { get; set; }
        [DisplayName("Remarks(Project Manager):")]
        public string Remarks { get; set; }
        [DisplayName("Remarks(Client):")]
        public string ClientRemarks { get; set; }
    }
}