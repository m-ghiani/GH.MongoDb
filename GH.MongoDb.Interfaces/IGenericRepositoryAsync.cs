using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GH.MongoDb.Interfaces
{
    public interface IGenericRepositoryAsync<T, in TKey> where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
    {
        IMongoDbConnector Connector { get; }
        Task Add(T entity, CancellationToken token = default(CancellationToken));
        Task Add(List<T> entities, CancellationToken token = default(CancellationToken));
        Task<long> Count(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken));
        Task Delete(TKey id, CancellationToken token = default(CancellationToken));
        Task DropCollection(CancellationToken token = default(CancellationToken));
        Task<bool> Exist(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken));
        Task<bool> ExistCollection(CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(int? skip, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(int? skip, int? limit, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, int? limit, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, int? skip, int? limit, CancellationToken token = default(CancellationToken));
        Task<T> Get(TKey id, CancellationToken token = default(CancellationToken));
        Task Update(T entity, CancellationToken token = default(CancellationToken));
        Task Update(TKey id, T entitty, CancellationToken token = default(CancellationToken));
        Task Update(TKey id, string fieldName, object fieldValue, CancellationToken token = default(CancellationToken));
    }
}