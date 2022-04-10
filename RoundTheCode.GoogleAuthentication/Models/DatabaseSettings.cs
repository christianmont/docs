namespace MongoDBWebAPI.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string DocsCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string DocsCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }
}