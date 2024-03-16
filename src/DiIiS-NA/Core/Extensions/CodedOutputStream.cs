using System;

namespace DiIiS_NA.Core.Extensions
{
	public static class CodedOutputStreamExtensions
	{
		public static void WriteInt16NoTag(this Google.ProtocolBuffers.CodedOutputStream s, short value)
		{
			s.WriteRawBytes(BitConverter.GetBytes(value));
		}
	}
}
