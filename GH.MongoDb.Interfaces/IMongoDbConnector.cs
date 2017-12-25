using System;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace GH.MongoDb.Interfaces
{
    public interface IMongoDbConnector : IDisposable
    {
        IMongoDatabase Db { get; }
        GridFSBucket GetBucket(string bucketName);
    }
}