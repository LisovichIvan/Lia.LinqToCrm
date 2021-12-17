using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Lia.LinqToCrm.Provider
{
	internal static class PagingCookieHelper
	{
		public static object[] ToContinuationToken(string pagingCookie, int pageNumber)
		{
			return Deserialize(pagingCookie, pageNumber).ToArray();
		}

		public static string ToPagingCookie(object[] continuationToken, out int pageNumber)
		{
			return Serialize(continuationToken, out pageNumber);
		}

		private static List<object> Deserialize(string pagingCookie, int pageNumber)
		{
			ClientExceptionHelper.ThrowIfNegative(pageNumber, nameof(pageNumber));
			List<object> objectList = new List<object>();
			try
			{
				using (XmlReader xmlReader = CreateXmlReader(pagingCookie))
				{
					xmlReader.Read();
					objectList.Add(pageNumber);
					string attribute1 = xmlReader.GetAttribute("parentEntityId");
					if (!string.IsNullOrEmpty(attribute1))
					{
						objectList.Add(new Guid(attribute1));
						string attribute2 = xmlReader.GetAttribute("parentAttributeName");
						ClientExceptionHelper.ThrowIfNullOrEmpty(attribute2, "parentAttributeName");
						objectList.Add(attribute2);
						if (int.TryParse(xmlReader.GetAttribute("parentEntityObjectTypeCode"), out var result))
						{
							objectList.Add(result);
						}
						else
						{
							ClientExceptionHelper.ThrowIfNegative(result, "parentOtc");
						}
					}
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType != XmlNodeType.EndElement)
						{
							string name = xmlReader.Name;
							ClientExceptionHelper.ThrowIfNullOrEmpty(name, "field");
							objectList.Add(name);
							if (xmlReader.AttributeCount != 2)
							{
								throw new NotSupportedException("Malformed XML Passed to in the Paging Cookie. We expect at most two attributes (first/firstNull and last/lastNull)");
							}

							string attribute2 = xmlReader.GetAttribute("last");
							if (attribute2 == null)
							{
								if (xmlReader.GetAttribute("lastnull") == null)
								{
									throw new NotSupportedException("Malformed XML Passed to in the Paging Cookie. Value for attribute last was not specified, and it was not null either.");
								}

								objectList.Add(null);
							}
							else
							{
								objectList.Add(attribute2);
							}

							string attribute3 = xmlReader.GetAttribute("first");
							if (attribute3 == null)
							{
								if (xmlReader.GetAttribute("firstnull") == null)
								{
									throw new NotSupportedException("Malformed XML Passed to in the Paging Cookie. Value for attribute first was not specified, and it was not null either.");
								}

								objectList.Add(null);
							}
							else
							{
								objectList.Add(attribute3);
							}
						}
					}
				}
			}
			catch (XmlException ex)
			{
				throw new NotSupportedException("Malformed XML in the Paging Cookie", ex);
			}
			catch (FormatException ex)
			{
				throw new NotSupportedException("Malformed XML in the Paging Cookie", ex);
			}
			return objectList;
		}

		private static string Serialize(object[] pagingElements, out int pageNumber)
		{
			pageNumber = 0;
			if (pagingElements == null || pagingElements.Length == 0)
			{
				return null;
			}

			if (pagingElements.Length % 3 != 1)
			{
				throw new NotSupportedException("Skip token has incorrect length");
			}

			if (pagingElements[0] == null || !(pagingElements[0] is int) || (int)pagingElements[0] < 0)
			{
				throw new NotSupportedException("Skip token has incorrect page value");
			}

			pageNumber = (int)pagingElements[0];
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (XmlWriter xmlWriter = CreateXmlWriter(stringWriter))
				{
					xmlWriter.WriteStartElement("cookie");
					xmlWriter.WriteAttributeString("page", pageNumber.ToString(CultureInfo.InvariantCulture));
					int num = 1;
					if (pagingElements[1] != null && pagingElements[1] is Guid && (pagingElements[2] != null && pagingElements[2] is string) && (pagingElements[3] != null && pagingElements[3] is int))
					{
						num = 4;
						xmlWriter.WriteAttributeString("parentEntityId", pagingElements[1].ToString());
						xmlWriter.WriteAttributeString("parentAttributeName", (string)pagingElements[2]);
						xmlWriter.WriteAttributeString("parentEntityObjectTypeCode", pagingElements[3].ToString());
					}
					for (int index = num; index < pagingElements.Length; index += 3)
					{
						string pagingElement = (string) pagingElements[index];
						ClientExceptionHelper.ThrowIfNullOrEmpty(pagingElement, "attributeName");
						string str1 = (string) pagingElements[index + 1];
						string str2 = (string) pagingElements[index + 2];
						xmlWriter.WriteStartElement(pagingElement);
						string localName1;
						if (str1 != null)
						{
							localName1 = "last";
						}
						else
						{
							localName1 = "lastnull";
							str1 = "1";
						}
						string localName2;
						if (str2 != null)
						{
							localName2 = "first";
						}
						else
						{
							localName2 = "firstnull";
							str2 = "1";
						}
						xmlWriter.WriteAttributeString(localName1, str1);
						xmlWriter.WriteAttributeString(localName2, str2);
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
				}
				return stringWriter.ToString();
			}
		}

		private static XmlWriter CreateXmlWriter(TextWriter textWriter)
		{
			return XmlWriter.Create(textWriter, new XmlWriterSettings()
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			});
		}

		[SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "StringReader required for life of XmlReader")]
		private static XmlReader CreateXmlReader(string xml)
		{
			return XmlReader.Create(new StringReader(xml), new XmlReaderSettings()
			{
				IgnoreWhitespace = true
			});
		}
	}
}