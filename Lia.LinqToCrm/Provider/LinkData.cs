using Microsoft.Xrm.Sdk.Query;

namespace Lia.LinqToCrm.Provider
{
	internal class LinkData
	{
		public readonly string Item1;
		public string Environment;
		public readonly LinkEntity Link;
		public readonly string ParameterName;

		public LinkData(string parameterName, LinkEntity link, string item1, string environment)
		{
			Item1 = item1;
			Environment = environment;
			Link = link;
			ParameterName = parameterName;
		}
	}
}