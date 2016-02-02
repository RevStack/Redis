using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using RevStack.Pattern;

namespace RevStack.Redis
{
    public class RedisRepository<TEntity,TKey> : IRepository<TEntity,TKey> where TEntity : class, IEntity<TKey>
    {
        private readonly IRedisClient _client;
        private readonly IRedisTypedClient<TEntity> _typedClient;
        private readonly string _type;
        public RedisRepository(RedisDataContext context)
        {
            _client = context.Client();
            _typedClient = _client.As<TEntity>();
            _type = typeof(TEntity).ToString();
        }

        public IEnumerable<TEntity> Get()
        {
            var result =_typedClient.GetAll();
            return result;
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _typedClient.GetAll().AsQueryable().Where(predicate);
        }

        public TEntity Add(TEntity entity)
        {
            entity = RedisUtils.SetEntityIdProperty(entity,_client);
            _typedClient.Store(entity);
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            _typedClient.Store(entity);
            return entity;
        }

        public void Delete(TEntity entity)
        {
            _typedClient.Delete(entity);
        }

    }
}
