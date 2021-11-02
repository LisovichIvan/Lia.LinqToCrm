using Microsoft.Xrm.Sdk.Query;

namespace Lia.LinqToCrm.Provider
{
	internal sealed class LinkLookup
	{
		public string ParameterName { get; }

		public string Environment { get; }

		public LinkEntity Link { get; }

		public string SelectManyEnvironment { get; }

		public LinkLookup(string parameterName, string environment, LinkEntity link, string selectManyEnvironment = null)
		{
			ParameterName = parameterName;
			Environment = environment;
			Link = link;
			SelectManyEnvironment = selectManyEnvironment;
		}
	}
}