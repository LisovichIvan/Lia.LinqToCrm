using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	internal static class CrmEntityMap<T> where T : ICrmEntity
	{
		static CrmEntityMap()
		{
			T instance = Activator.CreateInstance<T>();
			Name = instance.EntityLogicalName;
		}

		public static string Name { get; }
	}

	internal class Query<T> : ICrmQueryable<T> where T : ICrmEntity
	{
		public ICrmQueryProvider Provider { get; }
		public Expression Expression { get; }
		
		private readonly Query<T> _beforeQuery;

		public Query(ICrmQueryProvider provider)
		{
			Provider = provider ?? throw new ArgumentNullException(nameof(provider));

			Expression = System.Linq.Expressions.Expression.Constant(this);
		}

		public Query(ICrmQueryProvider provider, MethodCallExpression expression)
		{
			Provider = provider ?? throw new ArgumentNullException(nameof(provider));
			Expression = expression ?? throw new ArgumentNullException(nameof(expression));
		}

		public Query(Query<T> beforeQuery, MethodCallExpression expression)
		{
			_beforeQuery = beforeQuery ?? throw new ArgumentNullException(nameof(beforeQuery));
			Provider = beforeQuery.Provider;
			Expression = expression ?? throw new ArgumentNullException(nameof(expression));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<T> GetEnumerator()
		{
			var expressions = this.GetExpressionsChain();
			var entityName = CrmEntityMap<T>.Name;
			return Provider.ExecuteEnumerable<T>(expressions, entityName).GetEnumerator();
		}

		private IEnumerable<MethodCallExpression> GetExpressionsChain()
		{
			if (_beforeQuery != null)
			{
				foreach (var item in _beforeQuery.GetExpressionsChain())
				{
					yield return item;
				}
			}

			if (Expression is MethodCallExpression methodCallExpression)
			{
				yield return methodCallExpression;
			}
		}

	}
}
