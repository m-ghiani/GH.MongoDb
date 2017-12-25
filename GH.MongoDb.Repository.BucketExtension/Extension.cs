using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GH.MongoDb.Interfaces;
using GH.MongoDb.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace GH.MongoDb.Repository.BucketExtension
{
    public static class Extension
    {
        public static async Task<ObjectId> UploadFile<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, string fileName, byte[] source, GridFSUploadOptions options, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
        {

            var bucket = repo.Connector.GetBucket(repo.CollectionName);
            if (bucket == null) return new ObjectId();
            return await bucket.UploadFromBytesAsync(fileName, source, options, token);
        }

        public static async Task<ObjectId> UploadFile<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, string fileName, byte[] source, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
        {
            var bucket = repo.Connector.GetBucket(repo.CollectionName);
            if (bucket == null) return new ObjectId();
            return await bucket.UploadFromBytesAsync(fileName, source, cancellationToken: token);
        }
        public static async Task<byte[]> DownloadFile<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, ObjectId id, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
        {
            var bucket = repo.Connector.GetBucket(repo.CollectionName);
            if (bucket == null) return null;
            return await bucket.DownloadAsBytesAsync(id, cancellationToken: token);
        }
        public static async Task<GridFSFileInfo> GetFileInfo<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, string fileName, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
        {
            var bucket = repo.Connector.GetBucket(repo.CollectionName);
            if (bucket == null) return null;
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Limit = 1,
                Sort = sort
            };
            return await bucket.Find(filter, options).FirstAsync(token);
        }
        public static async Task DeleteFile<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, ObjectId id, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
        {
            var bucket = repo.Connector.GetBucket(repo.CollectionName);
            if (bucket == null) return;
            await bucket.DeleteAsync(id, token);
        }
        public static async Task RenameFile<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, ObjectId id, string newFileName, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
        {
            var bucket = repo.Connector.GetBucket(repo.CollectionName);
            if (bucket == null) return;
            await bucket.RenameAsync(id, newFileName, token);
        }

    }
}
