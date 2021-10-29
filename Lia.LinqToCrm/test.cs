using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApp6
{
	public interface IQu<T>// : IEnumerable<T> 
	{ }

	internal class Qu<T> : IQu<T>
	{
		//IEnumerator<T> IEnumerable<T>.GetEnumerator()
		//{
		//	throw new NotImplementedException();
		//}

		//public IEnumerator GetEnumerator()
		//{
		//	throw new NotImplementedException();
		//}
	}

	public interface IQuTake<T>
	{

	}

	public interface IQuSkip<T>
	{

	}

	public static class QuEx
	{
		public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(
			this IQu<TSource> source,
			Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(
			this IQu<TSource> source,
			Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(
			this IOrderedQueryable<TSource> source,
			Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static IQuTake<TSource> Take<TSource>(
			this IQu<TSource> source,
			int count)
		{
			return null;
		}

		public static IQuTake<TSource> Take<TSource>(
			this IQuTake<TSource> source,
			int count)
		{
			return null;
		}

		public static IQuSkip<TSource> Skip<TSource>(
			this IQu<TSource> source,
			int count)
		{
			return null;
		}

		public static IQuSkip<TSource> Skip<TSource>(
			this IQuSkip<TSource> source,
			int count)
		{
			return null;
		}

		public static TSource First<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TSource First<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource FirstOrDefault<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TSource FirstOrDefault<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource Single<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TSource Single<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static bool Any<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static bool Any<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource SingleOrDefault<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TSource SingleOrDefault<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static IQu<TSource> Distinct<TSource>(this IQu<TSource> source)
		{
			return null;
		}

		public static int Count<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static int Count<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static long LongCount<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static long LongCount<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static IQu<TResult> SelectMany<TSource, TCollection, TResult>(
	        this IQu<TSource> source,
	        Expression<Func<TSource, IQu<TCollection>>> collectionSelector,
	        Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
	        return null;
        }

        public static IQu<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
	        this IQu<TOuter> outer,
	        IQu<TInner> inner,
	        Expression<Func<TOuter, TKey>> outerKeySelector,
	        Expression<Func<TInner, TKey>> innerKeySelector,
	        Expression<Func<TOuter, IQu<TInner>, TResult>> resultSelector)
        {
	        return null;
        }

		public static IQu<TSource> DefaultIfEmpty<TSource>(this IQu<TSource> source)
		{
			return null;
		}

		public static IQu<TResult> Join<TOuter, TInner, TKey, TResult>(
			this IQu<TOuter> outer,
			IQu<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, TInner, TResult>> resultSelector)
		{
			return null;
		}

		public static IQu<TSource> Where<TSource>(
			this IQu<TSource> source, 
			Expression<Func<TSource, bool>> predicate)
		{
			return null;
		}


	}
	
	class Program
	{
		static IQu<int> a = new Qu<int>();
		static IQu<int> b = new Qu<int>();

		static void Main(string[] args)
		{
			var temp = a.Take(10).Take(10);
			var temp0 = a.Skip(10).Skip(10);

			Console.WriteLine("press any key");
			Console.ReadKey();
		}

		static void Where_L()
		{
			var temp1 =
				from a1 in a
				where a1 > 10
				select a1;
		}

		static void Inner_Join_Where_L()
		{
			var temp1 =
				from a1 in a
				join b1 in b on a1 equals b1
				where a1 > 10 && b1 > 10
				select a1;
		}
		static void Inner_Join_Where_M()
		{
			var temp1 = a.Join(b, a1 => a1, b1 => b1, (a1, b1) => new {a1, b1})
				.Where(@t => @t.a1 > 10 && @t.b1 > 10)
				.Select(@t => @t.a1);
		}

		static void Inner_Join_L()
		{
			var temp1 =
				from a1 in a
				join b1 in b on a1 equals b1
				select a1;
		}

		static void Inner_Join_M()
		{
			var temp1 = a.Join(b, a1 => a1, b1 => b1, (a1, b1) => a1);
		}

		static void Left_Join_L()
		{
			var temp =
				from a1 in a
				join b1 in b on a1 equals b1 into b11
				from b111 in b11.DefaultIfEmpty()
				select new {a1, b111};
		}
		
		static void Left_Join_M()
		{
			var temp = a
				.GroupJoin(b, a1 => a1, b1 => b1, (a1, b11) => new {a1, b11})
				.SelectMany(@t => @t.b11.DefaultIfEmpty(), (@t, b111) => new {@t.a1, b111});		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp6
{
	public static class QuNotSupported
	{
		public static IQu<TSource> Where<TSource>(
			this IQu<TSource> source,
			Expression<Func<TSource, int, bool>> predicate)
		{
			return null;
		}

		public static IQu<TResult> Select<TSource, TResult>(
			this IQu<TSource> source,
			Expression<Func<TSource, TResult>> selector)
		{
			return null;
		}

		public static IQu<TResult> Select<TSource, TResult>(
			this IQu<TSource> source,
			Expression<Func<TSource, int, TResult>> selector)
		{
			return null;
		}

		public static IQu<TResult> SelectMany<TSource, TResult>(
			this IQu<TSource> source,
			Expression<Func<TSource, IEnumerable<TResult>>> selector)
		{
			return null;
		}

		public static IQu<TResult> SelectMany<TSource, TResult>(
			this IQu<TSource> source,
			Expression<Func<TSource, int, IEnumerable<TResult>>> selector)
		{
			return null;
		}

		public static IQu<TResult> SelectMany<TSource, TCollection, TResult>(
			this IQu<TSource> source,
			Expression<Func<TSource, int, IEnumerable<TCollection>>> collectionSelector,
			Expression<Func<TSource, TCollection, TResult>> resultSelector)
		{
			return null;
		}
		public static IQu<TResult> Join<TOuter, TInner, TKey, TResult>(
			this IQu<TOuter> outer, IEnumerable<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, TInner, TResult>> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static IQu<TSource> DefaultIfEmpty<TSource>(
			this IQu<TSource> source,
			TSource defaultValue)
		{
			return null;
		}
		
		public static IQu<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
			this IQu<TOuter> outer,
			IEnumerable<TInner> inner,
			Expression<Func<TOuter, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(
			this IQu<TSource> source,
			Expression<Func<TSource, TKey>> keySelector,
			IComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(
			this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector,
			IComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static IOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
		{
			return null;
		}

		public static IQu<TSource> TakeWhile<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return null;
		}

		public static IQu<TSource> TakeWhile<TSource>(this IQu<TSource> source, Expression<Func<TSource, int, bool>> predicate)
		{
			return null;
		}

		public static IQu<TSource> SkipWhile<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return null;
		}

		public static IQu<TSource> SkipWhile<TSource>(this IQu<TSource> source, Expression<Func<TSource, int, bool>> predicate)
		{
			return null;
		}

		public static IQu<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static IQu<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
		{
			return null;
		}

		public static IQu<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static IQu<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static IQu<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector)
		{
			return null;
		}

		public static IQu<TResult> GroupBy<TSource, TKey, TResult>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector)
		{
			return null;
		}

		public static IQu<TResult> GroupBy<TSource, TKey, TResult>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static IQu<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IQu<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return null;
		}

		public static IQu<TSource> Distinct<TSource>(this IQu<TSource> source, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static IQu<TSource> Concat<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static IQu<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IQu<TFirst> source1, IEnumerable<TSecond> source2)
		{
			return null;
		}

		public static IQu<TResult> Zip<TFirst, TSecond, TResult>(this IQu<TFirst> source1, IEnumerable<TSecond> source2, Expression<Func<TFirst, TSecond, TResult>> resultSelector)
		{
			return null;
		}

		public static IQu<TSource> Union<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static IQu<TSource> Union<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static IQu<TSource> Intersect<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static IQu<TSource> Intersect<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static IQu<TSource> Except<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2)
		{
			return null;
		}

		public static IQu<TSource> Except<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return null;
		}

		public static TSource Last<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TSource Last<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource LastOrDefault<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TSource LastOrDefault<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource ElementAt<TSource>(this IQu<TSource> source, int index)
		{
			return default;
		}

		public static TSource ElementAtOrDefault<TSource>(this IQu<TSource> source, int index)
		{
			return default;
		}

		public static bool Contains<TSource>(this IQu<TSource> source, TSource item)
		{
			return default;
		}

		public static bool Contains<TSource>(this IQu<TSource> source, TSource item, IEqualityComparer<TSource> comparer)
		{
			return default;
		}

		public static IQu<TSource> Reverse<TSource>(this IQu<TSource> source)
		{
			return null;
		}

		public static bool SequenceEqual<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2)
		{
			return default;
		}

		public static bool SequenceEqual<TSource>(this IQu<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
		{
			return default;
		}

		public static bool All<TSource>(this IQu<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			return default;
		}

		public static TSource Min<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TResult Min<TSource, TResult>(this IQu<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			return default;
		}

		public static TSource Max<TSource>(this IQu<TSource> source)
		{
			return default;
		}

		public static TResult Max<TSource, TResult>(this IQu<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			return default;
		}

		public static int Sum(this IQu<int> source)
		{
			return default;
		}

		public static int? Sum(this IQu<int?> source)
		{
			return null;
		}

		public static long Sum(this IQu<long> source)
		{
			return default;
		}

		public static long? Sum(this IQu<long?> source)
		{
			return null;
		}

		public static float Sum(this IQu<float> source)
		{
			return default;
		}

		public static float? Sum(this IQu<float?> source)
		{
			return null;
		}

		public static double Sum(this IQu<double> source)
		{
			return default;
		}

		public static double? Sum(this IQu<double?> source)
		{
			return null;
		}

		public static decimal Sum(this IQu<decimal> source)
		{
			return default;
		}

		public static decimal? Sum(this IQu<decimal?> source)
		{
			return null;
		}

		public static int Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, int>> selector)
		{
			return default;
		}

		public static int? Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, int?>> selector)
		{
			return null;
		}

		public static long Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, long>> selector)
		{
			return default;
		}

		public static long? Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, long?>> selector)
		{
			return null;
		}

		public static float Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, float>> selector)
		{
			return default;
		}

		public static float? Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, float?>> selector)
		{
			return null;
		}

		public static double Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, double>> selector)
		{
			return default;
		}

		public static double? Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, double?>> selector)
		{
			return null;
		}

		public static decimal Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, decimal>> selector)
		{
			return default;
		}

		public static decimal? Sum<TSource>(this IQu<TSource> source, Expression<Func<TSource, decimal?>> selector)
		{
			return null;
		}

		public static double Average(this IQu<int> source)
		{
			return default;
		}

		public static double? Average(this IQu<int?> source)
		{
			return null;
		}

		public static double Average(this IQu<long> source)
		{
			return default;
		}

		public static double? Average(this IQu<long?> source)
		{
			return null;
		}

		public static float Average(this IQu<float> source)
		{
			return default;
		}

		public static float? Average(this IQu<float?> source)
		{
			return null;
		}

		public static double Average(this IQu<double> source)
		{
			return default;
		}

		public static double? Average(this IQu<double?> source)
		{
			return null;
		}

		public static decimal Average(this IQu<decimal> source)
		{
			return default;
		}

		public static decimal? Average(this IQu<decimal?> source)
		{
			return null;
		}

		public static double Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, int>> selector)
		{
			return default;
		}

		public static double? Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, int?>> selector)
		{
			return null;
		}

		public static float Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, float>> selector)
		{
			return default;
		}

		public static float? Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, float?>> selector)
		{
			return null;
		}

		public static double Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, long>> selector)
		{
			return default;
		}

		public static double? Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, long?>> selector)
		{
			return null;
		}

		public static double Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, double>> selector)
		{
			return default;
		}

		public static double? Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, double?>> selector)
		{
			return null;
		}

		public static decimal Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, decimal>> selector)
		{
			return default;
		}

		public static decimal? Average<TSource>(this IQu<TSource> source, Expression<Func<TSource, decimal?>> selector)
		{
			return null;
		}

		public static TSource Aggregate<TSource>(this IQu<TSource> source, Expression<Func<TSource, TSource, TSource>> func)
		{
			return default;
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this IQu<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func)
		{
			return default;
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this IQu<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector)
		{
			return default;
		}

		public static IQu<TSource> SkipLast<TSource>(this IQu<TSource> source, int count)
		{
			return null;
		}

		public static IQu<TSource> TakeLast<TSource>(this IQu<TSource> source, int count)
		{
			return null;
		}

		public static IQu<TSource> Append<TSource>(this IQu<TSource> source, TSource element)
		{
			return null;
		}

		public static IQu<TSource> Prepend<TSource>(this IQu<TSource> source, TSource element)
		{
			return null;
		}
	}
}
