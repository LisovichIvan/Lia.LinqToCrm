using NUnit.Framework;

namespace Lia.LinqToCrm.Tests
{
	public class LinqTests
	{
		static ICrmQueryable<int> a = null;
		static ICrmQueryable<int> b = null;

		[SetUp]
		public void Setup() { }

		[Test]
		public void TakeSkip()
		{
			var temp = a.Take(10).Take(10);
			var temp0 = a.Skip(10).Skip(10);
		}

		[Test]
		public void Where_L()
		{
			var temp1 =
				from a1 in a
				where a1 > 10
				select a1;
		}

		[Test]
		public void Inner_Join_Where_L()
		{
			var temp1 =
				from a1 in a
				join b1 in b on a1 equals b1
				where a1 > 10 && b1 > 10
				select a1;
		}

		[Test]
		public void Inner_Join_Where_M()
		{
			var temp1 = a
				.Join(b, a1 => a1, b1 => b1, (a1, b1) => new {a1, b1})
				.Where(t1 => t1.a1 > 10 && t1.b1 > 10)
				.Select(t1 => t1.a1);
		}

		[Test]
		public void Inner_Join_L()
		{
			var temp1 =
				from a1 in a
				join b1 in b on a1 equals b1
				select a1;
		}

		[Test]
		public void Inner_Join_M()
		{
			var temp1 = a
				.Join(b, a1 => a1, b1 => b1, (a1, b1) => a1);
		}

		[Test]
		public void Left_Join_L()
		{
			var temp =
				from a1 in a
				join b1 in b on a1 equals b1 into b11
				from b111 in b11.DefaultIfEmpty()
				select new {a1, b111};
		}
		
		[Test]
		public void Left_Join_M()
		{
			var temp = a
				.GroupJoin(b, a1 => a1, b1 => b1, (a1, b11) => new {a1, b11})
				.SelectMany(@t => @t.b11.DefaultIfEmpty(), (@t, b111) => new {@t.a1, b111});		
		}
	}
}