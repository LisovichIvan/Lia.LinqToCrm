using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	internal class MethodData
	{
		public MethodType MethodType { get; }
		public Expression Predicate { get; }

		public MethodData(MethodType methodType, Expression predicate)
		{
			MethodType = methodType;
			Predicate = predicate;
		}
	}
}