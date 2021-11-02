using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public interface ICrmQueryable<T>
	{
		ICrmQueryProvider Provider { get; }
		Expression Expression { get; }
	}
}
