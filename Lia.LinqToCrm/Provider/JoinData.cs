using System.Linq.Expressions;

namespace Lia.LinqToCrm.Provider
{
	internal readonly struct JoinData
	{
		public ConstantExpression Outer => (ConstantExpression) _methodCallExpression.Arguments[0];
		public ConstantExpression Inner => (ConstantExpression) _methodCallExpression.Arguments[1];
		public UnaryExpression OuterKeySelector => (UnaryExpression) _methodCallExpression.Arguments[2];
		public UnaryExpression InnerKeySelector => (UnaryExpression) _methodCallExpression.Arguments[3];
		public UnaryExpression ResultSelector => (UnaryExpression) _methodCallExpression.Arguments[4];
		public string MethodName => _methodCallExpression.Method.Name;

		private readonly MethodCallExpression _methodCallExpression;

		public JoinData(MethodCallExpression methodCallExpression)
		{
			_methodCallExpression = methodCallExpression;
		}
	}
}