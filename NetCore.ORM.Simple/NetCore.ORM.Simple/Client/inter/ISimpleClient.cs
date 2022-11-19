﻿using NetCore.ORM.Simple.Entity;
using NetCore.ORM.Simple.Queryable;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

/*********************************************************
 * 命名空间 NetCore.ORM.Simple.Client
 * 接口名称 ISimpleClient
 * 开发人员：-nhy
 * 创建时间：2022/9/20 17:40:52
 * 描述说明：
 * 更改历史：
 * 
 * *******************************************************/
namespace NetCore.ORM.Simple
{
    public interface ISimpleClient
    {
        public ISimpleCommand<TEntity> Insert<TEntity>(TEntity entity) where TEntity : class, new();
        public ISimpleCommand<TEntity> Insert<TEntity>(List<TEntity> entitys) where TEntity : class, new();

        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public ISimpleCommand<TEntity> Update<TEntity>(TEntity entity) where TEntity : class, new();
        public ISimpleCommand<TEntity> Update<TEntity>(List<TEntity> entitys) where TEntity : class, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>

        public ISimpleCommand<TEntity> Delete<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, new();
        public ISimpleCommand<TEntity> Delete<TEntity>(TEntity entity) where TEntity : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public ISimpleQueryable<T1> Queryable<T1>() where T1 : class, new();

        public ISimpleQueryable<T1, T2> Queryable<T1, T2>(Expression<Func<T1, T2, JoinInfoEntity>> expression) where T1 : class;


        public ISimpleQueryable<T1, T2, T3> Queryable<T1, T2, T3>(Expression<Func<T1, T2, T3, JoinInfoEntity>> expression) where T1 : class;
       
        public ISimpleQueryable<T1, T2, T3, T4> Queryable<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, JoinInfoEntity>> expression) where T1 : class;
        public ISimpleQueryable<T1, T2, T3, T4, T5> Queryable<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, JoinInfoEntity>> expression) where T1 : class;


        public ISimpleQueryable<T1, T2, T3, T4, T5, T6> Queryable<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, JoinInfoEntity>> expression) where T1 : class;
        
        public ISimpleQueryable<T1, T2, T3, T4, T5, T6, T7> Queryable<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, JoinInfoEntity>> expression) where T1 : class;
        
        public ISimpleQueryable<T1, T2, T3, T4, T5, T6, T7, T8> Queryable<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, JoinInfoEntity>> expression) where T1 : class;
        
        public ISimpleQueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinInfoEntity>> expression) where T1 : class;


        public ISimpleQueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, JoinInfoEntity>> expression) where T1 : class;
       
        public ISimpleQueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, JoinInfoEntity>> expression) where T1 : class;
        
        public ISimpleQueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, JoinInfoEntity>> expression) where T1 : class;

        public Task<int> SaveChangeAsync();

        public int SaveChange();

        public void SetAOPLog(Action<string,DbParameter[]> action);

        public void SetAttr(Type Table = null, Type Column = null);

        public void BeginTransaction();

        public  Task BeginTransactionAsync();

        public void Commit();

        public  Task CommitAsync();


        public void RollBack();

        public  Task RollBackAsync();

        //public List<T1> GetEntity<T, T1>(T t, Expression<Func<T, T1>> expression);

        //public List<T2> GetEntity<T, T1, T2>(T t, Expression<Func<T, T1>> expression, Expression<Func<T1, T2>> expression1);

        //public List<T> GetEntity<T>(Dictionary<string, object> data);
    }
}
