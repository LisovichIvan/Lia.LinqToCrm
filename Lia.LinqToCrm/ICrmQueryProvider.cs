using System.Linq;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public interface ICrmQueryProvider
	{
		IQueryable<TElement> CreateQuery<TElement>(Expression expression);
		TResult Execute<TResult>(Expression expression);
	}
}