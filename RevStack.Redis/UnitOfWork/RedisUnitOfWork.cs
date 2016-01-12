using System;
using System.Threading.Tasks;
using RevStack.Pattern;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace RevStack.Redis
{
    public class RedisUnitOfWork<TEntity, TKey> : IUnitOfWork<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        protected readonly IRedisClient _client;
        private readonly IRedisTypedClient<TEntity> _typedClient;
        private readonly string _storeKey;
        private readonly string _removeKey;
        private bool _disposedLists;
        public RedisUnitOfWork(RedisDataContext context)
        {
            _disposedLists = false;
            _client = context.Client();
            _typedClient = _client.As<TEntity>();
            string type = typeof(TEntity).ToString();
            _storeKey = RedisUtils.GetListKey(type, RedisOperation.Save);
            _removeKey = RedisUtils.GetListKey(type, RedisOperation.Delete);
        }

        public void Commit()
        {
            save();
            remove();
            disposeLists();
        }

        public Task CommitAsync()
        {
            Commit();
            return Task.FromResult(true);
        }

        private void save()
        {
            IRedisList<TEntity> storeList = _typedClient.Lists[_storeKey];
            foreach (var entity in storeList)
            {
                _typedClient.Store(entity);
            }
        }

        private void remove()
        {
            IRedisList<TEntity> removeList = _typedClient.Lists[_removeKey];
            foreach (var entity in removeList)
            {
                _typedClient.Delete(entity);
            }
        }

        #region "Dispose"
        private void disposeLists()
        {
            if(!_disposedLists)
            {
                _typedClient.SetSequence(0);
                _client.Remove(_storeKey);
                _client.Remove(_removeKey);
                _disposedLists = true;
            }
        }

        private void disposeContext()
        {
            if (_client != null)
            {
                _client.Dispose();
                _typedClient.Dispose();
            }
        }

        ~RedisUnitOfWork()
        {
            disposeLists();
            Dispose(false);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    disposeLists();
                    disposeContext();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
