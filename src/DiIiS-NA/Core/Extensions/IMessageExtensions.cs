//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using Google.ProtocolBuffers.Descriptors;

namespace DiIiS_NA.Core.Extensions
{
	static class IMessageExtensions
	{

		static void AppendLevel(StringBuilder result, int level)
		{
			for (int i = 0; i < level; i++) result.Append(' ');
		}

		static void AppendLine(StringBuilder result, int level, string str) { for (int i = 0; i < level; i++) result.Append(' '); result.AppendLine(str); }
		static char ToHexChar(byte b)
		{
			return (b >= 0x20 && b < 0x80) ? (char)b : '.';
		}

		static void AppendHexdump(StringBuilder b, int level, byte[] buffer)
		{
			b.AppendLine();
			AppendLine(b, level++, "[0x" + buffer.Length.ToString("X8") + "] {");

			int length = Math.Min(buffer.Length, 0xFFFF);
			for (int i = 0; i < length; i += 16)
			{

				AppendLevel(b, level);

				b.AppendFormat("{0:X4}  ", i);
				for (int n = 0; n < 8; n++)
				{
					int o = i + n;
					if (o < length)
						b.AppendFormat("{0:X2} ", buffer[o]);
					else
						b.Append("   ");
				}
				b.Append(" ");
				for (int n = 0; n < 8; n++)
				{
					int o = i + n + 8;
					if (o < length)
						b.AppendFormat("{0:X2} ", buffer[o]);
					else
						b.Append("   ");
				}
				b.Append(" ");

				for (int n = 0; n < 8; n++)
				{
					int o = i + n;
					if (o < length)
						b.AppendFormat("{0}", ToHexChar(buffer[o]));
					else
						b.Append(" ");
				}
				b.Append(" ");
				for (int n = 0; n < 8; n++)
				{
					int o = i + n + 8;
					if (o < length)
						b.AppendFormat("{0}", ToHexChar(buffer[o]));
					else
						b.Append(" ");
				}
				b.AppendLine();
			}
			AppendLine(b, --level, "}");
		}

		static void AppendFieldValue(StringBuilder result, int level, FieldType type, object value)
		{
			switch (type)
			{
				case FieldType.Bool:
					result.AppendLine((bool)value ? "true" : "false");
					break;
				case FieldType.Bytes:
					result.AppendLine(EscapeBytes((ByteString)value));
					AppendHexdump(result, level, ((ByteString)value).ToByteArray());
					break;
				case FieldType.Double:
					result.AppendLine(((double)value).ToString("G"));
					break;
				case FieldType.Float:
					result.AppendLine(((float)value).ToString("G"));
					break;
				case FieldType.Int32:
				case FieldType.SFixed32:
				case FieldType.SInt32:
					result.AppendLine(value.ToString());
					break;
				case FieldType.Int64:
				case FieldType.SFixed64:
				case FieldType.SInt64:
					result.AppendLine("0x" + ((long)value).ToString("X16") + "l (" + ((long)value) + ")");
					break;
				case FieldType.Fixed32:
				case FieldType.UInt32:
					result.AppendLine("0x" + ((uint)value).ToString("X8") + "u (" + ((uint)value) + ")");
					break;
				case FieldType.Fixed64:
				case FieldType.UInt64:
					result.AppendLine("0x" + ((ulong)value).ToString("X16") + "ul (" + ((ulong)value) + ")");
					break;
				case FieldType.Message:
					result.AppendLine();
					AppendMessage(result, level, (IMessage)value);
					break;
				case FieldType.Enum:
					{
						EnumValueDescriptor e = (EnumValueDescriptor)value;

						result.AppendLine(e.Name);
					}
					break;
				case FieldType.String:
					result.AppendLine(value.ToString());
					break;
				case FieldType.Group:
				default:
					throw new Exception("Unhandled FieldType");
			}
		}
		static void AppendField(StringBuilder result, int level, FieldDescriptor fieldDesc, object value)
		{
			if (value == null)
				return;
			if (fieldDesc.IsRepeated)
			{
				var e = ((System.Collections.IEnumerable)value).GetEnumerator();
				while (e.MoveNext())
				{
					AppendLevel(result, level);
					result.Append(fieldDesc.Name);
					result.Append(": ");
					AppendFieldValue(result, level, fieldDesc.FieldType, e.Current);
				}
			}
			else
			{
				AppendLevel(result, level);
				result.Append(fieldDesc.Name);
				result.Append(": ");
				AppendFieldValue(result, level, fieldDesc.FieldType, value);
			}
		}

		static void AppendMessage(StringBuilder result, int level, IMessage msg)
		{
			AppendLine(result, level++, "{");
			var fields = msg.AllFields;
			foreach (var pair in fields)
				AppendField(result, level, pair.Key, pair.Value);
			AppendLine(result, --level, "}");
		}

		public static string AsText(this IMessage msg)
		{
			var msgDesc = msg.DescriptorForType;
			StringBuilder result = new StringBuilder();
			result.AppendLine(msgDesc.FullName);
			AppendMessage(result, 0, msg);
			return result.ToString();
		}

		static String EscapeBytes(ByteString input)
		{
			StringBuilder builder = new StringBuilder(input.Length);
			foreach (byte b in input)
			{
				switch (b)
				{
					// C# does not use \a or \v
					case 0x07: builder.Append("\\a"); break;
					case (byte)'\b': builder.Append("\\b"); break;
					case (byte)'\f': builder.Append("\\f"); break;
					case (byte)'\n': builder.Append("\\n"); break;
					case (byte)'\r': builder.Append("\\r"); break;
					case (byte)'\t': builder.Append("\\t"); break;
					case 0x0b: builder.Append("\\v"); break;
					case (byte)'\\': builder.Append("\\\\"); break;
					case (byte)'\'': builder.Append("\\\'"); break;
					case (byte)'"': builder.Append("\\\""); break;
					default:
						if (b >= 0x20 && b < 128)
						{
							builder.Append((char)b);
						}
						else
						{
							builder.Append('\\');
							builder.Append((char)('0' + ((b >> 6) & 3)));
							builder.Append((char)('0' + ((b >> 3) & 7)));
							builder.Append((char)('0' + (b & 7)));
						}
						break;
				}
			}
			return builder.ToString();
		}
	}
}
