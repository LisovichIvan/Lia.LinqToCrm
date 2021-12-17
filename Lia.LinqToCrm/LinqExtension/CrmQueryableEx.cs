using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lia.LinqToCrm
{
	internal static class MethodInfoCache<T>
	{
		private static readonly Dictionary<Type, MethodInfo> MethodsMap = new Dictionary<Type, MethodInfo>();
		private static readonly MethodInfo _noLock = typeof(QueryableNoLock).GetMethod(nameof(QueryableNoLock.NoLock));

		private static MethodInfo GetMethod(Type type)
		{
			if (MethodsMap.TryGetValue(type, out var value))
			{
				return value;
			}

			lock (_noLock)
			{
				if (MethodsMap.TryGetValue(type, out value))
				{
					return value;
				}

				value = _noLock.MakeGenericMethod(type);

				MethodsMap.Add(type, value);

				return value;
			}
		}
	}

	public static class CrmQueryableEx
	{
		public static ICrmQueryable<TResult> Cast<TSource, TResult>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static List<TSource> ToList<TSource>(this ICrmQueryable<TSource> source)
		{
			return new List<TSource>(source);
		}

		public static TSource First<TSource>(this ICrmQueryable<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			//return source.Provider.Execute<TSource>(
			//	Expression.Call(
			//		null,
			//		CachedReflectionInfo.First_TSource_1(typeof(TSource)), source.Expression));


			return default;
		}

		public static TSource First<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource FirstOrDefault<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static TSource FirstOrDefault<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource Single<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static TSource Single<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static bool Any<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static bool Any<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource SingleOrDefault<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static TSource SingleOrDefault<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static ICrmQueryable<TSource> Distinct<TSource>(this ICrmQueryable<TSource> source)
		{
			return null;
		}

		public static int Count<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static int Count<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static long LongCount<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static long LongCount<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static ICrmQueryable<TResult> SelectMany<TSource, TCollection, TResult>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, ICrmQueryable<TCollection>>> collectionSelector,
			Expression<Func<TSource, TCollection, TResult>> resultSelector)
		{
			return null;
		}

		public static ICrmQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
			this ICrmQueryable<TOuter> outer,
			ICrmQueryable<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, ICrmQueryable<TInner>, TResult>> resultSelector)
		{
			return null;
		}

		public static ICrmQueryable<TSource> DefaultIfEmpty<TSource>(this ICrmQueryable<TSource> source)
		{
			return null;
		}

		public static ICrmQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(
			this ICrmQueryable<TOuter> outer,
			ICrmQueryable<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, TInner, TResult>> resultSelector)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Where<TSource>(
			this ICrmQueryable<TSource> source, 
			Expression<Func<TSource, bool>> predicate)
		{
			return null;
		}


	}
}