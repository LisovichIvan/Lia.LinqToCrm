using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public interface ICrmQueryable<T> : IEnumerable<T>
	{
		ICrmQueryProvider Provider { get; }
		Expression Expression { get; }
	}
}
