using System;
using Microsoft.Xrm.Sdk.Query;

namespace Lia.LinqToCrm.Provider
{
	internal sealed class FilterExpressionWrapper
	{
		public FilterExpression Filter { get; }

		public string Alias { get; }

		public FilterExpressionWrapper(FilterExpression filter, string alias)
		{
			Filter = filter ?? throw new ArgumentNullException(nameof(filter));
			Alias = alias;
		}
	}
}