using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;

namespace Lia.LinqToCrm.Tests.Model
{
	class Contact : ICrmEntity
	{
		public string EntityLogicalName { get; } = "contact";

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public SystemUser.Ref OwnerId { get; set; }
	}
}
