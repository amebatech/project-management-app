using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Revoluza.Models
{
    public class UserAdmin
    {
        private ObjAddress _address;
        private Contact _contact;
        private Details _details;
        private Preferences _prefer;

        public BsonBinaryData _id { get; set; }
        [BsonIgnore]
        public BsonType _t { get; set; }
        [DisplayName("User Name:")]
        public string uname { get; set; }
        public string lname { get; set; }
        [DisplayName("Email Id:")]
        public string email { get; set; }
        public string lemail { get; set; }
        public string dname { get; set; }
        public string cmnt { get; set; }
        public string pass { get; set; }
        public Int32 fmt { get; set; }
        public string salt { get; set; }
        public string qstion { get; set; }
        public string ans { get; set; }
        [DisplayName("Is Approved:")]
        public Boolean apprvd { get; set; }
        [DisplayName("Activated On:")]
        public DateTime actdate { get; set; }
        public DateTime logindate { get; set; }
        public DateTime passdate { get; set; }
        [DisplayName("Created On:")]
        public DateTime create { get; set; }
        [DisplayName("Is Locked:")]
        public Boolean lockd { get; set; }
        public DateTime lockdate { get; set; }
        public Int32 passcount { get; set; }
        public DateTime passwindow { get; set; }
        public Int32 anscount { get; set; }
        public DateTime answindow { get; set; }
        public string [] roles { get; set; }
        public DateTime moddate { get; set; }
        public Contact Contacts 
        { 
            get { return _contact; } 
            set { _contact = value; } 
        }
        public ObjAddress Address 
        { 
            get { return _address; } 
            set { _address = value; } 
        }
        public Details Personal 
        { 
            get { return _details; } 
            set { _details = value; } 
        }
        public Preferences Preferences
        {
            get { return _prefer; }
            set { _prefer = value; }
        }
        [DisplayName("Full Address:")]
        public string FullAddress
        {
            get
            {
                if (_address != null)
                {
                    return string.Format("{0} {1} {2} {3} {4} {5}", _address.AptNumber, _address.Address, _address.City, _address.State, _address.Country, _address.PostalCode);
                }
                else
                {
                    return "";
                }
            }
        }
        [DisplayName("User Name:")]
        public string FullName
        {
            get
            {
                if(_details != null)
                {
                    return string.Format("{0} {1}", _details.FirstName, _details.LastName);
                }
                else
                {
                    return "";
                }
            }
        }

    }

    public class Contact
    {
        [DisplayName("Phone No.:")]
	    public string DayTimePhone { get; set; }
	    public string FaxHome { get; set; }
	    public string DayTimePhoneExt { get; set; }
	    public string EveningPhoneExt { get; set; }
	    public string EveningPhone { get; set; }
	    public string FaxBusiness { get; set; }
        [DisplayName("Mobile No.:")]
	    public string CellPhone { get; set; }
    } 

    public class ObjAddress
    { 
	    public string Country { get; set; }
	    public string City { get; set; }
	    public string State { get; set; }
	    public string Address { get; set; }
	    public string PostalCode { get; set; }
	    public string AptNumber { get; set; }
    }

    public class Details
    { 
	    public string Occupation { get; set; }
	    public string Website { get; set; }
	    public string Gender { get; set; }
        [DisplayName("Date Of Birth:")]
	    public DateTime BirthDate { get; set; }
	    public string LastName { get; set; }
        public string FirstName { get; set; }
    } 

    public class Preferences
    { 
	    public string Culture { get; set; }
	    public string Theme { get; set; }
        public string Newsletter { get; set; }
    } 	
}