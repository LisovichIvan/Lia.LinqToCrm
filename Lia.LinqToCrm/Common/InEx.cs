using System.Text;
using System.Threading.Tasks;

namespace Lia.LinqToCrm.Common
{
	public static class InEx
	{
		public static bool In<T>(this T item, params T[] items) where T : class
		{
			foreach (T it in items)
			{
				if (item == it)
				{
					return true;
				}
			}

			return false;
		}

		public static bool In<T>(this T item, T item0, T item1) where T : class
		{
			if (item == item0)
			{
				return true;
			}

			if (item == item1)
			{
				return true;
			}

			return false;
		}

	}
}
