using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using Lia.LinqToCrm.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Lia.LinqToCrm.Provider
{
	internal sealed partial class QueryProvider
	{
		private static readonly string[] _supportedMethods = 
		{
			nameof(object.Equals),
			nameof(string.Contains),
			nameof(string.StartsWith),
			nameof(string.EndsWith)
		};
		private static readonly string[] _validMethods = _supportedMethods.Concat(new[]
		{
			"Compare",
			"Like",
			"GetValueOrDefault"
		}).ToArray();
		
		private static readonly string[] _validProperties = {"Id", "Value"};
		
		private static readonly Dictionary<ExpressionType, ConditionOperator> _conditionLookup = new()
		{
			{ExpressionType.Equal, ConditionOperator.Equal},
			{ExpressionType.GreaterThan, ConditionOperator.GreaterThan},
			{ExpressionType.GreaterThanOrEqual, ConditionOperator.GreaterEqual},
			{ExpressionType.LessThan, ConditionOperator.LessThan},
			{ExpressionType.LessThanOrEqual, ConditionOperator.LessEqual},
			{ExpressionType.NotEqual, ConditionOperator.NotEqual}
		};
		private static readonly Dictionary<ConditionOperator, ConditionOperator> _operatorNegationLookup = new()
		{
			{ConditionOperator.Equal, ConditionOperator.NotEqual},
			{ConditionOperator.NotEqual, ConditionOperator.Equal},
			{ConditionOperator.GreaterThan, ConditionOperator.LessEqual},
			{ConditionOperator.GreaterEqual, ConditionOperator.LessThan},
			{ConditionOperator.LessThan, ConditionOperator.GreaterEqual},
			{ConditionOperator.LessEqual, ConditionOperator.GreaterThan},
			{ConditionOperator.Like, ConditionOperator.NotLike},
			{ConditionOperator.NotLike, ConditionOperator.Like},
			{ConditionOperator.Null, ConditionOperator.NotNull},
			{ConditionOperator.NotNull, ConditionOperator.Null}
		};
		private static readonly Dictionary<ExpressionType, LogicalOperator> _booleanLookup = new()
		{
			{ExpressionType.And, LogicalOperator.And},
			{ExpressionType.Or, LogicalOperator.Or},
			{ExpressionType.AndAlso, LogicalOperator.And},
			{ExpressionType.OrElse, LogicalOperator.Or}
		};
		private static readonly Dictionary<LogicalOperator, LogicalOperator> _logicalOperatorNegationLookup = new()
		{
			{LogicalOperator.And, LogicalOperator.Or},
			{LogicalOperator.Or, LogicalOperator.And}
		};

		private object[] BuildProjection(Projection projection, Entity entity, List<LinkLookup> linkLookups)
		{
			if (entity == null)
			{
				return null;
			}

			if (linkLookups.Count == 0)
			{
				return new object[]
				{
					entity
				};
			}

			var parameters = projection.Expression.Parameters;
			if (linkLookups.Count != 2 || parameters.Count != 2)
			{
				return parameters.Select(parameter => BuildProjection(null, projection.MethodName, parameter.Type, entity, linkLookups)).ToArray();
			}

			return new[]
			{
				linkLookups[1].Link.JoinOperator == JoinOperator.LeftOuter ? BuildProjection(null, projection.MethodName, parameters[0].Type, entity, linkLookups) : entity,
				BuildProjectionParameter(parameters[1].Type, entity, linkLookups[1])
			};
		}

		private object BuildProjection(string environment, string projectingMethodName, Type entityType, Entity entity, List<LinkLookup> linkLookups)
		{
			if (IsEntity(entityType))
			{
				return BuildProjectionParameter(null, projectingMethodName, entityType, entity, linkLookups) ?? entity;
			}

			var constructors = entityType.GetConstructors();
			if (IsAnonymousType(entityType) && constructors.Length != 1)
			{
				throw new InvalidOperationException("The result selector of the 'Join' operation is not returning a valid anonymous type.");
			}

			var ci = constructors.First();
			var parameters = ci.GetParameters();
			if (parameters.Length != 2)
			{
				throw new InvalidOperationException("The result selector of the 'Join' operation must return an anonymous type of two properties.");
			}

			var parameterInfo1 = parameters[0];
			var parameterInfo2 = parameters[1];
			if (IsEntity(parameterInfo1.ParameterType) && IsEntity(parameterInfo2.ParameterType))
			{
				var obj1 = BuildProjectionParameter(parameterInfo1, environment, projectingMethodName, parameterInfo1.ParameterType, entity, linkLookups);
				var obj2 = BuildProjectionParameter(parameterInfo2, environment, projectingMethodName, parameterInfo2.ParameterType, entity, linkLookups);
				return ConstructorInvoke(ci, new[] {obj1, obj2});
			}
			if (IsEntity(parameterInfo2.ParameterType))
			{
				var obj1 = BuildProjectionParameter(parameterInfo1, environment, projectingMethodName, entity, linkLookups);
				var obj2 = BuildProjectionParameter(parameterInfo2, environment, projectingMethodName, parameterInfo2.ParameterType, entity, linkLookups);
				return ConstructorInvoke(ci, new[] {obj1, obj2});
			}
			if (IsEntity(parameterInfo1.ParameterType))
			{
				var obj1 = BuildProjectionParameter(parameterInfo1, environment, projectingMethodName, parameterInfo1.ParameterType, entity, linkLookups);
				var obj2 = BuildProjectionParameter(parameterInfo2, environment, projectingMethodName, entity, linkLookups);
				return ConstructorInvoke(ci, new[] {obj1, obj2});
			}
			throw new InvalidOperationException($"Invalid left '{parameterInfo1.ParameterType.Name}' and right '{parameterInfo2.ParameterType.Name}' parameters.");
		}

		private object BuildProjectionParameter(ParameterInfo parameter, string environment, string projectingMethodName, Entity entity, List<LinkLookup> linkLookups)
		{
			return parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ? null : BuildProjection(GetEnvironment(parameter, environment), projectingMethodName, parameter.ParameterType, entity, linkLookups);
		}

		private object BuildProjectionParameter(ParameterInfo pi, string environment, string projectingMethodName, Type entityType, Entity entity, List<LinkLookup> linkLookups)
		{
			return BuildProjectionParameter(GetEnvironment(pi, environment), projectingMethodName, entityType, entity, linkLookups);
		}

		private object BuildProjectionParameter(string environment, string projectingMethodName, Type entityType, Entity entity, List<LinkLookup> linkLookups)
		{
			var link = projectingMethodName == "SelectMany" ? linkLookups.SingleOrDefault(l => l.SelectManyEnvironment != null && l.SelectManyEnvironment == environment) : linkLookups.SingleOrDefault(l => l.Environment == environment);
			if (link != null)
			{
				return BuildProjectionParameter(entityType, entity, link);
			}

			throw new InvalidOperationException("The projection property does not match an existing entity binding.");
		}

		private object BuildProjectionParameter(Type entityType, Entity entity, LinkLookup link)
		{
			if (link.Link == null)
			{
				return entity;
			}

			var entity1 = entityType == typeof (Entity) ? new Entity(link.Link.LinkToEntityName) : (Entity)Activator.CreateInstance(entityType);
			var entityAlias = $"{link.Link.EntityAlias}.";
			var aliasIndex = entityAlias.Length;

			foreach (var attribute in entity.Attributes)
			{
				if (attribute.Value is AliasedValue value && attribute.Key.StartsWith(entityAlias, StringComparison.Ordinal))
				{
					entity1.Attributes.Add(attribute.Key.Substring(aliasIndex), value);
				}
			}

			return entity1;
		}

		private (QueryExpression query, bool throwIfSequenceIsEmpty, bool throwIfSequenceNotSingle, Projection projection, List<LinkLookup> linkLookups) GetQueryExpression(List<MethodCallExpression> expressions, string entityName)
		{
			bool throwIfSequenceIsEmpty = false;
			bool throwIfSequenceNotSingle = false;
			Projection projection = null;
			List<LinkLookup> linkLookups = new List<LinkLookup>();
			var skip = new int?();
			var take = new int?();
			var query = new QueryExpression();
			var isFirstJoin = expressions.Count > 0 && expressions[0].Method.Name.In(nameof(Queryable.Join), nameof(Queryable.GroupJoin));
			for (var i = 0; i < expressions.Count; ++i)
			{
				var mce = expressions[i];
				var methodName = mce.Method.Name;

				switch (methodName)
				{
					default:
					{
						throw new NotSupportedException($"The method '{methodName}' is not supported.");
					}
					case nameof(QueryableNoLock.NoLock):
					{
						query.NoLock = true;
						break;
					}
					case nameof(Queryable.Count):
					{
						query.PageInfo.ReturnTotalRecordCount = true;
						throw new NotImplementedException();
						break;
					}
					case nameof(Queryable.FirstOrDefault):
					{
						take = 1;
						var methodData = GetMethodCallBody(mce);
						if (methodData.Body == null)
						{
							break;
						}
						TranslateWhere(query, methodData.parameterName, methodData.Body, linkLookups);
						break;
					}
					case nameof(Queryable.SingleOrDefault):
					{
						take = 2;
						throwIfSequenceNotSingle = true;

						var methodData = GetMethodCallBody(mce);
						if (methodData.Body == null)
						{
							break;
						}
						TranslateWhere(query, methodData.parameterName, methodData.Body, linkLookups);
						break;
					}
					case nameof(Queryable.First):
					{
						take = 1;
						throwIfSequenceIsEmpty = true;
						var methodData = GetMethodCallBody(mce);
						if (methodData.Body == null)
						{
							break;
						}
						TranslateWhere(query, methodData.parameterName, methodData.Body, linkLookups);
						break;
					}
					case nameof(Queryable.Single):
					{
						throwIfSequenceIsEmpty = true;
						take = 2;
						throwIfSequenceNotSingle = true;
						var methodData = GetMethodCallBody(mce);
						if (methodData.Body == null)
						{
							break;
						}
						TranslateWhere(query, methodData.parameterName, methodData.Body, linkLookups);
						break;
					}
					case nameof(Queryable.Where):
					{
						var methodData = GetMethodCallBody(mce);
						TranslateWhere(query, methodData.parameterName, methodData.Body, linkLookups);
						break;
					}
					case nameof(Queryable.OrderBy):
					case nameof(Queryable.ThenBy):
					{	
						var methodData = GetMethodCallBody(mce);
						TranslateOrderBy(query, methodData.Body, methodData.parameterName, OrderType.Ascending, linkLookups);
						break;
					}
					case nameof(Queryable.OrderByDescending):
					case nameof(Queryable.ThenByDescending):
					{	
						var methodData = GetMethodCallBody(mce);
						TranslateOrderBy(query, methodData.Body, methodData.parameterName, OrderType.Descending, linkLookups);
						break;
					}
					//case nameof(Queryable.Join):
					//{
					//	var data = TranslateJoin(qe, expressions, ref i);
					//	projection = data.projection;
					//	linkLookups = data.linkLookups;
					//	break;
					//}
					//case nameof(Queryable.GroupJoin):
					//{
					//	var data = TranslateGroupJoin(qe, expressions, ref i);
					//	projection = data.projection;
					//	linkLookups = data.linkLookups;
					//	break;
					//}
					case nameof(Queryable.Select):
					{
						if (!isFirstJoin)
						{
							linkLookups.Clear();
						}

						TranslateEntityName(query, mce);
						var operand = ((UnaryExpression) mce.Arguments[1]).Operand as LambdaExpression;
						projection = new Projection(methodName, operand);

						break;
					}
					case nameof(Enumerable.Distinct):
						query.Distinct = true;
						break;
				}
				TranslateEntityName(query, mce);
			}

			if (projection != null)
			{
				AddColumnToColumntSet(query, projection.Expression, linkLookups);
			}

			if (expressions.Count == 0)
			{
				query.EntityName = entityName;
			}

			BuildPagingInfo(query, skip, take);

			FixColumnSet(query);
			return (query, throwIfSequenceIsEmpty, throwIfSequenceNotSingle, projection, linkLookups);
		}

		private static string GetEnvironment(ParameterInfo pi, string environment)
		{
			return environment == null ? pi.Name : $"{environment}.{pi.Name}";
		}

		private void BuildPagingInfo(QueryExpression qe, int? skip, int? take)
		{
			if (!skip.HasValue && !take.HasValue)
			{
				return;
			}

			if (qe.PageInfo == null)
			{
				qe.PageInfo = new PagingInfo();
			}

			if (skip > 0)
			{
				qe.PageInfo.PageNumber = skip.Value;
			}

			if (take > 0)
			{
				qe.PageInfo.Count = take.Value;
			}
		}

		private void FixColumnSet(QueryExpression qe)
		{
			qe.ColumnSet = qe.ColumnSet == null || qe.ColumnSet.Columns.Count == 0 ? new ColumnSet(true) : qe.ColumnSet;
		}

		//private (Projection projection, List<LinkLookup> linkLookups) TranslateJoin(QueryExpression qe, List<MethodCallExpression> methods, ref int i)
		//{
		//	var num = 0;
		//	var source = new List<LinkData>();
		//	Projection projection;
		//	do
		//	{
		//		var method = new JoinData(methods[i]);
		//		projection = new Projection(method.MethodName, method.ResultSelector.Operand as LambdaExpression);
		//		string str;
		//		string environment;
		//		if (i < methods.Count - 1)
		//		{
		//			environment = GetEnvironmentForParameter(projection.Expression, 0);
		//			str = GetEnvironmentForParameter(projection.Expression, 1);
		//		}
		//		else
		//		{
		//			environment = str = null;
		//		}

		//		var operand1 = method.OuterKeySelector.Operand as LambdaExpression;
		//		var name1 = operand1.Parameters[0].Name;
		//		var entityExpression = FindValidEntityExpression(operand1.Body, nameof(Queryable.Join));
		//		var attributeName1 = TranslateExpressionToAttributeName(entityExpression);
				
		//		var operand2 = method.InnerKeySelector.Operand as LambdaExpression;
		//		var name2 = operand2.Parameters[0].Name;
		//		var entityExpression2 = FindValidEntityExpression(operand2.Body, nameof(Queryable.Join));
		//		var attributeName2 = TranslateExpressionToAttributeName(entityExpression2);
				
		//		var entityLogicalName = (method.Inner.Value as ICrmEntity).EntityLogicalName;
				
		//		LinkEntity linkEntity;
		//		if (source.Count == 0)
		//		{
		//			qe.EntityName = (method.Outer.Value as ICrmEntity).EntityLogicalName;

		//			source.Add(new LinkData(name1, null, environment, environment));

		//			linkEntity = qe.AddLink(entityLogicalName, attributeName1, attributeName2, JoinOperator.Inner);
		//		}
		//		else
		//		{
		//			if (environment != null)
		//			{
		//				foreach (var linkData in source)
		//				{
		//					linkData.Environment = environment + "." + linkData.Environment;
		//				}
		//			}

		//			var parentMember = GetUnderlyingMemberExpression(entityExpression).Member.Name;
					
		//			var linkEntity2 = source.Single(l => l.Item1 == parentMember).Link;
					
		//			linkEntity = linkEntity2 == null 
		//				? qe.AddLink(entityLogicalName, attributeName1, attributeName2, JoinOperator.Inner) 
		//				: linkEntity2.AddLink(entityLogicalName, attributeName1, attributeName2, JoinOperator.Inner);
		//		}
		//		linkEntity.EntityAlias = $"{name2}_{num++}";
		//		source.Add(new LinkData(name2, linkEntity, str, str));
		//		++i;
		//	}
		//	while (i < methods.Count && methods[i].Method.Name == nameof(Queryable.Join));
			
		//	--i;
		//	var linkLookups = source.Select(l => new LinkLookup(l.ParameterName, l.Environment, l.Link)).ToList();
		//	return (projection, linkLookups);
		//}

		//private (Projection projection, List<LinkLookup> linkLookups) TranslateGroupJoin(QueryExpression qe, List<MethodCallExpression> methods, ref int i)
		//{
		//	var method1 = methods[i];
		//	var linkLookups1 = TranslateJoin(qe, methods, ref i).linkLookups;
		//	++i;
		//	if (i + 1 > methods.Count || !IsValidLeftOuterSelectManyExpression(methods[i]))
		//	{
		//		throw new NotSupportedException("The 'GroupJoin' operation must be followed by a 'SelectMany' operation where the collection selector is invoking the 'DefaultIfEmpty' method.");
		//	}

		//	var method2 = methods[i];
		//	LambdaExpression expression;
		//	if (method2.Arguments.Count == 3)
		//	{
		//		expression = (method2.Arguments[2] as UnaryExpression).Operand as LambdaExpression;
		//	}
		//	else
		//	{
		//		var parameter1 = ((method2.Arguments[1] as UnaryExpression).Operand as LambdaExpression).Parameters[0];
		//		var parameter2 = ((method1.Arguments[3] as UnaryExpression).Operand as LambdaExpression).Parameters[0];
		//		expression = Expression.Lambda(parameter2, parameter1, parameter2);
		//	}
		//	var projection = new Projection(method2.Method.Name, expression);
		//	var environmentForParameter1 = GetEnvironmentForParameter(projection.Expression, 0);
		//	var environmentForParameter2 = GetEnvironmentForParameter(projection.Expression, 1);
			
		//	var firstJoinLink = linkLookups1[0];
			
		//	var environment1 = environmentForParameter1 == null ? firstJoinLink.Environment : $"{environmentForParameter1}.{firstJoinLink.Environment}";
		//	var linkLookup = new LinkLookup(firstJoinLink.ParameterName, environment1, firstJoinLink.Link, firstJoinLink.Environment);
			
		//	var linkLookupList1 = new List<LinkLookup>();
			
		//	linkLookupList1.Add(linkLookup);
		//	linkLookupList1.Add(new LinkLookup(linkLookups1[1].ParameterName, environmentForParameter2, linkLookups1[1].Link));

		//	linkLookups1[1].Link.JoinOperator = JoinOperator.LeftOuter;

		//	return (projection, linkLookupList1);
		//}

		//private bool IsValidLeftOuterSelectManyExpression(MethodCallExpression mce)
		//{
		//	return 
		//		mce.Method.Name == nameof(Queryable.SelectMany) && 
		//		mce.Arguments[1] is UnaryExpression unaryExpression1 && 
		//		unaryExpression1.Operand is LambdaExpression operand1 && 
		//		operand1.Body is MethodCallExpression body && 
		//		body.Method.Name == nameof(Queryable.DefaultIfEmpty) && 
		//		body.Arguments.Count == 1 && 
		//		(
		//			mce.Arguments.Count == 2 || 
		//			mce.Arguments.Count == 3 && 
		//			mce.Arguments[2] is UnaryExpression unaryExpression2 && 
		//			unaryExpression2.Operand is LambdaExpression operand2 && 
		//			operand2.Parameters.Count == 2
		//		);
		//}

		//private string GetEnvironmentForParameter(LambdaExpression projection, int index)
		//{
		//	if (projection.Body is NewExpression body)
		//	{
		//		var parameter = projection.Parameters[index];
		//		var arguments = body.Arguments;

		//		for (int i = 0; i < arguments.Count; i++)
		//		{
		//			var argument = arguments[i];
		//			if (argument == parameter)
		//			{
		//				return body.Members[i].Name;
		//			}
		//		}
		//	}

		//	return null;
		//}

		private ConditionOperator NegateOperator(ConditionOperator op)
		{
			return _operatorNegationLookup[op];
		}

		private LogicalOperator NegateOperator(LogicalOperator op)
		{
			return _logicalOperatorNegationLookup[op];
		}

		private void TranslateWhere(QueryExpression qe, string parameterName, Expression body, List<LinkLookup> linkLookups)
		{
			var filter = GetFilter(qe);

			TranslateWhereBoolean(parameterName, body, null, filter, linkLookups, null, false);
		}

		private void TranslateWhere(string parameterName, BinaryExpression be, FilterExpressionWrapper parentFilter, Func<Expression, FilterExpressionWrapper> getFilter, List<LinkLookup> linkLookups, bool negate)
		{
			if (_booleanLookup.ContainsKey(be.NodeType))
			{
				var entityExr = be.Left.FindPreorder(IsEntityExpression);
				parentFilter = GetFilter(entityExr, parentFilter, getFilter);
				var filter = parentFilter.Filter.AddFilter(_booleanLookup[be.NodeType]);
				var parentFilter1 = new FilterExpressionWrapper(filter, parentFilter.Alias);
				parentFilter1.Filter.FilterOperator = negate ? NegateOperator(parentFilter1.Filter.FilterOperator) : parentFilter1.Filter.FilterOperator;
				TranslateWhereBoolean(parameterName, be.Left, parentFilter1, getFilter, linkLookups, be, negate);
				TranslateWhereBoolean(parameterName, be.Right, parentFilter1, getFilter, linkLookups, be, negate);
			}
			else
			{
				if (!_conditionLookup.ContainsKey(be.NodeType))
				{
					return;
				}

				var negate1 = negate;
				if (TranslateWhere(be.Left, ref negate1) is MethodCallExpression methodCallExpression)
				{
					if (methodCallExpression.Method.Name == "Compare" || _supportedMethods.Contains(methodCallExpression.Method.Name))
					{
						TranslateWhereBoolean(parameterName, methodCallExpression, parentFilter, getFilter, linkLookups, be, negate1);
					}
					else
					{
						TranslateWhereCondition(be, parentFilter, getFilter, GetLinkLookup(parameterName, linkLookups), negate);
					}
				}
				else
				{
					TranslateWhereCondition(be, parentFilter, getFilter, GetLinkLookup(parameterName, linkLookups), negate);
				}
			}
		}

		private Expression TranslateWhere(Expression exp, ref bool negate)
		{
			if (exp is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Not)
			{
				negate = !negate;
				return TranslateWhere(unaryExpression.Operand, ref negate);
			}

			return exp;
		}

		private void TranslateWhereBoolean(string parameterName, Expression body, FilterExpressionWrapper parentFilter, Func<Expression, FilterExpressionWrapper> getFilter, List<LinkLookup> linkLookups, BinaryExpression parent, bool negate)
		{
			switch (body)
			{
				case MemberExpression me:
				{
					var value = TranslateExpressionToValue(me);
					if (value is bool boolValue)
					{
						if (boolValue)
						{
							return;
						}
						else
						{
							throw new NotImplementedException();
						}
					}
					break;
				}
				case ConstantExpression ce:
				{
					if (ce.Value is bool boolValue)
					{
						if (boolValue)
						{
							return;
						}
						else
						{
							throw new NotImplementedException();
						}
					}
					break;
				}
				case BinaryExpression be:
				{	
					if (be.Left is ConstantExpression left)
					{
						if (be.NodeType == ExpressionType.AndAlso && Equals(left.Value, true))
						{
							TranslateWhereBoolean(parameterName, be.Right, parentFilter, getFilter, linkLookups, parent, negate);
						}
						else if (be.NodeType == ExpressionType.OrElse && Equals(left.Value, false))
						{
							TranslateWhereBoolean(parameterName, be.Right, parentFilter, getFilter, linkLookups, parent, negate);
						}
						//else if (be.NodeType == ExpressionType.Equal)
						//{
						//	throw new NotImplementedException();
						//}
						//else if (be.NodeType == ExpressionType.NotEqual)
						//{
						//	throw new NotImplementedException();
						//}
						else
						{
							TranslateWhere(parameterName, be, parentFilter, getFilter, linkLookups, negate);
						}
					}
					else
					{
						TranslateWhere(parameterName, be, parentFilter, getFilter, linkLookups, negate);
					}
					break;
				}
				case MethodCallExpression mce:
				{
					var linkLookup = GetLinkLookup(parameterName, linkLookups);
					TranslateWhereMethodCall(mce, parentFilter, getFilter, linkLookup, parent, negate);
					break;
				}
				case UnaryExpression unaryExpression:
				{
					if (unaryExpression.NodeType == ExpressionType.Convert)
					{
						TranslateWhereBoolean(parameterName, unaryExpression.Operand, parentFilter, getFilter, linkLookups, parent, negate);
					}
					else
					{
						if (unaryExpression.NodeType != ExpressionType.Not)
						{
							return;
						}

						TranslateWhereBoolean(parameterName, unaryExpression.Operand, parentFilter, getFilter, linkLookups, parent, !negate);
					}
					break;
				}
				default:
				{
					if (!(body.Type == typeof(bool)))
					{
						return;
					}

					TranslateWhere(parameterName, Expression.Equal(body, Expression.Constant(true)), parentFilter, getFilter, linkLookups, negate);
					break;
				}
			}
		}

		private string GetLinkEntityAlias(Expression expression, Func<Expression, LinkLookup> getLinkLookup)
		{
			return getLinkLookup(expression)?.Link?.EntityAlias;
		}

		private void TranslateWhereCondition(BinaryExpression be, FilterExpressionWrapper parentFilter, Func<Expression, FilterExpressionWrapper> getFilter, Func<Expression, LinkLookup> getLinkLookup, bool negate)
		{
			var entityExpression = FindValidEntityExpression(be.Left, nameof(Queryable.Where));
			object conditionValue;
			if (entityExpression == null)
			{
				entityExpression = FindValidEntityExpression(be.Right, nameof(Queryable.Where));
				conditionValue = TranslateExpressionToConditionValue(be.Left);
			}
			else
			{
				conditionValue = TranslateExpressionToConditionValue(be.Right);
			}
			var attributeName = TranslateExpressionToAttributeName(entityExpression);
			var linkEntityAlias = GetLinkEntityAlias(entityExpression, getLinkLookup);
			ConditionExpression condition;
			if (conditionValue != null)
			{
				condition = new ConditionExpression(linkEntityAlias, attributeName, _conditionLookup[be.NodeType], conditionValue);
			}
			else if (be.NodeType == ExpressionType.Equal)
			{
				condition = new ConditionExpression(linkEntityAlias, attributeName, ConditionOperator.Null);
			}
			else if (be.NodeType == ExpressionType.NotEqual)
			{
				condition = new ConditionExpression(linkEntityAlias, attributeName, ConditionOperator.NotNull);
			}
			else
			{
				throw new NotSupportedException("Invalid 'where' condition.");
			}

			condition.Operator = negate ? NegateOperator(condition.Operator) : condition.Operator;
			var filter = GetFilter(entityExpression, parentFilter, getFilter);
			AddCondition(filter, condition, null);
		}

		private void TranslateWhereMethodCall(MethodCallExpression mce, FilterExpressionWrapper parentFilter, Func<Expression, FilterExpressionWrapper> getFilter, Func<Expression, LinkLookup> getLinkLookup, BinaryExpression parent, bool negate)
		{
			if (_supportedMethods.Contains(mce.Method.Name) && mce.Arguments.Count == 1)
			{
				var entityExpression = FindValidEntityExpression(mce.Object, nameof(Queryable.Where));
				var linkEntityAlias = GetLinkEntityAlias(entityExpression, getLinkLookup);
				var attributeName = TranslateExpressionToAttributeName(entityExpression);
				var conditionValue = TranslateExpressionToConditionValue(mce.Arguments[0]);
				if (parent != null)
				{
					if (parent.NodeType == ExpressionType.NotEqual)
					{
						negate = !negate;
					}

					if (parent.NodeType == ExpressionType.Equal || parent.NodeType == ExpressionType.NotEqual)
					{
						var value = TranslateExpressionToConditionValue(parent.Right);
						if (Equals(value, false))
						{
							negate = !negate;
						}
					}
				}
				var condition = TranslateConditionMethodExpression(mce, attributeName, conditionValue);
				condition.EntityName = linkEntityAlias;
				condition.Operator = negate ? NegateOperator(condition.Operator) : condition.Operator;
				var filter = GetFilter(entityExpression, parentFilter, getFilter);
				AddCondition(filter, condition, null);
			}
			else if (mce.Method.Name == "Compare" && mce.Arguments.Count == 2)
			{
				var entityExpression = FindValidEntityExpression(mce.Arguments[0], nameof(Queryable.Where));
				var linkEntityAlias = GetLinkEntityAlias(entityExpression, getLinkLookup);
				var attributeName = TranslateExpressionToAttributeName(entityExpression);
				var conditionValue = TranslateExpressionToConditionValue(mce.Arguments[1]);
				if (parent == null || !Equals(TranslateExpressionToConditionValue(parent.Right), 0) || !_conditionLookup.TryGetValue(parent.NodeType, out var conditionOperator))
				{
					return;
				}

				var condition = new ConditionExpression(linkEntityAlias, attributeName, conditionOperator, conditionValue);
				condition.Operator = negate ? NegateOperator(condition.Operator) : condition.Operator;
				var filter = GetFilter(entityExpression, parentFilter, getFilter);
				AddCondition(filter, condition, null);
			}
			else if (mce.Method.Name == "Like" && mce.Arguments.Count == 2)
			{
				var entityExpression = FindValidEntityExpression(mce.Arguments[0], nameof(Queryable.Where));
				var alias = GetLinkEntityAlias(entityExpression, getLinkLookup);
				var attributeNme = TranslateExpressionToAttributeName(entityExpression);
				var value = TranslateExpressionToConditionValue(mce.Arguments[1]);
				var condition = new ConditionExpression(alias, attributeNme, ConditionOperator.Like, value);
				condition.Operator = negate ? NegateOperator(condition.Operator) : condition.Operator;
				var filter = GetFilter(entityExpression, parentFilter, getFilter);
				AddCondition(filter, condition, null);
			}
			else
			{
				if (parent != null && !_booleanLookup.ContainsKey(parent.NodeType) || !(mce.Type.GetUnderlyingType() == typeof(bool)))
				{
					return;
				}

				var entityExpression = FindValidEntityExpression(mce, nameof(Queryable.Where));
				var alias = GetLinkEntityAlias(entityExpression, getLinkLookup);
				var attributeName = TranslateExpressionToAttributeName(entityExpression);
				var condition = new ConditionExpression(alias, attributeName, ConditionOperator.Equal, true);
				condition.Operator = negate ? NegateOperator(condition.Operator) : condition.Operator;
				var filter = GetFilter(entityExpression, parentFilter, getFilter);
				AddCondition(filter, condition, null);
			}
		}

		private ConditionExpression TranslateConditionMethodExpression(MethodCallExpression mce, string attributeName, object value)
		{
			ConditionExpression conditionExpression;
			switch (mce.Method.Name)
			{
				case nameof(object.Equals):
					conditionExpression = value == null 
						? new ConditionExpression(attributeName, ConditionOperator.Null) 
						: new ConditionExpression(attributeName, ConditionOperator.Equal, value);
					break;
				case nameof(string.Contains):
					conditionExpression = new ConditionExpression(attributeName, ConditionOperator.Like, "%" + value + "%");
					break;
				case nameof(string.StartsWith):
					conditionExpression = new ConditionExpression(attributeName, ConditionOperator.Like, value + "%");
					break;
				case nameof(string.EndsWith):
					conditionExpression = new ConditionExpression(attributeName, ConditionOperator.Like, "%" + value);
					break;
				default:
					throw new NotSupportedException($"The method '{mce.Method.Name}' is not supported.");
			}
			return conditionExpression;
		}

		private void AddCondition(FilterExpressionWrapper filter, ConditionExpression condition, string alias)
		{
			if (filter.Alias != alias)
			{
				throw new NotSupportedException("filter conditions of different entity types, in the same expression, are not supported");
			}

			filter.Filter.AddCondition(condition);
		}

		private FilterExpressionWrapper GetFilter(Expression entityExpression, FilterExpressionWrapper parentFilter, Func<Expression, FilterExpressionWrapper> getFilter)
		{
			return parentFilter ?? getFilter(entityExpression);
		}

		private Func<Expression, LinkLookup> GetLinkLookup(string parameterName, List<LinkLookup> linkLookups)
		{
			return exp =>
			{
				var expName = GetUnderlyingParameterExpressionName(exp);
				return linkLookups.SingleOrDefault(link =>
				{
					var str = $"{parameterName}.{link.Environment}";
					if (expName == str)
					{
						return true;
					}

					return expName.StartsWith(str) && expName[str.Length] == '.';
				});
			};
		}

		private Func<Expression, FilterExpressionWrapper> GetFilter(QueryExpression qe)
		{
			return exp => new FilterExpressionWrapper(qe.Criteria, null);
		}

		private void TranslateOrderBy(QueryExpression qe, Expression exp, string parameterName, OrderType orderType, List<LinkLookup> linkLookups)
		{
			if (IsEntityExpression(exp))
			{
				ValidateRootEntity(nameof(Queryable.OrderBy), exp, parameterName, linkLookups);
				
				var attributeName = TranslateExpressionToAttributeName(exp);
				qe.AddOrder(attributeName, orderType);
			}
			else
			{
				throw new NotSupportedException($"The '{nameof(Queryable.OrderBy)}' call must specify property names.");
			}
		}

		private void ValidateRootEntity(string operationName, Expression exp, string parameterName, List<LinkLookup> linkLookups)
		{
			if (linkLookups == null)
			{
				return;
			}

			var parameterExpressionName = GetUnderlyingParameterExpressionName(exp);
			var linkLookup = linkLookups.SingleOrDefault(l => l.Link == null);
			if (linkLookup == null)
			{
				return;
			}

			if ($"{parameterName}.{linkLookup.Environment}" == parameterExpressionName)
			{
				return;
			}

			throw new NotSupportedException($"The '{operationName}' expression is limited to invoking the '{linkLookup.ParameterName}' parameter.");
		}

		private void AddColumnToColumntSet(QueryExpression qe, LambdaExpression exp, List<LinkLookup> linkLookups)
		{
			var parameterName = exp.Parameters[0].Name;

			foreach (var column in TraverseSelect(exp.Body))
			{
				if (linkLookups != null)
				{
					var expName = column.ParameterName;

					var linkLookup = linkLookups.SingleOrDefault(l => $"{parameterName}.{l.Environment}" == expName);
					if (linkLookup != null)
					{
						if (linkLookup.Link != null)
						{
							AddColumnToColumnSet(column, linkLookup.Link.Columns);
							continue;
						}
					}
					else
					{
						if (exp.Parameters.Count > 1)
						{
							var name = exp.Parameters[1].Name;

							var linkLookup2 = column.ParameterName == name 
								? linkLookups.Last() 
								: column.ParameterName != parameterName || linkLookups.Count != 2 
									? null
									: linkLookups.First();

							if (linkLookup2?.Link != null)
							{
								AddColumnToColumnSet(column, linkLookup2.Link.Columns);
								continue;
							}
						}
					}
				}
				AddColumnToColumnSet(column, qe.ColumnSet);
			}
		}

		private void AddColumnToColumnSet(EntityColumn column, ColumnSet columnSet)
		{
			if (column.AllColumns)
			{
				return;
			}
			
			if (columnSet.Columns.Contains(column.Column))
			{
				return;
			}

			columnSet.AddColumn(column.Column);
		}

		private IEnumerable<EntityColumn> TraverseSelect(Expression exp)
		{
			var column = TranslateSelectColumn(exp);
			if (column != null)
			{
				if (column.AllColumns || column.Column != null)
				{
					yield return column;
				}
			}
			else
			{
				foreach (var child in exp.GetChildren())
				{
					foreach (var entityColumn in TraverseSelect(child))
					{
						yield return entityColumn;
					}
				}
			}
		}

		private EntityColumn TranslateSelectColumn(Expression exp)
		{
			if (exp is MemberExpression memberExpression)
			{
				if (memberExpression.Expression != null && IsEntity(memberExpression.Expression.Type))
				{
					var attributeName = TranslateExpressionToAttributeName(memberExpression);
					if (!string.IsNullOrEmpty(attributeName))
					{
						return new EntityColumn(GetUnderlyingParameterExpressionName(memberExpression), attributeName);
					}
				}
				else
				{
					if (IsEntity(memberExpression.Type))
					{
						return new EntityColumn(memberExpression.ToString(), true);
					}

					if (IsEnumerableEntity(memberExpression.Type))
					{
						throw new NotSupportedException($"The expression '{memberExpression}' is an invalid column projection expression. Entity collections cannot be selected.");
					}
				}
			}
			else
			{
				if (exp is MethodCallExpression methodCallExpression)
				{
					if (methodCallExpression.Object != null && IsEntity(methodCallExpression.Object.Type))
					{
						var attributeName = TranslateExpressionToAttributeName(methodCallExpression);
						if (!string.IsNullOrEmpty(attributeName))
						{
							return new EntityColumn(GetUnderlyingParameterExpressionName(methodCallExpression), attributeName);
						}
					}
					else
					{
						if (IsEntity(methodCallExpression.Type))
						{
							return new EntityColumn(methodCallExpression.ToString(), true);
						}

						if (IsEnumerableEntity(methodCallExpression.Type))
						{
							throw new NotSupportedException($"The expression '{methodCallExpression}' is an invalid column projection expression. Entity collections cannot be selected.");
						}
					}
				}
				else
				{
					var pe = exp as ParameterExpression;
					return TranslateSelectColumn(pe);
				}
			}

			return null;
		}

		private EntityColumn TranslateSelectColumn(ParameterExpression pe)
		{
			if (pe != null)
			{
				if (IsEntity(pe.Type))
				{
					return new EntityColumn(pe.ToString(), true);
				}

				if (IsEnumerableEntity(pe.Type))
				{
					throw new NotSupportedException($"The expression '{pe}' is an invalid column projection expression. Entity collections cannot be selected.");
				}
			}

			return null;
		}

		private (string parameterName, Expression Body) GetMethodCallBody(MethodCallExpression mce)
		{
			if (mce.Arguments.Count <= 1)
			{
				return (null, null);
			}

			var firstArgument = (UnaryExpression)mce.Arguments[1];
			var operand = (LambdaExpression)firstArgument.Operand;
			return (operand.Parameters[0].Name, operand.Body);
		}

		private string TranslateExpressionToAttributeName(MethodCallExpression methodCallExpression)
		{
			var valueExpression = methodCallExpression.Method.IsStatic
				? methodCallExpression.Arguments[1]
				: methodCallExpression.Arguments[0];

			var attributeName = TranslateExpressionToValue(valueExpression);
			return (string)attributeName;
		}

		private string TranslateExpressionToAttributeName(MemberExpression memberExpression)
		{
			switch (memberExpression.Expression)
			{
				case MemberExpression expression:
				{
					var defaultCustomAttribute = expression.Member.GetFirstOrDefaultCustomAttribute<AttributeLogicalNameAttribute>();
					if (defaultCustomAttribute != null)
					{
						return defaultCustomAttribute.LogicalName;
					}

					break;
				}
				case ParameterExpression expression:
				{
					if (memberExpression.Member.Name == "Id")
					{
						var defaultCustomAttribute = expression.Type.GetProperty("Id").GetFirstOrDefaultCustomAttribute<AttributeLogicalNameAttribute>();
						if (defaultCustomAttribute != null)
						{
							return defaultCustomAttribute.LogicalName;
						}
					}

					break;
				}
			}

			return memberExpression.Member.GetLogicalName();
		}

		private string TranslateExpressionToAttributeName(Expression exp)
		{
			switch (exp)
			{
				case MethodCallExpression methodCallExpression:
				{
					var valueExpression = methodCallExpression.Method.IsStatic
						? methodCallExpression.Arguments[1]
						: methodCallExpression.Arguments[0];

					var attributeName = TranslateExpressionToValue(valueExpression);
					return (string)attributeName;
				}
				case MemberExpression memberExpression:
				{
					if (memberExpression.Expression is MemberExpression expression1)
					{
						var defaultCustomAttribute = expression1.Member.GetFirstOrDefaultCustomAttribute<AttributeLogicalNameAttribute>();
						if (defaultCustomAttribute != null)
						{
							return defaultCustomAttribute.LogicalName;
						}
					}
					else if (memberExpression.Expression is ParameterExpression expression2 && memberExpression.Member.Name == "Id")
					{
						var defaultCustomAttribute = expression2.Type.GetProperty("Id").GetFirstOrDefaultCustomAttribute<AttributeLogicalNameAttribute>();
						if (defaultCustomAttribute != null)
						{
							return defaultCustomAttribute.LogicalName;
						}
					}
					return memberExpression.Member.GetLogicalName();
				}
			}
			throw new InvalidOperationException("Cannot determine the attribute name.");
		}

		private bool IsEnumerableEntity(Type type)
		{
			if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(IEnumerable<>))
			{
				return false;
			}

			var genericArguments = type.GetGenericArguments();
			return genericArguments.Length == 1 && IsEntity(genericArguments[0]);
		}

		private static bool IsAnonymousType(Type type)
		{
			return 
				type.GetCustomAttributes(typeof (CompilerGeneratedAttribute), false).Any() && 
				type.Name.Contains("AnonymousType");
		}

		private bool IsEntity(Type type)
		{
			return true;
		}

		private Expression FindValidEntityExpression(Expression exp, string operation)
		{
			if (exp is UnaryExpression unaryExpression && (unaryExpression.NodeType == ExpressionType.Convert || unaryExpression.NodeType == ExpressionType.TypeAs))
			{
				return FindValidEntityExpression(unaryExpression.Operand, operation);
			}

			if (exp is NewExpression newExpression && newExpression.Type == typeof(EntityReference) && newExpression.Arguments.Count >= 2)
			{
				return FindValidEntityExpression(newExpression.Arguments[1], operation);
			}

			if (IsEntityExpression(exp))
			{
				return exp;
			}

			switch (exp)
			{
				case MemberExpression memberExpression when _validProperties.Contains(memberExpression.Member.Name):
					return FindValidEntityExpression(memberExpression.Expression, operation);
				case MethodCallExpression methodCallExpression when _validMethods.Contains(methodCallExpression.Method.Name):
					return FindValidEntityExpression(methodCallExpression.Object, operation);
				default:
					return null;//throw new NotSupportedException($"Invalid '{operation}' condition. An entity member is invoking an invalid property or method.");
			}
		}

		private bool IsEntityExpression(Expression e)
		{
			if (e is MethodCallExpression methodCallExpression)
			{
				if (methodCallExpression.Object != null)
				{
					return methodCallExpression.Object.Type == typeof(ICrmEntity);
				}

				if (methodCallExpression.Method.IsStatic)
				{
					return methodCallExpression.Arguments[0].Type == typeof(ICrmEntity);
				}
			}
			else if (e is MemberExpression me)
			{
				return IsEntityMemberExpression(me);
			}

			return false;
		}

		private bool IsEntityMemberExpression(MemberExpression me)
		{
			return IsEntity(me.Member.DeclaringType);
		}

		//private MemberExpression GetUnderlyingMemberExpression(Expression exp)
		//{
		//	switch (exp)
		//	{
		//		case MemberExpression memberExpression:
		//			return memberExpression.Expression as MemberExpression;
		//		case MethodCallExpression methodCallExpression:
		//			return methodCallExpression.Object as MemberExpression;
		//		default:
		//			throw new InvalidOperationException($"The expression '{exp}' must be a '{typeof(MemberExpression)}' or a '{typeof(MethodCallExpression)}'.");
		//	}
		//}

		private string GetUnderlyingParameterExpressionName(MemberExpression memberExpression)
		{
			return memberExpression.Expression.ToString();
		}
		private string GetUnderlyingParameterExpressionName(MethodCallExpression methodCallExpression)
		{
			Debug.Assert(methodCallExpression.Object != null, "methodCallExpression.Object != null");

			return methodCallExpression.Object.ToString();
		}
		private string GetUnderlyingParameterExpressionName(Expression exp)
		{
			switch (exp)
			{
				case MemberExpression memberExpression:
					return memberExpression.Expression.ToString();
				case MethodCallExpression methodCallExpression:
					Debug.Assert(methodCallExpression.Object != null, "methodCallExpression.Object != null");
					return methodCallExpression.Object.ToString();
				default:
					throw new InvalidOperationException($"The expression '{exp}' must be a '{typeof(MemberExpression)}' or a '{typeof(MethodCallExpression)}'.");
			}
		}

		private object TranslateExpressionToValue(Expression exp, params ParameterExpression[] parameters)
		{
			if (exp is ConstantExpression constantExpression)
			{
				return constantExpression.Value;
			}

			if (exp is MemberExpression memberExpression && memberExpression.Expression is ConstantExpression expression)
			{
				switch (memberExpression.Member)
				{
					case FieldInfo filedMember:
						return GetFieldValue(filedMember, expression.Value);
					case PropertyInfo propertyMember:
						return GetPropertyValue(propertyMember, expression.Value);
				}
			}

			if (exp is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
			{
				return TranslateExpressionToValue(unaryExpression.Operand);
			}
			else
			{
				return DynamicInvoke(CompileExpression(Expression.Lambda(exp, parameters)));
			}
		}

		[SecuritySafeCritical]
		private object GetFieldValue(FieldInfo fieldInfo, object target)
		{
			try
			{
				new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess).Assert();
				return fieldInfo.GetValue(target);
			}
			finally
			{
				CodeAccessPermission.RevertAssert();
			}
		}

		[SecuritySafeCritical]
		private object GetPropertyValue(PropertyInfo propertyInfo, object target)
		{
			try
			{
				new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess).Assert();
				return propertyInfo.GetValue(target, null);
			}
			finally
			{
				CodeAccessPermission.RevertAssert();
			}
		}

		private object TranslateExpressionToConditionValue(Expression exp, params ParameterExpression[] parameters)
		{
			var obj = TranslateExpressionToValue(exp, parameters);
			if (obj is DateTime dateTime)
			{
				obj = dateTime.ToString("u", CultureInfo.InvariantCulture);
			}
			else if (obj is EntityReference entityReference)
			{
				obj = entityReference.Id;
			}
			else if (obj is Money money)
			{
				obj = money.Value;
			}
			else if (obj is OptionSetValue optionSetValue)
			{
				obj = optionSetValue.Value;
			}
			else if (obj != null && obj.GetType().IsEnum)
			{
				obj = (int)obj;
			}

			return obj;
		}

		private void TranslateEntityName(QueryExpression qe, Expression expression)
		{
			if (qe.EntityName != null)
			{
				return;
			}

			var constantExpression = expression is MethodCallExpression 
				? expression.GetMethodsPreorder().Last().Arguments[0] as ConstantExpression 
				: expression as ConstantExpression;

			if (constantExpression?.Value is ICrmEntity entityQuery)
			{
				qe.EntityName = entityQuery.EntityLogicalName;
			}
		}
	}
}
