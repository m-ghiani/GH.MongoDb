using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GH.MongoDb.Interfaces
{
    public interface IGenericRepositoryAsync<T, in TKey> : IGenericReadRepositoryAsync<T,TKey>
        where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
    {
        Task Add(T entity, CancellationToken token = default(CancellationToken));
        Task Add(List<T> entities, CancellationToken token = default(CancellationToken));
        Task Delete(TKey id, CancellationToken token = default(CancellationToken));
        Task DropCollection(CancellationToken token = default(CancellationToken));
        Task<T> Get(TKey id, CancellationToken token = default(CancellationToken));
        Task Update(T entity, CancellationToken token = default(CancellationToken));
        Task Update(TKey id, T entitty, CancellationToken token = default(CancellationToken));
        Task Update(TKey id, string fieldName, object fieldValue, CancellationToken token = default(CancellationToken));
    }
}