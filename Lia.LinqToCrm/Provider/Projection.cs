using System.Linq.Expressions;

namespace Lia.LinqToCrm.Provider
{
	internal sealed class Projection
	{
		public string MethodName { get; }

		public LambdaExpression Expression { get; }

		public Projection(string methodName, LambdaExpression expression)
		{
			MethodName = methodName;
			Expression = expression;
		}
	}
}