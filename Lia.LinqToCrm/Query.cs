using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	internal class Query<T> : ICrmQueryable<T>
	{
		private readonly ICrmQueryProvider _provider;
		private readonly Expression _expression;

		public Query(ICrmQueryProvider provider)
		{
			_provider = provider ?? throw new ArgumentNullException(nameof(provider));
			_expression = Expression.Constant(this);
		}

		public Query(ICrmQueryProvider provider, Expression expression)
		{
			_provider = provider ?? throw new ArgumentNullException(nameof(provider));
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _provider.GetEnumerator<T>(_expression);
		}

	}
}
