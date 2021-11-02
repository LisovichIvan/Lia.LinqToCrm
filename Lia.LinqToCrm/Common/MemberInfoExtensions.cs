using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Lia.LinqToCrm.Common
{
	internal static class MemberInfoExtensions
	{
		private static readonly ConcurrentDictionary<MemberInfoAttributeType, IEnumerable<object>> _memberInfoToAttributesLookup = new ConcurrentDictionary<MemberInfoAttributeType, IEnumerable<object>>();

		public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo info) where T : Attribute
		{
			MemberInfoAttributeType key = new MemberInfoAttributeType()
			{
				MemberInfo = info,
				Type = typeof (T)
			};
			return _memberInfoToAttributesLookup.GetOrAdd(key, _ => info.GetCustomAttributes(typeof (T), true)).Cast<T>();
		}

		public static T GetFirstOrDefaultCustomAttribute<T>(this MemberInfo info) where T : Attribute
		{
			return info.GetCustomAttributes<T>().FirstOrDefault<T>();
		}

		public static string GetLogicalName(this MemberInfo property)
		{
			AttributeLogicalNameAttribute defaultCustomAttribute1 = property.GetFirstOrDefaultCustomAttribute<AttributeLogicalNameAttribute>();
			if (defaultCustomAttribute1 != null)
			{
				return defaultCustomAttribute1.LogicalName;
			}
			EntityLogicalNameAttribute defaultCustomAttribute2 = property.GetFirstOrDefaultCustomAttribute<EntityLogicalNameAttribute>();
			if (defaultCustomAttribute2 != null)
			{
				return defaultCustomAttribute2.LogicalName;
			}

			return property.GetFirstOrDefaultCustomAttribute<RelationshipSchemaNameAttribute>()?.SchemaName;
		}

		private struct MemberInfoAttributeType
		{
			public MemberInfo MemberInfo;
			public Type Type;
		}
	}
}