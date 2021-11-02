using System;

namespace Lia.LinqToCrm.Common
{
	internal static class TypeExtensions
	{
		public static Type GetUnderlyingType(this Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return (object) underlyingType != null ? underlyingType : type;
		}
	}
}