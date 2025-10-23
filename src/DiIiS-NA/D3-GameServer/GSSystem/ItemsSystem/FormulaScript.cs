using DiIiS_NA.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
	public static class FormulaScript
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		private static float BinaryIntToFloat(int n)
		{
			//byte[] array = BitConverter.GetBytes(n);
			//return BitConverter.ToSingle(array, 0);
			return BitConverter.Int32BitsToSingle(n);
		}

		public static bool Evaluate(int[] script, ItemRandomHelper irh, out float result)
		{
			float minValue;
			float maxValue;
			return Evaluate(script, irh, out result, out minValue, out maxValue);
		}

		public static bool Evaluate(int[] script, ItemRandomHelper irh, out float result, out float minValue, out float maxValue)
		{
			result = 0;
			Stack<float> stack = new Stack<float>(64);
			int pos = 0;
			float numb1, numb2; //numb3;
			minValue = 0f;
			maxValue = 0f;
			while (pos < script.Length)
			{
				switch ((byte)script[pos])
				{
					case 0:
						if (stack.Count < 1)
						{
							Logger.Error("Stack underflow");
							return false;
						}
						result = stack.Pop();
						return true;
					case 1:
						++pos;
						byte funcId = (byte)script[pos];
						switch (funcId)
						{
							case 3:
								if (stack.Count < 2)
								{
									Logger.Error("Stack underflow");
									return false;
								}
								numb2 = stack.Pop();
								numb1 = minValue = stack.Pop();
								maxValue = numb1 + numb2;
								stack.Push(irh.Next(numb1, numb1 + numb2));
								break;
							case 4:
								if (stack.Count < 2)
								{
									Logger.Error("Stack underflow");
									return false;
								}
								numb2 = maxValue = stack.Pop();
								numb1 = minValue = stack.Pop();
								stack.Push(irh.Next(numb1, numb2));
								break;
							default:
								Logger.Error("Unimplemented function");
								return false;
						}
						break;
					case 5:
						++pos;
						stack.Push(BinaryIntToFloat(script[pos]));
						break;
					case 6:
						++pos;
						stack.Push(BinaryIntToFloat(script[pos]));
						break;
					case 11:
						if (stack.Count < 2)
						{
							Logger.Error("Stack underflow");
							return false;
						}
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 + numb2);
						break;
					case 12:
						if (stack.Count < 2)
						{
							Logger.Error("Stack underflow");
							return false;
						}
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 - numb2);
						break;
					case 13:
						if (stack.Count < 2)
						{
							Logger.Error("Stack underflow");
							return false;
						}
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 * numb2);
						break;
					case 14:
						if (stack.Count < 2)
						{
							Logger.Error("Stack underflow");
							return false;
						}
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						if (numb1 == 0f)
						{
							Logger.Error("Division by zero");
							return false;
						}
						stack.Push(numb1 / numb2);
						minValue /= numb2;
						maxValue /= numb2;
						break;
					default:
						Logger.Error("Unimplemented OpCode {0}", (byte)script[pos]);
						return false;
				}
				++pos;
			}
			return false;
		}

		public static string ToString(int[] script)
		{
			StringBuilder b = new StringBuilder();
			int pos = 0;
			while (pos < script.Length)
			{
				switch ((byte)script[pos])
				{
					case 0:
						b.Append("return; ");
						break;
					case 1:
						++pos;
						ToStringFunc(b, (byte)script[pos]);
						break;
					case 5:
						ToStringIdentifierEval(
							b,
							script[pos + 1],
							script[pos + 2],
							script[pos + 3],
							script[pos + 4]);
						++pos;
						break;
					case 6:
						b.Append("push ");
						++pos;
						b.Append(BinaryIntToFloat(script[pos]));
						b.Append("; ");
						break;
					case 11:
						b.Append("add; ");
						break;
					case 12:
						b.Append("sub; ");
						break;
					case 13:
						b.Append("mul; ");
						break;
					case 14:
						b.Append("div; ");
						break;
					default:
						b.Append("unknownOp (");
						b.Append(script[pos]);
						b.Append("); ");
						break;
				}
				++pos;
			}
			return b.ToString();
		}

		static void ToStringFunc(StringBuilder b, byte funcId)
		{
			switch (funcId)
			{
				case 0:
					b.Append("func Min(stack1, stack0); ");
					break;
				case 1:
					b.Append("func Max(stack1, stack0); ");
					break;
				case 2:
					b.Append("func Pin(stack2, stack1, stack0); ");
					break;
				case 3:
					b.Append("func RandomIntMinRange(stack1, stack0); ");
					break;
				case 4:
					b.Append("func RandomIntMinMax(stack1, stack0); ");
					break;
				case 5:
					b.Append("func Floor(stack0); ");
					break;
				case 6:
					b.Append("func Dim(stack2, stack1, stack0); ");
					break;
				case 7:
					b.Append("func Pow(stack1, stack0); ");
					break;
				case 8:
					b.Append("func Log(stack0); ");
					break;
				case 9:
					b.Append("func RandomFloatMinRange(stack1, stack0); ");
					break;
				case 10:
					b.Append("func RandomFloatMinMax(stack1, stack0); ");
					break;
				case 11:
					b.Append("func TableLookup(stack1, stack0); ");
					break;
				default:
					b.Append("unknownFunc(");
					b.Append(funcId);
					b.Append("); ");
					break;
			}
		}

		static void ToStringIdentifierEval(StringBuilder b, int numb1, int numb2, int numb3, int numb4)
		{
			switch (numb1)
			{
				case 0:
					b.Append("GetAttribute ");
					b.Append(GameAttributes.Attributes[numb2].Name);
					b.Append(" ("); b.Append(numb2); b.Append("); ");
					break;
				case 22:
					b.Append("RunScript ... ; ");
					break;
				default:
					b.Append("unknown source on identifier function ("); b.Append(numb1); b.Append("); ");
					break;
			}
		}
	}
}
