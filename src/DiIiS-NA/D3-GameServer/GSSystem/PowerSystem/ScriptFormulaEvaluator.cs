using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public static class ScriptFormulaEvaluator
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public static bool Evaluate(int powerSNO, TagKeyScript scriptTag, GameAttributeMap attributes, Random rand, out float result)
		{
			result = 0;

			ScriptFormula scriptFormula = FindScriptFormula(powerSNO, scriptTag);
			if (scriptFormula == null)
			{
				//Logger.Error("could not find script tag {0} in power {1}", scriptTag.ID, powerSNO);
				return false;
			}

			byte[] script = scriptFormula.OpCodeArray;
			Stack<float> stack = new Stack<float>(4);  // analysis of all stack formulas found the biggest stack is currently 11
			int pos = 0;
			float numb1, numb2, numb3;
			float temp;
			while (pos < script.Length)
			{
				switch (script[pos])
				{
					case 0: // return
						if (StackUnderflow(stack, 1))
							return false;
						result = stack.Pop();
						return true;

					case 1: // function
						pos += 4;
						switch (script[pos])
						{
							case 0: // Min()
								if (StackUnderflow(stack, 2))
									return false;
								numb2 = stack.Pop();
								numb1 = stack.Pop();
								stack.Push(Math.Min(numb1, numb2));
								break;

							case 1: // Max()
								if (StackUnderflow(stack, 2))
									return false;
								numb2 = stack.Pop();
								numb1 = stack.Pop();
								stack.Push(Math.Max(numb1, numb2));
								break;

							case 2: // Pin()
								if (StackUnderflow(stack, 3))
									return false;
								numb3 = stack.Pop();
								numb2 = stack.Pop();
								numb1 = stack.Pop();
								if (numb2 > numb1)
									stack.Push(numb2);
								else if (numb1 > numb3)
									stack.Push(numb3);
								else
									stack.Push(numb1);

								break;

							case 3: // RandomIntMinRange()
								if (StackUnderflow(stack, 2))
									return false;
								numb2 = stack.Pop();
								numb1 = stack.Pop();
								stack.Push(rand.Next((int)numb1, (int)numb1 + (int)numb2));
								break;

							case 4: // RandomIntMinMax()
								if (StackUnderflow(stack, 2))
									return false;
								numb2 = stack.Pop();
								numb1 = stack.Pop();
								stack.Push(rand.Next((int)numb1, (int)numb2));
								break;

							case 5: // Floor()
								if (StackUnderflow(stack, 1))
									return false;
								numb1 = stack.Pop();
								stack.Push((float)Math.Floor(numb1));
								break;

							case 9: // RandomFloatMinRange()
								if (StackUnderflow(stack, 2))
									return false;
								numb2 = stack.Pop();
								numb1 = stack.Pop();
								stack.Push(numb1 + (float)rand.NextDouble() * numb2);
								break;

							case 10: // RandomFloatMinMax()
								if (StackUnderflow(stack, 2))
									return false;
								numb2 = stack.Pop();
								numb1 = stack.Pop();
								stack.Push(numb1 + (float)rand.NextDouble() * (numb2 - numb1));
								break;

							case 11: // Table()
								if (StackUnderflow(stack, 2))
									return false;
								float index = stack.Pop();
								float tableID = stack.Pop();
								if (!LookupBalanceTable(tableID, index, out temp))
									return false;
								stack.Push(temp);
								break;

							default:
								Logger.Error("Unimplemented function");
								return false;
						}
						break;
					case 5: // external identifier
						if (!LoadIdentifier(powerSNO, scriptTag, attributes, rand,
											BitConverter.ToInt32(script, pos + 4 * 1),
											BitConverter.ToInt32(script, pos + 4 * 2),
											BitConverter.ToInt32(script, pos + 4 * 3),
											BitConverter.ToInt32(script, pos + 4 * 4),
											out temp))
							return false;

						stack.Push(temp);
						pos += 4 * 4;
						break;

					case 6: // push float
						pos += 4;
						stack.Push(BitConverter.ToSingle(script, pos));
						break;

					case 8: // operator >
						if (StackUnderflow(stack, 2))
							return false;
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 > numb2 ? 1 : 0);
						break;

					case 11: // operator +
						if (StackUnderflow(stack, 2))
							return false;
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 + numb2);
						break;

					case 12: // operator -
						if (StackUnderflow(stack, 2))
							return false;
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 - numb2);
						break;

					case 13: // operator *
						if (StackUnderflow(stack, 2))
							return false;
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 * numb2);
						break;

					case 14: // operator /
						if (StackUnderflow(stack, 2))
							return false;
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						if (numb2 == 0f)
						{
							Logger.Error("Division by zero, 0 pushed to stack instead of divide result");
							stack.Push(0f);
						}
						else
						{
							stack.Push(numb1 / numb2);
						}
						break;

					case 16: // operator -(unary)
						if (StackUnderflow(stack, 1))
							return false;
						numb1 = stack.Pop();
						stack.Push(-numb1);
						break;

					case 17: // operator ?:
						if (StackUnderflow(stack, 3))
							return false;
						numb3 = stack.Pop();
						numb2 = stack.Pop();
						numb1 = stack.Pop();
						stack.Push(numb1 != 0 ? numb2 : numb3);
						break;

					default:
						Logger.Error("Unimplemented OpCode({0})", script[pos]);
						return false;
				}
				pos += 4;
			}

			// HACK: ignore bad formula
			if (powerSNO == SkillsSystem.Skills.Barbarian.FurySpenders.Whirlwind &&
				scriptTag.ID == 266560) // ScriptFormula(4)
			{
				return true;
			}

			Logger.Error("script finished without return opcode");
			return false;
		}

		private static bool StackUnderflow(Stack<float> stack, int popcount)
		{
			if (stack.Count < popcount)
			{
				Logger.Error("Stack underflow");
				return true;
			}
			return false;
		}

		private static float BinaryIntToFloat(int n)
		{
			byte[] array = BitConverter.GetBytes(n);
			return BitConverter.ToSingle(array, 0);
		}

		private static bool LoadIdentifier(int powerSNO, TagKeyScript scriptTag, GameAttributeMap attributes, Random rand,
										   int numb1, int numb2, int numb3, int numb4,
										   out float result)
		{
			switch (numb1)
			{
				case 0:
					return LoadAttribute(powerSNO, attributes, numb2, out result);

				case 1: // slevel
					result = attributes[GameAttribute.Skill, powerSNO];
					return true;

				case 22: // absolute power formula ref
				case 94:
					return Evaluate(numb2, new TagKeyScript(numb3), attributes, rand, out result);
				default:
					if (numb1 >= 23 && numb1 <= 62) // SF_N, relative power formula ref
					{
						int SF_N = numb1 - 23;
						TagKeyScript relativeTag = PowerTagHelper.GenerateTagForScriptFormula(SF_N);
						return Evaluate(powerSNO, relativeTag, attributes, rand, out result);
					}
					else if (numb1 >= 63 && numb1 <= 113) // known gamebalance power table id range
					{
						result = BinaryIntToFloat(numb1); // simply store id, used later by Table()
						return true;
					}
					else
					{
						Logger.Error("unknown identifier: num1 {0}", numb1);
						result = 0;
						return false;
					}
			}
		}

		// this lists the attributes that need to be keyed with the powerSNO to work
		private static readonly SortedSet<int> _powerKeyedAttributes = new SortedSet<int>()
		{
			GameAttribute.Rune_A.Id,
			GameAttribute.Rune_B.Id,
			GameAttribute.Rune_C.Id,
			GameAttribute.Rune_D.Id,
			GameAttribute.Rune_E.Id,
			GameAttribute.Buff_Icon_Count0.Id,
			GameAttribute.Buff_Icon_Count1.Id,
			GameAttribute.Buff_Icon_Count2.Id,
			GameAttribute.Buff_Icon_Count3.Id,
			GameAttribute.Buff_Icon_Count4.Id,
			GameAttribute.Buff_Icon_Count5.Id,
			GameAttribute.Buff_Icon_Count6.Id,
			GameAttribute.Buff_Icon_Count7.Id,
		};

		private static bool LoadAttribute(int powerSNO, GameAttributeMap attributes, int attributeId, out float result)
		{
			GameAttribute attr = GameAttribute.Attributes[attributeId];
			bool needs_key = _powerKeyedAttributes.Contains(attributeId);

			if (attr is GameAttributeF)
			{
				if (needs_key) result = attributes[(GameAttributeF)attr, powerSNO];
				else result = attributes[(GameAttributeF)attr];

				return true;
			}
			else if (attr is GameAttributeI)
			{
				if (needs_key) result = (float)attributes[(GameAttributeI)attr, powerSNO];
				else result = (float)attributes[(GameAttributeI)attr];

				return true;
			}
			else if (attr is GameAttributeB)
			{
				if (needs_key) result = attributes[(GameAttributeB)attr, powerSNO] ? 1 : 0;
				else result = attributes[(GameAttributeB)attr] ? 1 : 0;

				return true;
			}
			else
			{
				Logger.Error("invalid attribute {0}", attributeId);
				result = 0;
				return false;
			}
		}

		private static ScriptFormula FindScriptFormula(int powerSNO, TagKeyScript scriptTag)
		{
			TagMap tagmap = PowerTagHelper.FindTagMapWithKey(powerSNO, scriptTag);
			if (tagmap != null)
				return tagmap[scriptTag];
			else
				return null;
		}

		private static bool LookupBalanceTable(float tableId, float index, out float result)
		{
			result = 0;

			int tableByte = BitConverter.GetBytes(tableId)[0];
			string tableName = GetTableName(tableByte);
			if (tableName == null)
				return false;

			foreach (GameBalance gb in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values.Where(a => a.Data != null)
																						  .Select(a => a.Data)
																						  .Cast<GameBalance>())
			{
				if (gb.PowerFormula != null)
				{
					foreach (var powerEntry in gb.PowerFormula)
					{
						if (powerEntry.S0 == tableName)
						{
							result = powerEntry.F0[(int)index];
							return true;
						}
					}
				}
			}

			Logger.Error("could not find table {0}", tableName);
			return false;
		}

		private static string GetTableName(int tableId)
		{
			switch (tableId)
			{
				case 63:
					return "DmgTier1";
				case 64:
					return "DmgTier2";
				case 65:
					return "DmgTier3";
				case 66:
					return "DmgTier4";
				case 67:
					return "DmgTier5";
				case 68:
					return "DmgTier6";
				case 69:
					return "DmgTier7";
				case 70:
					return "Healing";
				case 71:
					return "WDCost";
				case 72:
					return "RuneDamageBonus";
				default:
					Logger.Error("Unknown table id {0}", tableId);
					return null;
			}
		}
	}
}
