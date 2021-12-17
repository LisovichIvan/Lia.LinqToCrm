using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Lia.LinqToCrm.Common
{
	internal static class ExpressionExtensions
	{
		public static T GetValue<T>(this ConstantExpression expression)
		{
			return (T)expression.Value;
		}

		public static void ForSubtreePreorder(this Expression exp, ExpressionAction action)
		{
			exp.ForSubtreePreorder(null, action);
		}

		public static void ForSubtreePreorder(this Expression exp, Expression parent, ExpressionAction action)
		{
			action(exp, parent);
			foreach (Expression child in exp.GetChildren())
			{
				child.ForSubtreePreorder(exp, action);
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "Value is returned from method and cannot be disposed.", Target = "local$0")]
		public static IEnumerable<Expression> GetChildren(this Expression exp)
		{
			switch (exp)
			{
				case UnaryExpression expression:
					yield return expression.Operand;
					break;
				case BinaryExpression expression:
					yield return expression.Left;
					yield return expression.Right;
					yield return expression.Conversion;
					break;
				case TypeBinaryExpression expression:
					yield return expression.Expression;
					break;
				case ConditionalExpression expression:
					yield return expression.Test;
					yield return expression.IfTrue;
					yield return expression.IfFalse;
					break;
				case MemberExpression expression:
					yield return expression.Expression;
					break;
				case MethodCallExpression callExpression:
					yield return callExpression.Object;
					foreach (Expression expression in callExpression.Arguments)
						yield return expression;
					break;
				case LambdaExpression expression:
					yield return expression.Body;
					foreach (ParameterExpression parameter in expression.Parameters)
						yield return parameter;
					break;
				case NewExpression newExpression:
					foreach (Expression expression in newExpression.Arguments)
						yield return expression;
					break;
				case NewArrayExpression arrayExpression:
					foreach (Expression expression in arrayExpression.Expressions)
						yield return expression;
					break;
				case InvocationExpression invocationExpression:
					yield return invocationExpression.Expression;
					foreach (Expression expression in invocationExpression.Arguments)
						yield return expression;
					break;
				case MemberInitExpression expression:
					yield return expression.NewExpression;
					foreach (Expression child in GetChildren(expression.Bindings))
						yield return child;
					break;
				case ListInitExpression expression:
					yield return expression.NewExpression;
					foreach (Expression child in GetChildren(expression.Initializers))
						yield return child;
					break;
			}
		}

		private static IEnumerable<Expression> GetChildren(IEnumerable<MemberBinding> bindings)
		{
			return bindings.SelectMany(GetChildren);
		}

		[SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "Value is returned from method and cannot be disposed.", Target = "local$0")]
		private static IEnumerable<Expression> GetChildren(MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					yield return ((MemberAssignment)binding).Expression;
					break;
				case MemberBindingType.MemberBinding:
					foreach (Expression child in GetChildren(((MemberMemberBinding)binding).Bindings))
					{
						yield return child;
					}
					break;
				case MemberBindingType.ListBinding:
					using (IEnumerator<Expression> enumerator = GetChildren(((MemberListBinding)binding).Initializers).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Expression child = enumerator.Current;
							yield return child;
						}
						break;
					}
			}
		}

		private static IEnumerable<Expression> GetChildren(IEnumerable<ElementInit> initializers)
		{
			return initializers.SelectMany(initializer => initializer.Arguments);
		}

		[SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "Value is returned from method and cannot be disposed.", Target = "local$0")]
		public static IEnumerable<Expression> GetSubtreePreorder(this Expression exp)
		{
			yield return exp;
			foreach (Expression expression in exp.GetChildren().SelectMany(child => child.GetSubtreePreorder()))
			{
				yield return expression;
			}
		}

		public static Expression FindPreorder(this Expression exp, Predicate<Expression> match)
		{
			return exp.GetSubtreePreorder().FirstOrDefault(child => match(child));
		}

		[SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "Value is returned from method and cannot be disposed.", Target = "local$0")]
		public static IEnumerable<MethodCallExpression> GetMethodsPreorder(this Expression exp)
		{
			if (exp is MethodCallExpression mce)
			{
				yield return mce;
				foreach (MethodCallExpression methodCallExpression in mce.Arguments[0].GetMethodsPreorder())
				{
					yield return methodCallExpression;
				}
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "Value is returned from method and cannot be disposed.", Target = "local$0")]
		public static IEnumerable<MethodCallExpression> GetMethodsPostorder(this Expression exp)
		{
			if (exp is MethodCallExpression mce)
			{
				foreach (MethodCallExpression methodCallExpression in mce.Arguments[0].GetMethodsPostorder())
				{
					yield return methodCallExpression;
				}
				yield return mce;
			}
		}

		public delegate void ExpressionAction(Expression exp, Expression parent);
	}
}