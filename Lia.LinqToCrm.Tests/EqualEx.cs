using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace Lia.LinqToCrm.Tests
{
	public static class EqualEx
	{
		public static void AreEqual(QueryExpression item0, QueryExpression item1)
		{
			Assert.AreEqual(item0.EntityName, item1.EntityName);
			Assert.AreEqual(item0.Distinct, item1.Distinct);
			Assert.AreEqual(item0.NoLock, item1.NoLock);
			Assert.AreEqual(item0.TopCount, item1.TopCount);
			Assert.AreEqual(item0.LinkEntities.Count, item1.LinkEntities.Count);
			Assert.AreEqual(item0.Orders.Count, item1.Orders.Count);

			AreEqual(item0.ColumnSet, item1.ColumnSet);

			AreEqual(item0.Criteria, item1.Criteria);

			AreEqual(item0.PageInfo, item1.PageInfo);
			
			foreach (var a in item0.LinkEntities)
			{
				bool flag = item1.LinkEntities.Any(b => Equal(a, b));

				if (flag == false)
				{
					Assert.Fail($"LinkEntity не совпадают. {a} не найдена");
				}
			}

			foreach (var a in item0.Orders)
			{
				bool flag = item1.Orders.Any(b => Equal(a, b));

				if (flag == false)
				{
					Assert.Fail($"OrderExpression не совпадают. {a} не найдена");
				}
			}

		}

		public static void AreEqual(ColumnSet item0, ColumnSet item1)
		{
			Assert.AreEqual(item0.AllColumns, item1.AllColumns);

			foreach (var a in item0.Columns)
			{
				bool flag = item1.Columns.Any(b => a == b);

				if (flag == false)
				{
					Assert.Fail($"Column не совпадают. {a} не найдена");
				}
			}
		}

		public static bool Equal(OrderExpression item0, OrderExpression item1)
		{
			if (item0.AttributeName != item1.AttributeName)
			{
				return false;
			}

			if (item0.OrderType != item1.OrderType)
			{
				return false;
			}

			return true;
		}

		public static void AreEqual(PagingInfo item0, PagingInfo item1)
		{
			Assert.AreEqual(item0.Count, item1.Count);

			Assert.AreEqual(item0.PageNumber, item1.PageNumber);

			Assert.AreEqual(item0.ReturnTotalRecordCount, item1.ReturnTotalRecordCount);
		}

		public static bool Equal(ColumnSet item0, ColumnSet item1)
		{
			if (item0.AllColumns != item1.AllColumns)
			{
				return false;
			}

			if (item0.Columns.Count != item1.Columns.Count)
			{
				return false;
			}

			foreach (var a in item0.Columns)
			{
				bool flag = item1.Columns.Any(b => a == b);

				if (flag == false)
				{
					return false;
				}
			}

			return true;
		}

		public static bool Equal(LinkEntity item0, LinkEntity item1)
		{
			if (item0.JoinOperator != item1.JoinOperator)
			{
				return false;
			}

			if (item0.LinkFromEntityName != item1.LinkFromEntityName)
			{
				return false;
			}

			if (item0.LinkToEntityName != item1.LinkToEntityName)
			{
				return false;
			}

			if (item0.LinkFromAttributeName != item1.LinkFromAttributeName)
			{
				return false;
			}

			if (item0.LinkToAttributeName != item1.LinkToAttributeName)
			{
				return false;
			}

			if (!Equal(item0.Columns, item1.Columns))
			{
				return false;
			}

			return true;
		}

		public static void AreEqual(FilterExpression item0, FilterExpression item1)
		{
			Assert.AreEqual(item0.FilterOperator, item1.FilterOperator);

			Assert.AreEqual(item0.Conditions.Count, item1.Conditions.Count);
			Assert.AreEqual(item0.Filters.Count, item1.Filters.Count);

			foreach (var a in item0.Conditions)
			{
				bool flag = item1.Conditions.Any(b => Equals(a, b));

				if (flag == false)
				{
					Assert.Fail($"Conditions не совпадают. {a} не найдена");
				}
			}

			foreach (var a in item0.Filters)
			{
				bool flag = item1.Filters.Any(b => Equals(a, b));

				if (flag == false)
				{
					Assert.Fail($"Filters не совпадают. {a} не найдена");
				}
			}
		}

		public static void AreEqual(ConditionExpression item0, ConditionExpression item1)
		{
			Assert.AreEqual(item0.Operator, item1.Operator);
			Assert.AreEqual(item0.EntityName, item1.EntityName);
			Assert.AreEqual(item0.AttributeName, item1.AttributeName);
		}

		public static bool Equals(ConditionExpression item0, ConditionExpression item1)
		{
			if (item0.Operator != item1.Operator)
			{
				return false;
			}

			if (item0.EntityName != item1.EntityName)
			{
				return false;
			}

			if (item0.AttributeName != item1.AttributeName)
			{
				return false;
			}

			if (item0.Values.Count != item1.Values.Count)
			{
				return false;
			}

			return item0.Values.All(t => item1.Values.Contains(t));
		}

		public static bool Equals(FilterExpression item0, FilterExpression item1)
		{
			if (item0.FilterOperator != item1.FilterOperator)
			{
				return false;
			}

			if (item0.Conditions.Count != item1.Conditions.Count)
			{
				return false;
			}

			if (item0.Filters.Count != item1.Filters.Count)
			{
				return false;
			}

			foreach (var a in item0.Conditions)
			{
				bool flag = item1.Conditions.Any(b => Equals(a, b));

				if (flag == false)
				{
					return false;
				}
			}

			foreach (var a in item0.Filters)
			{
				bool flag = item1.Filters.Any(b => Equals(a, b));

				if (flag == false)
				{
					return false;
				}
			}

			return true;
		}
	}
}