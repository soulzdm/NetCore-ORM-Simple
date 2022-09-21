﻿using NetCore.ORM.Simple.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

/*********************************************************
 * 命名空间 NetCore.ORM.Simple.Queryable
 * 接口名称 SimpleQueryable
 * 开发人员：-nhy
 * 创建时间：2022/9/21 10:25:31
 * 描述说明：
 * 更改历史：
 * 
 * *******************************************************/
namespace NetCore.ORM.Simple.Queryable
{
    public class SimpleQueryable<T>:QueryResult<T>,ISimpleQueryable<T>
    {
        public SimpleQueryable(eDBType DbType, params string[] tableNames) : base(DbType, tableNames)
        {
          
        }

        public IQueryResult<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            mapVisitor.Modify(expression);
            IQueryResult<TResult> query = new QueryResult<TResult>(mapVisitor,joinVisitor,conditionVisitor,DBType);
            return query;
        }

        ISimpleQueryable<T> ISimpleQueryable<T>.SimpleQueryable()
        {
            return this;
        }
    }
}
