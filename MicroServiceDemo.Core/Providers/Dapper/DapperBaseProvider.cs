using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MicroServiceDemo.Core.Providers.Dapper
{
    public class DapperBaseProvider<T> where T : class
    {
        #region Ctor

        public DapperBaseProvider(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            DbConnection = connectionString;
        }

        #endregion

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected string DbConnection;

        protected SqlConnection OpenDbConnection()
        {
            if (DbConnection == null)
            {
                throw new Exception("Connection string \"DbConnection\" can not be null.");
            }

            return new SqlConnection(DbConnection);
        }

        #region 表达式快捷
        protected IFieldPredicate Field(Expression<Func<T, object>> expression, Operator op, object value, bool not = false)
        {
            return Predicates.Field<T>(expression, op, value, not);
        }

        protected ISort Sort(Expression<Func<T, object>> expression, bool ascending = true)
        {
            return Predicates.Sort<T>(expression, ascending);
        }
        #endregion

        #region 执行sql

        /// <summary>
        /// 查询并返回一条数据
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public T QueryFirstOrDefault(string sql, object param = null)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.QueryFirstOrDefault<T>(sql, param);
            }
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public List<T2> Query<T2>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            where T2 : class
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.Query<T2>(sql, param,transaction,buffered,commandTimeout,commandType).ToList();
            }
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public List<T> Query(string sql, object param = null)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.Query<T>(sql, param).ToList();
            }
        }

        public IEnumerable<U> Query<TFirst, TSecond, U>(string sql, Func<TFirst, TSecond, U> map, object param = null, string splitOn = "id")
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.Query(sql, map, param, splitOn: splitOn).ToList();
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public int Execute(string sql, object param = null)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.Execute(sql, param);
            }
        }

        #endregion

        #region 增

        /// <summary>
        /// 新增对象数据
        /// </summary>
        /// <param name="obj">新增对象</param>
        /// <returns></returns>
        public long Insert(T obj)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.Insert(obj);
            }
        }

        /// <summary>
        /// 批量新增对象数据
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        public void Insert(IEnumerable<T> entities, DbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                conn.Insert(entities, transaction, commandTimeout);
            }
        }

        #endregion

        #region 删

        /// <summary>
        /// 删除对象数据
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public bool Delete(object where)
        {
            using (var db = OpenDbConnection())
            {
                db.Open();
                return db.Delete<T>(where);
            }
        }

        #endregion

        #region 改

        /// <summary>
        /// 修改对象数据
        /// </summary>
        /// <param name="entity">修改的对象</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public bool Update(T entity, DbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.Update(entity, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 修改对象数据
        /// </summary>
        /// <param name="func">修改对象（lambda表达式）</param>
        /// <returns></returns>
        public bool Update(Func<SqlConnection, bool> func)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return func(conn);
            }
        }

        #endregion

        #region 查

        /// <summary>
        /// 获取所有详情
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public List<T> GetList(object where)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.GetList<T>(where).ToList();
            }
        }

        public List<T> GetList(object where, IList<ISort> sorts)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.GetList<T>(where, sorts).ToList();
            }
        }

        /// <summary>
        /// 获取所有详情
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="sort">排序字段</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">每页数目</param>
        /// <returns></returns>
        public List<T> GetList(object where, IList<ISort> sort, int pageIndex, int pageSize)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                //分页从1开始
                return conn.GetPage<T>(where, sort, pageIndex - 1, pageSize).ToList();
            }
        }

        /// <summary>
        /// 根据主键Id获取详情
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        public T Get(int id)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();
                return conn.Get<T>(id);
            }
        }

        /// <summary>
        /// 获取所有详情取第一条
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public T Single(object where)
        {
            using (var db = OpenDbConnection())
            {
                db.Open();
                return db.GetList<T>(where).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据条件获取数目
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public int Count(object predicate, DbTransaction transaction = null, int? commandTimeout = null)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();

                return conn.Count<T>(predicate, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public bool Exists(object where)
        {
            return Count(where) > 0;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="sort">排序</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandTimeout">过期时间</param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<T> Find(object predicate = null, IList<ISort> sort = null, DbTransaction transaction = null, int? commandTimeout = null, bool buffered = false)
        {
            using (var conn = OpenDbConnection())
            {
                conn.Open();

                return conn.GetList<T>(predicate, sort, transaction, commandTimeout, buffered);
            }
        }

        #endregion
    }
}
