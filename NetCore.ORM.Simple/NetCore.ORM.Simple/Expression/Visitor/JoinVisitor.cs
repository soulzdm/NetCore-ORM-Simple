﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NetCore.ORM.Simple.Common;
using NetCore.ORM.Simple.Entity;

/*********************************************************
 * 命名空间 NetCore.ORM.Simple.Visitor
 * 接口名称 JoinVisitor
 * 开发人员：-nhy
 * 创建时间：2022/9/19 14:11:42
 * 描述说明：解析多表查询连接部分（Left Join,Innor Join）
 * 更改历史：
 * 
 * *******************************************************/
namespace NetCore.ORM.Simple.Visitor
{
    public class JoinVisitor : ExpressionVisitor, IExpressionVisitor
    {
        /// <summary>
        /// 表结合
        /// all tables
        /// </summary>

        private TableEntity Table;

        private Dictionary<string, int> currentTables;
        /// <summary>
        /// 表的连接信息
        /// for table join info
        /// </summary>

        private Dictionary<string,JoinTableEntity> JoinTables;
        /// <summary>
        /// 正在解析连接条件
        /// </summary>
        private JoinTableEntity CurrentJoinTable;
        /// <summary>
        /// 当前解析的单个等式
        /// current single equtaion
        /// </summary>
        private TreeConditionEntity currentTree;
        /// <summary>
        /// true-表示解析完成一个二元条件 false-表示正在解析当中
        /// current single equtaion Is or no final
        /// </summary>
        private bool IsComplete;

        public JoinVisitor(TableEntity table,Dictionary<string,JoinTableEntity> joinInfos)
        {
            if (Check.IsNull(table))
            {
                throw new ArgumentException("not table names!");
            }
            Table = table;
            currentTables = new Dictionary<string, int>();
            JoinTables = joinInfos;
            JoinTables.Add(Table.TableNames[0],new JoinTableEntity() { DisplayName = Table.TableNames[0],TableType = eTableType.Master });
            IsComplete = true;
        }


        public string GetValue()
        {
            StringBuilder value = new StringBuilder();
            //foreach (var jTable in JoinTables)
            //{
            //    if (jTable.Value.TableType.Equals(eTableType.Master))
            //    {
            //        continue;
            //    }
            //    int length = jTable.Value.QValue.Count;
            //    value.Append(jTable.Value.JoinType);
            //    value.Append($" {jTable.Key} ON ");
            //    for (int i = 0; i < length; i++)
            //    {
            //        value.Append($"{jTable.Value.QValue.Dequeue()}");
            //    }
            //}
            //Console.WriteLine(value);
            return value.ToString();
        }
        /// <summary>
        /// 返回解析信息-不生成sql语句
        /// </summary>
        /// <returns></returns>
        public List<JoinTableEntity> GetJoinInfos()
        {
            if (Check.IsNull(JoinTables))
            {
                return null;
            }
            return JoinTables.Values.ToList();
        }
        /// <summary>
        /// 修改表达式树的形式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression Modify(Expression expression)
        {
            currentTables.Clear();
            foreach (ParameterExpression item in ((dynamic)expression).Parameters)
            {
                currentTables.Add(item.Name, currentTables.Count);
            }
            Visit(expression);
            return expression;
        }

        /// <summary>
        /// 表达式树的二元操作
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {

            if (currentTables.Count() > SimpleConst.minTableCount)
            {
                #region
                IsComplete = false;
                switch (node.NodeType)
                {
                    case ExpressionType.AndAlso:
                        SingleLogicBinary(node, (queue) =>
                        {
                            CurrentJoinTable.Conditions.Add(
                                new ConditionEntity(eConditionType.Sign)
                                {
                                    SignType = eSignType.And
                                });
                        });
                        break;
                    case ExpressionType.Call:
                        break;
                    case ExpressionType.GreaterThan:
                        SingleBinary(node, (queue) => { 
                            currentTree.RelationCondition = new ConditionEntity(eConditionType.Sign) 
                            { 
                                SignType= eSignType.GrantThan
                            };
                        });
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        SingleBinary(node, (queue) => { 
                            currentTree.RelationCondition = new ConditionEntity(eConditionType.Sign)
                            {
                                SignType = eSignType.GreatThanOrEqual
                            };
                        });
                        break;
                    case ExpressionType.LessThan:
                        SingleBinary(node, (queue) => { 
                            currentTree.RelationCondition = new ConditionEntity(eConditionType.Sign)
                            {
                                SignType = eSignType.LessThan
                            };
                        });
                        break;
                    case ExpressionType.LessThanOrEqual:
                        SingleBinary(node, (queue) => { 
                            currentTree.RelationCondition = new ConditionEntity(eConditionType.Sign)
                            {
                                SignType = eSignType.LessThanOrEqual
                            };
                        });
                        break;
                    case ExpressionType.Equal:
                        SingleBinary(node, (queue) => { 
                            currentTree.RelationCondition = new ConditionEntity(eConditionType.Sign)
                            {
                                SignType = eSignType.Equal
                            };
                        });
                        break;
                    case ExpressionType.NotEqual:
                        SingleBinary(node, (queue) => { 
                            currentTree.RelationCondition = new ConditionEntity(eConditionType.Sign)
                            {
                                SignType = eSignType.NotEqual
                            };
                        });
                        break;
                    case ExpressionType.OrElse:
                        SingleLogicBinary(node, (queue) =>
                        {
                            CurrentJoinTable.Conditions.Add(
                                    new ConditionEntity(eConditionType.Sign)
                                    {
                                        SignType = eSignType.Or
                                    });
                        });
                        break;
                    default:
                        break;
                }
                #endregion
            }
            return node;
        }

        /// <summary>
        /// 表达式树的常量操作
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (currentTables.Count() > SimpleConst.minTableCount)
            {
                if (IsComplete)
                {
                    
                    CurrentJoinTable = new JoinTableEntity() { TableType = eTableType.Slave };
                    currentTree = new TreeConditionEntity();
                    // JoinTables.Add(CurrentJoinTable);
                    switch (node.Value)
                    {
                        case eJoinType.Inner:
                            CurrentJoinTable.JoinType = eJoinType.Inner;
                            break;
                        case eJoinType.Left:
                            CurrentJoinTable.JoinType = eJoinType.Left;
                            break;
                        case eJoinType.Right:
                            CurrentJoinTable.JoinType = eJoinType.Right;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    currentTree.RightCondition = new ConditionEntity(eConditionType.Constant);
                    currentTree.RightCondition.DisplayName= $"{node.Value}";
                }

            }
            return node;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            base.VisitMethodCall(node);
            Console.WriteLine(node.Method.Name);
            currentTree.RelationCondition = new ConditionEntity(eConditionType.Method);
            currentTree.RelationCondition.DisplayName = node.Method.Name;
            IsComplete = true;
            CurrentJoinTable.TreeConditions.Add(currentTree);
            return node;
        }

      
        /// <summary>
        /// 用于解析值
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            base.VisitMemberBinding(node);
            return node;
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!currentTables.ContainsKey(node.Name))
            {
                currentTables.Add(node.Name, currentTables.Count());
            }
            return base.VisitParameter(node);
        }
        protected override Expression VisitUnary(UnaryExpression node)
        {
            Console.WriteLine(node.ToString());
            return base.VisitUnary(node);
        }
        protected override Expression VisitListInit(ListInitExpression node)
        {
            return base.VisitListInit(node);
        }

        

        protected override Expression VisitMember(MemberExpression node)
        {
            base.VisitMember(node);
            if (currentTables.ContainsKey(node.Expression.ToString()))
            {
                currentTree.LeftCondition = new ConditionEntity(eConditionType.ColumnName);
                currentTree.LeftCondition.DisplayName = $"{Table.TableNames[currentTables[node.Expression.ToString()]]}.{ node.Member.Name}";
                currentTree.LeftCondition.PropertyType = node.Type;
            }
            return node;
        }
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            Console.WriteLine(node.ToString());
            return base.VisitMemberInit(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {

            if (currentTables.Count > SimpleConst.minTableCount)
            {
                base.VisitNew(node);
            }
            return node;
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            base.VisitNewArray(node);
            return node;
        }
    
        /// <summary>
        /// 条件表达式大于小于等于
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        private void SingleBinary(BinaryExpression node, Action<Queue<string>> action)
        {
            
            if (node.Left is ConstantExpression leftConstant)
            {
                currentTree.LeftCondition = new ConditionEntity(eConditionType.Constant);
                currentTree.LeftCondition.DisplayName = GetConstantValue(leftConstant);
            }
            else if (node.Left is MemberExpression leftMember)
            {
                currentTree.LeftCondition = new ConditionEntity(eConditionType.ColumnName);
                currentTree.LeftCondition.DisplayName = GetMemberValue(leftMember);
                currentTree.LeftCondition.PropertyType = leftMember.Type;

            }
            if (!Check.IsNull(action))
            {
                action.Invoke(null);
            }
            if (node.Right is ConstantExpression rightConstant)
            {
                currentTree.RightCondition = new ConditionEntity(eConditionType.Constant);
                currentTree.RightCondition.DisplayName = GetConstantValue(rightConstant);

            }
            else if (node.Right is MemberExpression rightMember)
            {
                currentTree.RightCondition = new ConditionEntity(eConditionType.ColumnName);
                currentTree.RightCondition.DisplayName = GetMemberValue(rightMember);
                currentTree.LeftCondition.PropertyType = rightMember.Type;
            }
            CurrentJoinTable.TreeConditions.Add(currentTree);
            IsComplete = true;

        }

        /// <summary>
        /// 或与非
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        private void SingleLogicBinary(BinaryExpression node, Action<Queue<string>> action)
        {
            if (IsComplete)
            {
               
                currentTree = new TreeConditionEntity();
                IsComplete = false;
            }
            currentTree.LeftBracket.Add(eSignType.LeftBracket);
            base.Visit(node.Left);
            currentTree.RightBracket.Add(eSignType.RightBracket);
            if (!Check.IsNull(action))
            {
                action.Invoke(null);
            }
            if (IsComplete)
            {
                currentTree = new TreeConditionEntity();
                IsComplete = false;
            }
            currentTree.LeftBracket.Add(eSignType.LeftBracket);
            base.Visit(node.Right);
            currentTree.RightBracket.Add(eSignType.RightBracket);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private string GetMemberValue(MemberExpression member)
        {
            string mName = string.Empty;
            if (!Check.IsNull(member))
            {

                if (Table.TableNames.Length > SimpleConst.minTableCount)
                {
                    if (currentTables.ContainsKey(member.Expression.ToString()))
                    {
                        if (!JoinTables.ContainsKey(Table.TableNames[currentTables[member.Expression.ToString()]]))
                        {
                            CreateJoinTable(member);
                        }
                        mName = $"{Table.TableNames[currentTables[member.Expression.ToString()]]}.{member.Member.Name}";

                    }

                }
            }
            return mName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="constant"></param>
        /// <returns></returns>

        private string GetConstantValue(ConstantExpression constant)
        {
            string mName = string.Empty;
            if (!Check.IsNull(constant))
            {
                mName = constant.Value.ToString();
            }
            return mName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>

        private void CreateJoinTable(MemberExpression member)
        {
            if (!Check.IsNull(member))
            {
                string mName = string.Empty;
                if (Table.TableNames.Length > SimpleConst.minTableCount)
                {
                    if (currentTables.ContainsKey(member.Expression.ToString()))
                    {
                        if (!JoinTables.ContainsKey(Table.TableNames[currentTables[member.Expression.ToString()]]))
                        {
                            if (Check.IsNull(CurrentJoinTable))
                            {
                                CurrentJoinTable = new JoinTableEntity();

                            }
                            Dictionary<string, string> dic = new Dictionary<string, string>();

                            CurrentJoinTable.AsName = Table.TableNames[currentTables[member.Expression.ToString()]];
                            CurrentJoinTable.DisplayName = Table.DicTable[CurrentJoinTable.AsName].DisplayNmae;
                            JoinTables.Add(CurrentJoinTable.DisplayName, CurrentJoinTable);
                        }
                    }

                }
            }
        }
    }
}
