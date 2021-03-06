using Microsoft.Xrm.Sdk;

namespace Lia.LinqToCrm.Tests.Model
{
	class SystemUser : ICrmEntity
	{
		public class Ref { }

		public Ref SystemUserId { set; get; }

		public string EntityLogicalName { get; } = "systemuser";

		[AttributeLogicalName("firstname")]
		public string FirstName { get; set; }
	}
}