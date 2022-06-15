using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Helpers;
using MongoDB.Bson.Serialization;

namespace MongoDBWebAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(IDatabaseSettings settings, IConfiguration configuration)
        {
            Configuration = configuration;
            var client = new MongoClient(Configuration["CONNECTION_STRING"]);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);

        }

        public IConfiguration Configuration { get; }

        public List<User> Get()
        {
            List<User> users;
            users = _users.Find(emp => true).ToList();
            return users;
        }

        public User Get(string id) =>
            _users.Find<User>(emp => emp.Id.Equals(id)).FirstOrDefault();

        public async Task<User> CreateDocument(User user)
        {
            var settings = new DatabaseSettings();
            MongoClient client = new MongoClient(Configuration["CONNECTION_STRING"]);
            var database = client.GetDatabase(Configuration["DatabaseSettings:DatabaseName"]);
            var collection = database.GetCollection<BsonDocument>(Configuration["DatabaseSettings:UsersCollectionName"]);

            var document = user.ToBsonDocument();
            await collection.InsertOneAsync(document);
            return BsonSerializer.Deserialize<User>(document);
        }

        public User GetUser(string UserName, string Password)
        {
            User u = _users.Find<User>(user => user.UserName == UserName).FirstOrDefault();
            if(u != null && Crypto.VerifyHashedPassword(u.Password, Password))
            {
                return u;
            }
            return null;
        }

        public User GetSiteUser(string Site, string UserId)
        {
            User u = _users.Find<User>(user => user.Site == Site && user.UserId == UserId).FirstOrDefault();
            if (u != null)
            {
                return u;
            }
            return null;
        }

        internal Task<IdentityResult> CreateAsync(string databaseName, object collection)
        {
            throw new NotImplementedException();
        }

        internal Task<IdentityResult> CreateDocument(object databaseName, string v)
        {
            throw new NotImplementedException();
        }
    }
}