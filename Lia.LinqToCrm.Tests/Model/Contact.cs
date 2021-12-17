using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Lia.LinqToCrm.Tests.Model
{
	class Contact : ICrmEntity
	{
		public string EntityLogicalName { get; } = "contact";

		[AttributeLogicalName("firstname")]
		public string FirstName { get; set; }

		[AttributeLogicalName("lastname")]
		public string LastName { get; set; }

		[AttributeLogicalName("ownerid")]
		public SystemUser.Ref OwnerId { get; set; }
	}
}
