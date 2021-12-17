using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Lia.LinqToCrm.Provider
{
	internal  sealed partial class QueryProvider : ICrmQueryProvider
	{
		private IOrganizationService _service;

		public QueryProvider(IOrganizationService service)
		{
			_service = service;
		}

		public ICrmQueryable<TElement> CreateQuery<TElement>(Expression expression) where TElement : ICrmEntity
		{
			throw new NotImplementedException();
		}

		public TResult Execute<TResult>(Expression expression) where TResult : ICrmEntity
		{
			throw new NotImplementedException();
		}

		private const int RetrievalUpperLimitWithoutPagingCookie = 5000;
		
		/// <summary>
		/// поиск наибольшего общего делителя
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static int gcd(int a, int b)
		{
			while (b != 0)
			{
				b = a % (a = b);
			}

			return a;
		}

		private static int gcd_fast(int a, int b) 
		{
			int nod = 1;
			if (a == 0)
				return b;
			if (b == 0)
				return a;
			if (a == b)
				return a;
			if (a == 1 || b == 1)
				return 1;

			while (a != 0 && b != 0)
			{
				if (((a & 1) | (b & 1)) == 0) 
				{
					nod <<= 1;
					a >>= 1;
					b >>= 1;
					continue;
				}

				if (((a & 1) == 0) && (b & 1) == 1) 
				{
					a >>= 1;
					continue;
				}

				if ((a & 1) == 1 && ((b & 1) == 0))
				{
					b >>= 1;
					continue;
				}

				int tmp;
				if (a > b) {
					tmp = a;
					a = b;
					b = tmp;
				}
				tmp = a;
				a = (b - a) >> 1;
				b = tmp;
			}
			if (a == 0)
				return nod * b;
			else
				return nod * a;
		} 

		private (int count, int startPage, int countPage) CalculatePageData(int targetCount, int targetPageNumber)
		{
			int newCount = gcd(targetCount, RetrievalUpperLimitWithoutPagingCookie);
			int startPage = (targetCount * (targetPageNumber - 1) / newCount) + 1;
			int countPage = targetCount / newCount;

			return (newCount, startPage, countPage);
		}

		private TResult Convert<TResult>(Entity entity) where TResult : ICrmEntity
		{
			return default(TResult);
		}

		private IEnumerable<TResult> RetrieveMultiplePages<TResult>(QueryExpression query, int countPages) where TResult : ICrmEntity
		{
			for (int i = 0; i < countPages; i++)
			{
				var respose = _service.RetrieveMultiple(query);
				foreach (var entity in respose.Entities)
				{
					yield return Convert<TResult>(entity);
				}

				if (!respose.MoreRecords)
				{
					break;
				}
				query.PageInfo.PageNumber ++;
				query.PageInfo.PagingCookie = respose.PagingCookie;
			}
		}

		public IEnumerable<TResult> ExecuteEnumerable<TResult>(IEnumerable<Expression> expression, string entityName) where TResult : ICrmEntity
		{
			var methodCallExpressions = expression.Cast<MethodCallExpression>().ToList();

			var (query, throwIfSequenceIsEmpty, throwIfSequenceNotSingle, projection, linkLookups) = GetQueryExpression(methodCallExpressions, entityName);

			if (query.PageInfo.Count > RetrievalUpperLimitWithoutPagingCookie)
			{
				var (count, startPage, countPage) = CalculatePageData(query.PageInfo.Count, query.PageInfo.PageNumber);

				query.PageInfo.Count = count;
				query.PageInfo.PageNumber = startPage;

				return RetrieveMultiplePages<TResult>(query, countPage);
			}

			var outItems = _service.RetrieveMultiple(query);

			if (throwIfSequenceIsEmpty && outItems.Entities.Count == 0)
			{
				throw new InvalidOperationException("Sequence contains no elements");
			}

			if (throwIfSequenceNotSingle && outItems.Entities.Count > 1)
			{
				throw new InvalidOperationException("Sequence contains more than one element");
			}
			return outItems.Entities.Select(Convert<TResult>);
		}
	}
}
