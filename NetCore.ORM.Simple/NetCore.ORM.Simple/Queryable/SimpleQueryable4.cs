﻿using NetCore.ORM.Simple.Common;
using NetCore.ORM.Simple.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NetCore.ORM.Simple.SqlBuilder;

/*********************************************************
 * 命名空间 NetCore.ORM.Simple.Queryable
 * 接口名称 SimpleQueryable2
 * 开发人员：-nhy
 * 创建时间：2022/9/20 17:49:02
 * 描述说明：
 * 更改历史：
 * 
 * *******************************************************/
namespace NetCore.ORM.Simple.Queryable
{
    public class SimpleQueryable<T1,T2,T3,T4>:
        QueryResult<T1>,ISimpleQueryable<T1,T2,T3,T4>where T1 : class
    {
        public SimpleQueryable(Expression<Func<T1,T2,T3,T4,JoinInfoEntity>> expression,Builder builder,DBDrive dBDrive)
        {
            Type[] types=ReflectExtension.GetType<T1,T2,T3,T4>();
            Init(builder,dBDrive,types);
            visitor.VisitJoin(expression);
        }

        public IQueryResult<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression)where TResult : class
        {
            visitor.VisitMap(expression);
            IQueryResult<TResult> query = new QueryResult<TResult>(visitor,builder,DbDrive);
            return query;
        }

        public ISimpleQueryable<T1, T2, T3, T4> Where(Expression<Func<T1,T2,T3,T4,bool>>expression)
        {
            visitor.VisitorCondition(expression);
            return this;
        }
    }
}