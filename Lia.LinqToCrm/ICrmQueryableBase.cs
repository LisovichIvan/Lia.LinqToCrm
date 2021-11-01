using System;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public interface ICrmQueryableBase<T>
	{
		Expression Expression { get; }
		Type ElementType { get; }
		ICrmQueryProvider Provider { get; }
	}
}