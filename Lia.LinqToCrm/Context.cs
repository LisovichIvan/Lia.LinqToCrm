using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Lia.LinqToCrm
{
	public class Context
	{
		private IOrganizationService _service;

		public Context(IOrganizationService service)
		{
			_service = service;
		}

		public ICrmQueryable<T> CreateQuery<T>() where T : ICrmEntity
		{
			return new Query<T>(null);
		}
	}
}
