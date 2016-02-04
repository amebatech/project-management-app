using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Revoluza.Models
{
    public class Users
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [DisplayName("User Name:")]
        public string UserName { get; set; }
        [DisplayName("Address:")]
        public string Address { get; set; }
        [DisplayName("EMail:")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Phone No:")]
        public string PhoneNo { get; set; }
        [DisplayName("Password:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}