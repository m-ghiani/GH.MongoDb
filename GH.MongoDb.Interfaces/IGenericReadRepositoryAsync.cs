using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GH.MongoDb.Interfaces
{
    public interface IGenericReadRepositoryAsync<T, in TKey>:IBaseRepository
        where T : IDocument<TKey>, new() where TKey : IEquatable<TKey>
    {
        Task<long> Count(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken));
        Task<bool> Exist(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken));
        Task<bool> ExistCollection(CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(int? skip, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(int? skip, int? limit, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, int? limit, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, int? skip, int? limit, CancellationToken token = default(CancellationToken));

    }
}