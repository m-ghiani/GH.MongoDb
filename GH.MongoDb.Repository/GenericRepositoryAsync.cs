using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GH.MongoDb.Interfaces;
using MongoDB.Driver;

namespace GH.MongoDb.Repository
{
    public abstract class GenericRepositoryAsync<T, TKey> : GenericReadRepositoryAsync<T, TKey>, IDisposable, IGenericRepositoryAsync<T, TKey>
        where T : IDocument<TKey>, new() 
        where TKey : IEquatable<TKey>
    {
        protected GenericRepositoryAsync(IMongoDbConnector connector) : this(connector, null)
        {
        }

        protected GenericRepositoryAsync(IMongoDbConnector connector, string collectionName):base(connector,collectionName)
        {
        }

        public event EventHandler<EntityEventArgs<T,TKey>> EntityAdded;
        public event EventHandler<EntitiesEventArgs<T, TKey>> EntitiesAdded;
        public event EventHandler<EntityEventArgs<T, TKey>> EntityDeleted;


        public virtual void Dispose() => GC.SuppressFinalize(this);


        /// <summary>
        /// Add entity to collection
        /// </summary>
        /// <param name="entity">document to add</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Add(T entity, CancellationToken token = default(CancellationToken))
        {
            await Collection.InsertOneAsync(entity, new InsertOneOptions() {BypassDocumentValidation = true}, token);
            EntityAdded?.Invoke(this, new EntityEventArgs<T,TKey>{Entity = entity});
        }

        /// <summary>
        /// Add entity to collection
        /// </summary>
        /// <param name="entities">list of documents to add</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Add(List<T> entities, CancellationToken token = default(CancellationToken))
        {
            await Collection.InsertManyAsync(entities, new InsertManyOptions() {BypassDocumentValidation = true},token);
            EntitiesAdded?.Invoke(this, new EntitiesEventArgs<T, TKey> { Entities = entities });
        }

        /// <summary>
        /// Add entity to collection
        /// </summary>
        /// <param name="id">id of document to remove</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Delete(TKey id, CancellationToken token = default(CancellationToken))
        {
            if (CollectionExist)
            {
                var entity = await this.Get(id, token);
                await Collection.FindOneAndDeleteAsync(Builders<T>.Filter.Eq(e => e.Id, id), new FindOneAndDeleteOptions<T, T>(), token);
                EntityDeleted?.Invoke(this, new EntityEventArgs<T, TKey>{Entity = entity});
            }
        }

        /// <summary>
        /// Delete entire collection
        /// </summary>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task DropCollection(CancellationToken token = default(CancellationToken))
        {
            if (CollectionExist)
                await Connector.Db.DropCollectionAsync(CollectionName, token);
        }



        /// <summary>
        /// Update document
        /// </summary>
        /// <param name="entity">document to update</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Update(T entity, CancellationToken token = default(CancellationToken))
        {
            if (!CollectionExist) return;
            await Collection.ReplaceOneAsync(_ => Equals(_.Id, entity.Id), entity, cancellationToken: token);
        }

        /// <summary>
        /// Update document
        /// </summary>
        /// <param name="id">id of document to update</param>
        /// <param name="entity">new document</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Update(TKey id, T entity, CancellationToken token = default(CancellationToken))
        {
            if (!CollectionExist) return;
            await Collection.ReplaceOneAsync(_ => Equals(_.Id, id), entity, cancellationToken: token);
        }

        /// <summary>
        /// Update document's field
        /// </summary>
        /// <param name="id">id of document to update</param>
        /// <param name="fieldName">field name</param>
        /// <param name="fieldValue">new field value</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Update(TKey id, string fieldName, object fieldValue, CancellationToken token = default(CancellationToken))
        {
            var filter = Builders<T>.Filter.Eq(_ => Equals(_.Id, id), true);
            var update = Builders<T>.Update.Set(fieldName, fieldValue);
            await Collection.UpdateOneAsync(filter, update, cancellationToken: token);
        }

    }

    public class EntitiesEventArgs<T, TKey> : EventArgs where T : IDocument<TKey>
    {
        public IEnumerable<T> Entities { get; set; }
    }

    public class EntityEventArgs<T,TKey> : EventArgs where T: IDocument<TKey>
    {
        public T Entity { get; set; }
    }
}
