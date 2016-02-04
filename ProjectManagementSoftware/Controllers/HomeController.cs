using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Revoluza.Models;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Revoluza.Controllers
{
    public class HomeController : Controller
    {
        private MongoDatabase mongoDatabase;
        public HomeController()
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

        public ActionResult About()
        {
            return View();
        }

        public ActionResult AfterLogin()
        {
            ViewBag.Message = "Thanks for using Ameba Project Tracker!";

            return View();
        }

        //public PartialViewResult GetAll()
        //{
        //    var collections = mongoDatabase.GetCollection<Users>("Users");
        //    IList<Users> listUsers = new List<Users>();
        //    var getUsers = collections.FindAs(typeof(Users), Query.NE("Name", "null"));
        //    foreach (Users user in getUsers)
        //    {
        //        listUsers.Add(user);
        //    }
        //    return PartialView("GetAll", listUsers);
        //}

        public PartialViewResult GetAll()
        {
            var collections = mongoDatabase.GetCollection<UserAdmin>("/users");
            IList<UserAdmin> listUsers = new List<UserAdmin>();
            var getUsers = collections.FindAs(typeof(UserAdmin), Query.NE("locked", "true"));
            foreach (UserAdmin user in getUsers)
            {
                listUsers.Add(user);
            }
            return PartialView("GetAll", listUsers);
        }

        public PartialViewResult Search(string searchVal)
        {
            var collections = mongoDatabase.GetCollection<Users>("Users");
            IList<Users> listSearchUser = new List<Users>();

            if (searchVal == "")
            {
                var getUser = collections.FindAs(typeof(Users), Query.NE("UserName", "null"));
                foreach (Users usr in getUser)
                {
                    listSearchUser.Add(usr);
                }
            }
            else
            {
                var queryUser = Query.Or(
                    Query.Matches("UserName", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Address", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("Email", new BsonRegularExpression(searchVal, "i")),
                    Query.Matches("PhoneNo", new BsonRegularExpression(searchVal, "i")));

                var getUser = collections.Find(queryUser);

                foreach (Users user in getUser)
                {
                    if (listSearchUser.Contains(user) == false)
                    {
                        listSearchUser.Add(user);
                    }
                }
            }
            return PartialView("GetAll", listSearchUser);
        }


        #region create new record
        public PartialViewResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult Create(Users user)
        {
            if (ModelState.IsValid)
            {
                var collections = mongoDatabase.GetCollection<Users>("Users");
                collections.Insert(user);
                var id = user.Id;
                return PartialView(user);
            }
            else
            {
                return PartialView(user);
            }
        }
        #endregion

        #region Edit Record
        public PartialViewResult Edit(string Id)
        {
            var collections = mongoDatabase.GetCollection<Users>("Users");
            
            ObjectId userId;
            var isValid = ObjectId.TryParse(Id,out userId);
            if (isValid)
            {
                var getQuery = Query<Users>.EQ(e => e.Id, userId);
                var user = collections.FindOne(getQuery);
                return PartialView("Edit",user);
            }
            else
            {
                return PartialView("Edit");
            }

        }

        [HttpPost]
        public PartialViewResult Edit(Users user,string Id)
        {
            ObjectId userId;
            var isValid = ObjectId.TryParse(Id, out userId);
            if (isValid)
            {
                var collections = mongoDatabase.GetCollection<Users>("Users");
                var getQuery = Query<Users>.EQ(e => e.Id, userId);
                var existinguser = collections.FindOne(getQuery);
                existinguser.UserName = user.UserName;
                existinguser.Address = user.Address;
                existinguser.Email = user.Email;
                existinguser.Password = user.Password;
                existinguser.PhoneNo = user.PhoneNo;
                collections.Save(existinguser);
                return PartialView("Edit");
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
            var _user = mongoDatabase.GetCollection<UserAdmin>("/users");

            //ObjectId userId;
            //var isValid = ObjectId.TryParse(Id, out userId);
            if (Id != null)
            {
                var getQuery = Query<UserAdmin>.EQ(e => e.lname, Id);
                var user = _user.FindOne(getQuery);
                return PartialView("Details", user);
            }
            else
            {

                return PartialView("Error");
            }
        }

        //public PartialViewResult Details(string Id)
        //{
        //    var collections = mongoDatabase.GetCollection<Users>("Users");

        //    ObjectId userId;
        //    var isValid = ObjectId.TryParse(Id, out userId);
        //    if (isValid)
        //    {
        //        var getQuery = Query<Users>.EQ(e => e.Id, userId);
        //        var user = collections.FindOne(getQuery);
        //        return PartialView("Details", user);
        //    }
        //    else
        //    {

        //        return PartialView("Error");
        //    }
        //}
        #endregion

        #region Delete Records
        public ActionResult Delete(string Id)
        {
            var _user = mongoDatabase.GetCollection<UserAdmin>("/users");

            //ObjectId userId;
            //var isValid = ObjectId.TryParse(Id, out userId);
            if (Id != null)
            {
                var getQuery = Query<UserAdmin>.EQ(e => e.lname, Id);
                _user.Remove(getQuery);
                return GetAll();
            }
            else
            {
                return View("Error");
            }
        }

        //public ActionResult Delete(string Id)
        //{
        //    var collections = mongoDatabase.GetCollection<Users>("Users");

        //    ObjectId userId;
        //    var isValid = ObjectId.TryParse(Id, out userId);
        //    if (isValid)
        //    {
        //        var getQuery = Query<Users>.EQ(e => e.Id, userId);
        //        collections.Remove(getQuery);
        //        return GetAll();
        //    }
        //    else
        //    {
        //        return View("Error");
        //    }
        //}
        #endregion
    }
}