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
using RoundTheCode.GoogleAuthentication.Models;
using MongoDB.Bson.Serialization;

namespace MongoDBWebAPI.Services
{
    public class MakeDocService
    {
        private readonly IMongoCollection<Doc> _docs;
        public MakeDocService(IDatabaseSettings settings, IConfiguration configuration)
        {
            Configuration = configuration;
            var client = new MongoClient(Configuration["CONNECTION_STRING"]);
            var database = client.GetDatabase(settings.DatabaseName);

            _docs = database.GetCollection<Doc>(settings.DocsCollectionName);

        }

        public IConfiguration Configuration { get; }

        public List<Doc> Get()
        {
            List<Doc> docs;
            docs = _docs.Find(emp => true).ToList();
            return docs;
        }

        public Doc Get(string id) =>
            _docs.Find<Doc>(emp => emp.Id.Equals(id)).FirstOrDefault();

        public List<Doc> GetUserDocs(string id) {
            return _docs.Find<Doc>(emp => emp.User.Equals(id)).ToList();
        }

        public async Task<Doc> CreateDocument(Doc doc)
        {
            var settings = new DatabaseSettings();
            MongoClient client = new MongoClient(Configuration["CONNECTION_STRING"]);
            var database = client.GetDatabase(Configuration["DatabaseSettings:DatabaseName"]);
            var collection = database.GetCollection<BsonDocument>(Configuration["DatabaseSettings:DocsCollectionName"]);

            var document = doc.ToBsonDocument();
            await collection.InsertOneAsync(document);
            return BsonSerializer.Deserialize<Doc>(document);
        }

        public async Task UpdateDocument(List<string> text, string id)
        {
            var settings = new DatabaseSettings();
            MongoClient client = new MongoClient(Configuration["CONNECTION_STRING"]);
            var database = client.GetDatabase(Configuration["DatabaseSettings:DatabaseName"]);
            var collection = database.GetCollection<BsonDocument>(Configuration["DatabaseSettings:DocsCollectionName"]);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<BsonDocument>.Update.Set("Text", text);

            await collection.UpdateOneAsync(filter, update);
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