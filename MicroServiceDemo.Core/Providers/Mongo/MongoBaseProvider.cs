using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MicroServiceDemo.Core.Providers.Mongo
{
    public abstract class MongoBaseProvider<T> where T : class
    {
        /// <summary>
        /// 默认数据库名
        /// 如果连接字符串未指定数据库名, 则使用该数据库名
        /// </summary>
        public string DefaultDBName = string.Empty;

        private string databaseName = string.Empty;

        public string DatabaseName
        {
            get
            {
                if (string.IsNullOrEmpty(databaseName))
                {
                    return DefaultDBName;
                }
                return databaseName;
            }
            private set { this.databaseName = value; }
        }
        public string collectionName = "NoName";
        public MongoClient client = null;
        public IMongoDatabase database = null;
        public MongoBaseProvider(string conUrl)
        {
            BsonSerializer.RegisterSerializationProvider(new MDBsonSerializationProvider());

            var mongoConnectionUrl = new MongoUrl(conUrl);
            if (!string.IsNullOrEmpty(mongoConnectionUrl.DatabaseName))
            {
                DatabaseName = mongoConnectionUrl.DatabaseName;
            }

            var setting = MongoClientSettings.FromUrl(mongoConnectionUrl);
            client = new MongoClient(setting);
        }

        public IMongoCollection<T> GetCollection()
        {
            var database = client.GetDatabase(DatabaseName);
            return database.GetCollection<T>(collectionName);
        }

        #region 新增

        public T FindOneAndReplace(Expression<Func<T, bool>> filter, T replacement, bool isUpsert = false)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            FindOneAndReplaceOptions<T> options = new FindOneAndReplaceOptions<T> { IsUpsert = isUpsert };
            return myCollection.FindOneAndReplaceAsync(filter, replacement, options).GetAwaiter().GetResult();
        }

        public bool Insert(T model)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            var task = myCollection.InsertOneAsync(model);
            task.Wait();
            return task.IsCompleted;
        }

        public bool InsertMany(List<T> models)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            var task = myCollection.InsertManyAsync(models);
            task.Wait();
            return task.IsCompleted;
        }

        #endregion

        #region 修改

        public UpdateResult UpdateOne(FilterDefinition<T> filter, UpdateDefinition<T> update, bool isUpsert = false)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            UpdateOptions options = new UpdateOptions() { IsUpsert = isUpsert };
            return myCollection.UpdateOneAsync(filter, update, options).GetAwaiter().GetResult();
        }

        public UpdateResult UpdateOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, bool isUpsert = false)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            UpdateOptions options = new UpdateOptions() { IsUpsert = isUpsert };
            return myCollection.UpdateOneAsync<T>(filter, update, options).GetAwaiter().GetResult();
        }

        public UpdateResult UpdateAll(FilterDefinition<T> filter, UpdateDefinition<T> update, bool isUpsert = false)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            UpdateOptions options = new UpdateOptions() { IsUpsert = isUpsert };
            return myCollection.UpdateManyAsync(filter, update, options).GetAwaiter().GetResult();
        }

        public UpdateResult UpdateAll(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, bool isUpsert = false)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            UpdateOptions options = new UpdateOptions() { IsUpsert = isUpsert };
            return myCollection.UpdateManyAsync<T>(filter, update, options).GetAwaiter().GetResult();
        }

        public T FindOneAndUpdate(FilterDefinition<T> filter, UpdateDefinition<T> update, bool isUpsert = false)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            FindOneAndUpdateOptions<T> options = new FindOneAndUpdateOptions<T> { IsUpsert = isUpsert };
            return myCollection.FindOneAndUpdateAsync(filter, update, options).GetAwaiter().GetResult();
        }

        #endregion

        #region 删除

        public DeleteResult DeleteOne(FilterDefinition<T> filter)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            return myCollection.DeleteOneAsync(filter).GetAwaiter().GetResult();
        }

        public DeleteResult DeleteOne(Expression<Func<T, bool>> filter)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            return myCollection.DeleteOneAsync<T>(filter).GetAwaiter().GetResult();
        }

        public DeleteResult DeleteMany(FilterDefinition<T> filter)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            return myCollection.DeleteManyAsync(filter).GetAwaiter().GetResult();
        }

        public DeleteResult DeleteMany(Expression<Func<T, bool>> filter)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            return myCollection.DeleteManyAsync<T>(filter).GetAwaiter().GetResult();
        }

        #endregion

        #region  查询

        

        public List<TEntity> Find<TEntity>(FilterDefinition<T> filter, FindOptions<T, TEntity> options = null)
        {
            var result = new List<TEntity>();
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            result = myCollection.FindAsync<TEntity>(filter, options).GetAwaiter().GetResult().ToListAsync().GetAwaiter().GetResult();
            return result;
        }

        public List<U> Find<T, U>(FilterDefinition<T> filter, FindOptions<T, U> options = null)
        {
            var result = new List<U>();
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            result = myCollection.FindAsync<U>(filter, options).GetAwaiter().GetResult().ToListAsync().GetAwaiter().GetResult();
            return result;
        }

        public List<T> Find(FilterDefinition<T> filter, FindOptions<T, T> options = null)
        {
            var result = new List<T>();
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            result = myCollection.FindAsync<T>(filter, options).GetAwaiter().GetResult().ToListAsync().GetAwaiter().GetResult();
            return result;
        }

        public List<T> Find(Expression<Func<T, bool>> filter, FindOptions<T, T> options = null)
        {
            var result = new List<T>();
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            result = myCollection.FindAsync<T>(filter, options).GetAwaiter().GetResult().ToListAsync().GetAwaiter().GetResult();
            return result;
        }

        public List<TEntity> Aggregate<TEntity>(PipelineDefinition<T, TEntity> filter, AggregateOptions options = null)
        {
            var result = new List<TEntity>();
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            result = myCollection.AggregateAsync<TEntity>(filter, options).GetAwaiter().GetResult().ToListAsync().GetAwaiter().GetResult();
            return result;
        }

        public List<T> Aggregate(PipelineDefinition<T, T> pipeline, AggregateOptions options = null)
        {
            var result = new List<T>();
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            result = myCollection.AggregateAsync<T>(pipeline, options).GetAwaiter().GetResult().ToListAsync().GetAwaiter().GetResult();
            return result;
        }

        public T FindOne(FilterDefinition<T> filter, FindOptions<T, T> options = null)
        {
            options = options ?? new FindOptions<T, T> { Limit = 1 };
            return Find(filter, options).FirstOrDefault();
        }

        public U FindOne<T, U>(FilterDefinition<T> filter, FindOptions<T, U> options = null)
        {
            options = options ?? new FindOptions<T, U>();
            if (options == null)
            {
                options = new FindOptions<T, U>() { Limit = 1 };
            }
            else
            {
                options.Limit = 1;
            }
            var result = Find(filter, options);
            return result.FirstOrDefault();
        }

        public T FindOne(Expression<Func<T, bool>> filter, FindOptions<T, T> options = null)
        {
            options = options ?? new FindOptions<T, T>();
            options.Limit = 1;
            if (options == null)
            {
                options = new FindOptions<T, T>() { Limit = 1 };
            }
            else
            {
                options.Limit = 1;
            }
            var result = Find(filter, options);
            return result.FirstOrDefault();
        }

        public long Count(FilterDefinition<T> filter, CountOptions option = null)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            var aggregateResult = myCollection.Aggregate().Match(filter).Group(new BsonDocument(){
                    {"_id","null"},
                    {"count",new BsonDocument("$sum",1)}
                });
            var results = aggregateResult.ToListAsync().GetAwaiter().GetResult();

            if (results.Count == 1)
                return Convert.ToInt64(results[0].GetElement("count").Value);
            else
                return 0;
        }

        public long Count(Expression<Func<T, bool>> filter)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            return myCollection.CountDocumentsAsync<T>(filter).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 统计某字段的和
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="sumKeyField">BSon关键词字段</param>
        /// <param name="sumField">BSon被统计字段</param>
        /// <returns></returns>
        public long Sum(FilterDefinition<T> filter, string sumKeyField, string sumField)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);

            var aggregate = myCollection.Aggregate()
                .Match(filter)
                .Group(new BsonDocument { { "_id", "$" + sumKeyField }, { "sum", new BsonDocument("$sum", "$" + sumField) } });

            var results = aggregate.ToListAsync().GetAwaiter().GetResult();

            if (results.Count == 1)
                return Convert.ToInt64(results[0].GetElement("sum").Value);
            else
                return 0;
        }

        /// <summary>
        /// 统计某字段的和
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="sumKeyField">BSon关键词字段</param>
        /// <param name="sumField">BSon被统计字段</param>
        /// <returns></returns>
        public long Sum(Expression<Func<T, bool>> filter, string sumKeyField, string sumField)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);

            var aggregate = myCollection.Aggregate()
                .Match(filter)
                .Group(new BsonDocument { { "_id", "$" + sumKeyField }, { "sum", new BsonDocument("$sum", "$" + sumField) } });

            var results = aggregate.ToListAsync().GetAwaiter().GetResult();

            if (results.Count == 1)
                return (long)results[0].GetElement("sum").Value;
            else
                return 0;
        }

        #endregion

        #region 批量写入

        public BulkWriteResult BulkWrite(IEnumerable<WriteModel<T>> models)
        {
            var database = client.GetDatabase(DatabaseName);
            var myCollection = database.GetCollection<T>(collectionName);
            return myCollection.BulkWriteAsync(models).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool BulkWriteOne(T entity)
        {
            InsertOneModel<T> model = new InsertOneModel<T>(entity);
            var inserResult = this.BulkWrite(new[] { model });
            if (inserResult != null && inserResult.InsertedCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 把顶级的查询分拆到 $or 查询中
        /// </summary>
        /// <typeparam name="TBson"></typeparam>
        /// <param name="doc"></param>
        /// <returns></returns>
        public TBson BreakOrQuery<TBson>(TBson doc) where TBson : BsonDocument, new()
        {
            if (!doc.Contains("$or") || !(doc["$or"] is BsonArray)) return doc;
            var orArr = new BsonArray();
            foreach (var elValue in doc["$or"] as BsonArray)
            {
                var el = elValue.Clone() as BsonDocument;
                if (el == null) return doc;
                foreach (var outerEl in doc)
                {
                    if (outerEl.Name == "$or") continue;
                    if (el.Contains(outerEl.Name)) return doc;
                    el.Add(outerEl.Name, outerEl.Value);
                }
                orArr.Add(el);
            }
            var newDoc = new TBson { { "$or", orArr } };
            return newDoc;
        }

        public bool IsObjectId(string id)
        {
            try
            {
                ObjectId obj;
                return ObjectId.TryParse(id, out obj);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
