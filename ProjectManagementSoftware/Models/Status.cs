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
    public class Status
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [DisplayName("Status:")]
        public String Name { get; set; }
    }
}