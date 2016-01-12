using System;
using ServiceStack.Redis;
using RevStack.Mvc;

namespace RevStack.Redis
{
    public static class RedisUtils
    {

        private const string ID_PROPERTY = "Id";
        private const int STRING_KEY_LENGTH = 12;
        public static string GetListKey(string type,RedisOperation op)
        {
            string operation = "";
            if(op==RedisOperation.Delete)
            {
                operation = "delete";
            }
            else
            {
                operation = "store";
            }
            string key = "urn:" + type + ":" + operation;
            return key;
        }

        public static TEntity SetEntityIdProperty<TEntity>(TEntity entity,IRedisClient client)
        {
            Type type = entity.GetType();
            var info = type.GetProperty(ID_PROPERTY);
            var value = info.GetValue(entity, null);
            if (info.PropertyType == typeof(int))
            {
                if ((int)value == default(int))
                {
                    info.SetValue(entity, client.IncrementValue(ID_PROPERTY));
                }
            }
            else if (info.PropertyType == typeof(long))
            {
                if ((long)value == default(long))
                {
                    info.SetValue(entity, client.IncrementValue(ID_PROPERTY));
                }
            }
            else if (info.PropertyType == typeof(Guid))
            {
                if ((Guid)value == default(Guid))
                {
                    info.SetValue(entity, Guid.NewGuid());
                }
            }
            else if (info.PropertyType == typeof(String))
            {
                if ((String)value == default(String))
                {
                    info.SetValue(entity, Utils.GenerateRandomString(STRING_KEY_LENGTH));
                }
            }

            return entity;
        }
    }
}
