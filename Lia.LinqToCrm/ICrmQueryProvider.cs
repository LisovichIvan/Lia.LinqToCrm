using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public interface ICrmQueryProvider
	{
		ICrmQueryable<TElement> CreateQuery<TElement>(Expression expression) where TElement : ICrmEntity;
		TResult Execute<TResult>(Expression expression) where TResult : ICrmEntity;
		IEnumerable<TResult> ExecuteEnumerable<TResult>(IEnumerable<Expression> expression, string entityName) where TResult : ICrmEntity;
	}
}