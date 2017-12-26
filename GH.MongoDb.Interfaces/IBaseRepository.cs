namespace GH.MongoDb.Interfaces
{
    public interface IBaseRepository
    {
        IMongoDbConnector Connector { get; }
    }
}