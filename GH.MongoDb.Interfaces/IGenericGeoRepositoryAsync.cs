using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GH.MongoDb.Interfaces
{
    public interface IGenericGeoRepositoryAsync<T, in TKey> : IGenericRepositoryAsync<T, TKey>
        where T : IDocument<TKey>, ILocationDocument, new()
        where TKey : IEquatable<TKey>
    {
        Task<long> Count(double latitude, double longitude, double distance, Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<T>> Get(double latitude, double longitude, double distance, Expression<Func<T, bool>> filter, int? skip, int? limit, CancellationToken token = default(CancellationToken));
    }
}