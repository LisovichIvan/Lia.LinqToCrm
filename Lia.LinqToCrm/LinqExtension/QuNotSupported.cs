using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public static class QuNotSupported
	{
		public static ICrmQueryable<TSource> Where<TSource>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, int, bool>> predicate)
		{
			return null;
		}

		public static ICrmQueryable<TResult> Select<TSource, TResult>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, TResult>> selector)
		{
			return null;
		}

		public static ICrmQueryable<TResult> Select<TSource, TResult>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, int, TResult>> selector)
		{
			return null;
		}

		public static ICrmQueryable<TResult> SelectMany<TSource, TResult>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, IEnumerable<TResult>>> selector)
		{
			return null;
		}

		public static ICrmQueryable<TResult> SelectMany<TSource, TResult>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, int, IEnumerable<TResult>>> selector)
		{
			return null;
		}

		public static ICrmQueryable<TResult> SelectMany<TSource, TCollection, TResult>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, int, IEnumerable<TCollection>>> collectionSelector,
			Expression<Func<TSource, TCollection, TResult>> resultSelector)
		{
			return null;
		}
		public static ICrmQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(
			this ICrmQueryable<TOuter> outer, IEnumerable<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, TInner, TResult>> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TSource> DefaultIfEmpty<TSource>(
			this ICrmQueryable<TSource> source,
			TSource defaultValue)
		{
			return null;
		}
		
		public static ICrmQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
			this ICrmQueryable<TOuter> outer,
			IEnumerable<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, TKey>> keySelector,
			IComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(
			this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector,
			IComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TSource> TakeWhile<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return null;
		}

		public static ICrmQueryable<TSource> TakeWhile<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
		{
			return null;
		}

		public static ICrmQueryable<TSource> SkipWhile<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return null;
		}

		public static ICrmQueryable<TSource> SkipWhile<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
		{
			return null;
		}

		public static ICrmQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static ICrmQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
		{
			return null;
		}

		public static ICrmQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static ICrmQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector)
		{
			return null;
		}

		public static ICrmQueryable<TResult> GroupBy<TSource, TKey, TResult>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector)
		{
			return null;
		}

		public static ICrmQueryable<TResult> GroupBy<TSource, TKey, TResult>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Distinct<TSource>(this ICrmQueryable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Concat<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static ICrmQueryable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this ICrmQueryable<TFirst> source1, IEnumerable<TSecond> source2)
		{
			return null;
		}

		public static ICrmQueryable<TResult> Zip<TFirst, TSecond, TResult>(this ICrmQueryable<TFirst> source1, IEnumerable<TSecond> source2, Expression<Func<TFirst, TSecond, TResult>> resultSelector)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Union<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Union<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Intersect<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Intersect<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Except<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Except<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static TSource Last<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static TSource Last<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource LastOrDefault<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static TSource LastOrDefault<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource ElementAt<TSource>(this ICrmQueryable<TSource> source, int index)
		{
			return default;
		}

		public static TSource ElementAtOrDefault<TSource>(this ICrmQueryable<TSource> source, int index)
		{
			return default;
		}

		public static bool Contains<TSource>(this ICrmQueryable<TSource> source, TSource item)
		{
			return default;
		}

		public static bool Contains<TSource>(this ICrmQueryable<TSource> source, TSource item, IEqualityComparer<TSource> comparer)
		{
			return default;
		}

		public static ICrmQueryable<TSource> Reverse<TSource>(this ICrmQueryable<TSource> source)
		{
			return null;
		}

		public static bool SequenceEqual<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2)
		{
			return default;
		}

		public static bool SequenceEqual<TSource>(this ICrmQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return default;
		}

		public static bool All<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource Min<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static TResult Min<TSource, TResult>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			return default;
		}

		public static TSource Max<TSource>(this ICrmQueryable<TSource> source)
		{
			return default;
		}

		public static TResult Max<TSource, TResult>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			return default;
		}

		public static int Sum(this ICrmQueryable<int> source)
		{
			return default;
		}

		public static int? Sum(this ICrmQueryable<int?> source)
		{
			return null;
		}

		public static long Sum(this ICrmQueryable<long> source)
		{
			return default;
		}

		public static long? Sum(this ICrmQueryable<long?> source)
		{
			return null;
		}

		public static float Sum(this ICrmQueryable<float> source)
		{
			return default;
		}

		public static float? Sum(this ICrmQueryable<float?> source)
		{
			return null;
		}

		public static double Sum(this ICrmQueryable<double> source)
		{
			return default;
		}

		public static double? Sum(this ICrmQueryable<double?> source)
		{
			return null;
		}

		public static decimal Sum(this ICrmQueryable<decimal> source)
		{
			return default;
		}

		public static decimal? Sum(this ICrmQueryable<decimal?> source)
		{
			return null;
		}

		public static int Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, int>> selector)
		{
			return default;
		}

		public static int? Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
		{
			return null;
		}

		public static long Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, long>> selector)
		{
			return default;
		}

		public static long? Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
		{
			return null;
		}

		public static float Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, float>> selector)
		{
			return default;
		}

		public static float? Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
		{
			return null;
		}

		public static double Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, double>> selector)
		{
			return default;
		}

		public static double? Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
		{
			return null;
		}

		public static decimal Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
		{
			return default;
		}

		public static decimal? Sum<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
		{
			return null;
		}

		public static double Average(this ICrmQueryable<int> source)
		{
			return default;
		}

		public static double? Average(this ICrmQueryable<int?> source)
		{
			return null;
		}

		public static double Average(this ICrmQueryable<long> source)
		{
			return default;
		}

		public static double? Average(this ICrmQueryable<long?> source)
		{
			return null;
		}

		public static float Average(this ICrmQueryable<float> source)
		{
			return default;
		}

		public static float? Average(this ICrmQueryable<float?> source)
		{
			return null;
		}

		public static double Average(this ICrmQueryable<double> source)
		{
			return default;
		}

		public static double? Average(this ICrmQueryable<double?> source)
		{
			return null;
		}

		public static decimal Average(this ICrmQueryable<decimal> source)
		{
			return default;
		}

		public static decimal? Average(this ICrmQueryable<decimal?> source)
		{
			return null;
		}

		public static double Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, int>> selector)
		{
			return default;
		}

		public static double? Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
		{
			return null;
		}

		public static float Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, float>> selector)
		{
			return default;
		}

		public static float? Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
		{
			return null;
		}

		public static double Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, long>> selector)
		{
			return default;
		}

		public static double? Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
		{
			return null;
		}

		public static double Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, double>> selector)
		{
			return default;
		}

		public static double? Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
		{
			return null;
		}

		public static decimal Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
		{
			return default;
		}

		public static decimal? Average<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
		{
			return null;
		}

		public static TSource Aggregate<TSource>(this ICrmQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func)
		{
			return default;
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this ICrmQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func)
		{
			return default;
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this ICrmQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector)
		{
			return default;
		}

		public static ICrmQueryable<TSource> SkipLast<TSource>(this ICrmQueryable<TSource> source, int count)
		{
			return null;
		}

		public static ICrmQueryable<TSource> TakeLast<TSource>(this ICrmQueryable<TSource> source, int count)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Append<TSource>(this ICrmQueryable<TSource> source, TSource element)
		{
			return null;
		}

		public static ICrmQueryable<TSource> Prepend<TSource>(this ICrmQueryable<TSource> source, TSource element)
		{
			return null;
		}
	}
}