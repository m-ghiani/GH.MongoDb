using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GH.MongoDb.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GH.MongoDb.Repository.GeoExtension
{
    public static class Extension
    {
        public static async Task<IEnumerable<T>> Get<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, double latitude, double longitude, double distance, Expression<Func<T, bool>> filter, int? skip, int? limit, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, ILocationDocument, new() where TKey : IEquatable<TKey>
        {
            if (!(await repo.ExistCollection(token))) return new List<T>();
            FilterDefinition<T> query = null;
            if (distance > 100 && distance < 50000)
            {
                var gp = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(latitude, longitude));
                query = Builders<T>.Filter.Near(s => s.Location, gp, distance);
            }
            query = query != null ? query & filter : filter;

            return await repo.Collection.Find(query).Skip(skip).Limit(limit).ToListAsync(token);

        }

        public static async Task<long> Count<T, TKey>(this GenericRepositoryAsync<T, TKey> repo, double latitude, double longitude, double distance, Expression<Func<T, bool>> filter, CancellationToken token = default(CancellationToken)) where T : IDocument<TKey>, ILocationDocument, new() where TKey : IEquatable<TKey>
        {
            if (!(await repo.ExistCollection(token))) return 0;
            FilterDefinition<T> query = null;
            if (distance > 100 && distance < 50000)
            {
                var gp = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(latitude, longitude));
                query = Builders<T>.Filter.Near(s => s.Location, gp, distance);
            }
            query = query != null ? query & filter : filter;

            return await repo.Collection.CountAsync(query, cancellationToken: token);

        }

    }
}
