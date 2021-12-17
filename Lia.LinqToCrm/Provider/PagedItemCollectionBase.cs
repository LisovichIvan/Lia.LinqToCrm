using Microsoft.Xrm.Sdk.Query;

namespace Lia.LinqToCrm.Provider
{
	internal abstract class PagedItemCollectionBase
	{
		public PagedItemCollectionBase(QueryExpression query, string pagingCookie, bool moreRecords)
		{
			Query = query;
			PagingCookie = pagingCookie;
			MoreRecords = moreRecords;
		}

		public QueryExpression Query { get; }

		public bool MoreRecords { get; }

		public string PagingCookie { get; }
	}
}