using Microsoft.Xrm.Sdk;

namespace Lia.LinqToCrm.Provider
{
	internal sealed class NavigationSource
	{
		public EntityReference Target { get; }

		public Relationship Relationship { get; }

		public NavigationSource(EntityReference target, Relationship relationship)
		{
			Target = target;
			Relationship = relationship;
		}
	}
}