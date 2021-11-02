using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	internal class Query<T> : ICrmQueryable<T>
	{
		public ICrmQueryProvider Provider { get; }
		public Expression Expression { get; }

		private readonly Query<T> _beforeQuery;

		public Query(ICrmQueryProvider provider)
		{
			Provider = provider ?? throw new ArgumentNullException(nameof(provider));
			Expression = Expression.Constant(this);
		}

		public Query(ICrmQueryProvider provider, Expression expression)
		{
			Provider = provider ?? throw new ArgumentNullException(nameof(provider));
			Expression = expression ?? throw new ArgumentNullException(nameof(expression));
		}

		public Query(Query<T> beforeQuery, Expression expression)
		{
			_beforeQuery = beforeQuery ?? throw new ArgumentNullException(nameof(beforeQuery));
			Provider = beforeQuery.Provider;
			Expression = expression ?? throw new ArgumentNullException(nameof(expression));
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		internal IEnumerable<Expression> GetExpressionsChain()
		{
			if (_beforeQuery != null)
			{
				foreach (Expression expression in _beforeQuery.GetExpressionsChain())
				{
					yield return expression;
				}
			}

			yield return Expression;
		}
	}
}
