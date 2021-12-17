using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xrm.Sdk.Query;

namespace Lia.LinqToCrm.Provider
{
	internal sealed class PagedItemCollection<TSource> : PagedItemCollectionBase, IEnumerable<TSource>, IEnumerator<TSource>
	{
		private TSource _current;
		private IEnumerator<TSource> _enumerator;
		private readonly IEnumerable<TSource> _source;

		public PagedItemCollection(IEnumerable<TSource> source, QueryExpression query, string pagingCookie, bool moreRecords)
			: base(query, pagingCookie, moreRecords)
		{
			_source = source;
		}

		public PagedItemCollection<TSource> Clone()
		{
			return new PagedItemCollection<TSource>(_source, Query, PagingCookie, MoreRecords);
		}

		public IEnumerator<TSource> GetEnumerator()
		{
			return Clone();
		}

		[SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "The enumerator will be disposed by the calling method.")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public TSource Current => _current;

		public void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
			}

			_enumerator = null;
			_current = default (TSource);
		}

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			if (_enumerator == null)
			{
				_enumerator = _source.GetEnumerator();
			}

			if (!_enumerator.MoveNext())
			{
				return false;
			}

			_current = _enumerator.Current;
			return true;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}
}