using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GH.MongoDb.Interfaces;
using GH.MongoDb.Repository.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GH.MongoDb.Repository
{
    public abstract class GenericRepositoryAsync<T, TKey> : IDisposable, IGenericRepositoryAsync<T, TKey> where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
    {

        protected GenericRepositoryAsync(IMongoDbConnector connector, string collectionName)
        {
            Connector = connector;
            CollectionName = collectionName;
            Collection = Connector.Db.GetCollection<T>(collectionName);
            CollectionExist = Collection != null;
        }

        public readonly string CollectionName;
        public IMongoCollection<T> Collection { get; set; }

        public bool CollectionExist;

        /// <summary>
        /// Get the connector to database
        /// </summary>
        public IMongoDbConnector Connector { get; }

        public virtual void Dispose() => GC.SuppressFinalize(this);

        /// <summary>
        /// Get documents count
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task<long> Count(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken)) => CollectionExist ? await Collection.CountAsync(filter, cancellationToken: token) : 0;

        /// <summary>
        /// Add entity to collection
        /// </summary>
        /// <param name="entity">document to add</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Add(T entity, CancellationToken token = default(CancellationToken)) => await Collection.InsertOneAsync(entity, new InsertOneOptions() { BypassDocumentValidation = true }, token);

        /// <summary>
        /// Add entity to collection
        /// </summary>
        /// <param name="entities">list of documents to add</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Add(List<T> entities, CancellationToken token = default(CancellationToken)) => await Collection.InsertManyAsync(entities, new InsertManyOptions() { BypassDocumentValidation = true }, token);

        /// <summary>
        /// Add entity to collection
        /// </summary>
        /// <param name="id">id of document to remove</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task Delete(TKey id, CancellationToken token = default(CancellationToken))
        {
            if (CollectionExist)
                await Collection.FindOneAndDeleteAsync(Builders<T>.Filter.Eq(e => e.Id, id),
                new FindOneAndDeleteOptions<T, T>(), token);
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
        /// Check if document exist
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task<bool> Exist(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken)) => CollectionExist && await Collection.Find(filter).AnyAsync(token);

        /// <summary>
        /// Check if collection exists
        /// </summary>
        /// <param name="token">cancellation token async support</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistCollection(CancellationToken token = default(CancellationToken)) => CollectionExist;

        /// <summary>
        /// Get all collection's documents
        /// </summary>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, null, null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="limit">limit number of returned documents</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(int? limit, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, new PagingSettings(null, limit), null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, null, null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="limit">limit number of returned documents</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, int? limit, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, new PagingSettings(null, limit), null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="skip">number of documents to skip</param>
        /// <param name="limit">limit number of returned documents</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, int? skip, int? limit, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, new PagingSettings(skip, limit), null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, PagingSettings settings, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, settings, null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, PagingSettings settings, IEnumerable<SortingField> sortings, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, settings, sortings, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, PagingSettings settings, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, settings, null, projection, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, PagingSettings settings, IEnumerable<SortingField> sortings, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, settings, sortings, projection, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, null, null, projection, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, IEnumerable<SortingField> sortings, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, null, sortings, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="filter">lambda expression filter</param>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, IEnumerable<SortingField> sortings, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(filter, null, sortings, projection, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="skip">number of documents to skip</param>
        /// <param name="limit">limit number of returned documents</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(int? skip, int? limit, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, new PagingSettings(skip, limit), null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(PagingSettings settings, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, settings, null, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(PagingSettings settings, IEnumerable<SortingField> sortings, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, settings, sortings, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(PagingSettings settings, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, settings, null, projection, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="settings">Istance of PagingSettings</param>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(PagingSettings settings, IEnumerable<SortingField> sortings, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, settings, sortings, projection, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(IEnumerable<SortingField> sortings, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, null, sortings, null, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, null, null, projection, token);

        /// <summary>
        /// Get collection's documents
        /// </summary>
        /// <param name="sortings">list of sortingField</param>
        /// <param name="projection">defines a projection document to specify or restrict fields to return</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return list of documents</returns>
        public virtual async Task<IEnumerable<T>> Get(IEnumerable<SortingField> sortings, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken)) => (IEnumerable<T>)await GetCollection(null, null, sortings, projection, token);

        /// <summary>
        /// Get document
        /// </summary>
        /// <param name="id">id of document to retrieve</param>
        /// <param name="token">cancellation token async support</param>
        /// <returns>Return document</returns>
        public virtual async Task<T> Get(TKey id, CancellationToken token = default(CancellationToken))
        {
            if (!CollectionExist) return new T();
            var filter = Builders<IDocument<TKey>>.Filter.Eq("_id", id);
            return await Collection.Find(e => filter.Inject()).SingleAsync(token);
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



        protected virtual async Task<IEnumerable<object>> GetCollection(Expression<Func<T, bool>> filter,
            PagingSettings paging, IEnumerable<SortingField> sortings, Expression<Func<T, object>> projection, CancellationToken token = default(CancellationToken))
        {
            if (!CollectionExist) return new List<object>();
            try
            {
                var collection = filter == null ? Collection.Find(_ => true) : Collection.Find(filter);
                var sortingBuilder = new SortDefinitionBuilder<T>();
                var sorts = new List<SortDefinition<T>>();
                if (sortings != null)
                {
                    sorts.AddRange(from sf in sortings
                                   where !sf.IsNull
                                   select sf.SortingMode == SortingModes.Ascending
                                       ? sortingBuilder.Ascending(sf.FieldName)
                                       : sortingBuilder.Descending(sf.FieldName));
                }
                collection.Sort(sortingBuilder.Combine(sorts));
                if (projection != null)
                {
                    collection.Project(projection);
                }
                if (paging == null) return (IEnumerable<object>)await collection.ToListAsync(CancellationToken.None);
                if (paging.Skip != null) collection = collection.Skip(paging.Skip);
                if (paging.Limit != null) collection = collection.Limit(paging.Limit);

                return (IEnumerable<object>)await collection.ToListAsync(token);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



    }
}
