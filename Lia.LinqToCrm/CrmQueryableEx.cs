using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public static class CrmQueryableEx
	{
		public static TSource First<TSource>(this ICrmQueryable<TSource> source)
		{
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