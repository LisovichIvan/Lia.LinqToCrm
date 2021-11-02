using System.Collections.Generic;

namespace Lia.LinqToCrm
{
	public static class CrmQueryableTakeSkipEx
	{
		public static List<TSource> ToList<TSource>(this ICrmQueryableTake<TSource> source)
		{
			return null;
		}

		public static List<TSource> ToList<TSource>(this ICrmQueryableSkip<TSource> source)
		{
			return null;
		}

		public static ICrmQueryableTake<TSource> Take<TSource>(
			this ICrmQueryable<TSource> source,
			int count)
		{
			return null;
		}

		public static ICrmQueryableTake<TSource> Take<TSource>(
			this ICrmQueryableTake<TSource> source,
			int count)
		{
			return null;
		}

		public static ICrmQueryableSkip<TSource> Skip<TSource>(
			this ICrmQueryable<TSource> source,
			int count)
		{
			return null;
		}

		public static ICrmQueryableSkip<TSource> Skip<TSource>(
			this ICrmQueryableSkip<TSource> source,
			int count)
		{
			return null;
		}
	}
}