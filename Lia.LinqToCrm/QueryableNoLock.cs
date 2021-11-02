using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Lia.LinqToCrm
{
	public static class QueryableNoLock
	{
		private static readonly Dictionary<Type, MethodInfo> _methodsMap = new Dictionary<Type, MethodInfo>();
		private static readonly MethodInfo _noLock = typeof(QueryableNoLock).GetMethod(nameof(QueryableNoLock.NoLock));

		private static MethodInfo GetMethod(Type type)
		{
			if (_methodsMap.TryGetValue(type, out var value))
			{
				return value;
			}

			lock (_noLock)
			{
				if (_methodsMap.TryGetValue(type, out value))
				{
					return value;
				}

				value = _noLock.MakeGenericMethod(type);

				_methodsMap.Add(type, value);

				return value;
			}
		}

		public static ICrmQueryable<TSource> NoLock<TSource>(this ICrmQueryable<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			var genericMethod = GetMethod(typeof(TSource));

			var expression = Expression.Call(null, genericMethod, source.Expression);

			return source.Provider.CreateQuery<TSource>(expression);
		}
	}
}