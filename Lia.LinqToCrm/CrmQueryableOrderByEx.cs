using System;
using System.Linq.Expressions;

namespace Lia.LinqToCrm
{
	public static class CrmQueryableOrderByEx
	{
		public static ICrmOrderedQueryable<TSource> OrderBy<TSource, TKey>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static ICrmOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(
			this ICrmQueryable<TSource> source,
			Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static ICrmOrderedQueryable<TSource> ThenBy<TSource, TKey>(
			this ICrmOrderedQueryable<TSource> source,
			Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}

		public static ICrmOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(
			this ICrmOrderedQueryable<TSource> source,
			Expression<Func<TSource, TKey>> keySelector)
		{
			return null;
		}
	}
}