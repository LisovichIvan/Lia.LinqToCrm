using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lia.LinqToCrm.Provider;
using Microsoft.Xrm.Sdk;

namespace Lia.LinqToCrm
{
	public class Context
	{
		private readonly ICrmQueryProvider _provider;

		public Context(IOrganizationService service)
		{
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			_provider = new QueryProvider(service);
		}

		public ICrmQueryable<T> CreateQuery<T>() where T : ICrmEntity
		{
			return new Query<T>(_provider);
		}
	}
}
