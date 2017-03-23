using Surya.India.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Surya.India.Data.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        Guid InstanceId { get; }
        TEntity Find(params object[] keyValues);
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        IQueryable<TEntity> SqlQuery(string query, params object[] parameters);
        void Add(TEntity entity);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void InsertGraph(TEntity entity);
        void InsertGraphRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Detach(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        IRepositoryQuery<TEntity> Query();
    }
}