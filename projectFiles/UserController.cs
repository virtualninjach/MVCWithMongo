using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVCWithMongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace MVCWithMongo.Controllers
{
    public class UserController : Controller
    {
        
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registration(UserModel um)
        {
            //make connection to mongodb
            var client = new MongoClient("mongodb://localhost:27017");
            var databse = client.GetDatabase("MVCTestDB");
            var collection = databse.GetCollection<UserModel>("Users");

            //grab current username
            string userId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            //Create guid for identification
            Guid g;
            g = Guid.NewGuid();
            string s = g.ToString();

            var objDocument = new UserModel
            {
                UserGuidID = s,
                UserName = userId,
                Password = um.Password,
                Email = um.Email,
                PhoneNo = um.PhoneNo,
                Address = um.Address

            };
                
            await collection.InsertOneAsync(objDocument);
            return RedirectToAction("GetUsers");
        }
        
        public async Task<ActionResult> GetUsers()
        {
            var users = await GetUserList();
            return View(users);
        }

        private async Task<IList<UserModel>> GetUserList()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var databse = client.GetDatabase("MVCTestDB");
            var collection = databse.GetCollection<UserModel>("Users");
            return await collection.Find(new BsonDocument()).ToListAsync();
        }
        
        public async  Task<ActionResult> Delete(string id)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var databse = client.GetDatabase("MVCTestDB");
            var collection = databse.GetCollection<UserModel>("Users");
            await collection.FindOneAndDeleteAsync(Builders<UserModel>.Filter.Eq("UserGuidID", id));
                   
            return RedirectToAction("GetUsers");
        }
        
        
        public async Task<ActionResult> Edit(string id)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var databse = client.GetDatabase("MVCTestDB");
            var collection = databse.GetCollection<UserModel>("Users");
            var filter = Builders<UserModel>.Filter.Eq("UserGuidID", id);

            var document = await collection.Find(filter).FirstAsync();
            
            return View(document);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UserModel um)
        {

            var client = new MongoClient("mongodb://localhost:27017");
            var databse = client.GetDatabase("MVCTestDB");
            var collection = databse.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("UserGuidID", um.UserGuidID);

            var newModel = new BsonDocument
            {

                {"UserGuidID",um.UserGuidID },
                {"UserName",um.UserName },
                {"Password",um.Password},
                {"Email",um.Email},
                {"PhoneNo",um.PhoneNo },
                {"Address",um.Address }
        };
      await collection.ReplaceOneAsync(filter,newModel);

           
                return RedirectToAction("GetUsers");
           
        }
        

    }
}
