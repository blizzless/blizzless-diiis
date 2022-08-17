//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.Implementations
{
	//[HandledType("SpellRune")]
	public class SpellRune : Item
	{
		// type of rune is in Name
		// Attributes[GameAttribute.Rune_<x>] = <rank>; // on attuned runes ONLY
		// Attributes[GameAttribute.Rune_Rank] = <in spec>; // on unattuned rune ONLY, inititalized in creation
		// Attributes[GameAttribute.Rune_Attuned_Power] = 0; // need s to be 0 on unattuned or random value from all powers

		public static readonly Logger Logger = LogManager.CreateLogger();

		public SpellRune(World world, DiIiS_NA.Core.MPQ.FileFormats.GameBalance.ItemTable definition, int cork = -1, bool cork2 = false, int cork3 = -1)
			: base(world, definition)
		{
			if (!definition.Name.Contains("X"))
			{
				// attuned rune, randomize power
				int classRnd = RandomHelper.Next(0, 5);
				int PowerSNOId = -1;
				switch (classRnd)
				{
					case 0:
						PowerSNOId = SkillsSystem.Skills.Barbarian.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Barbarian.AllActiveSkillsList.Count));
						break;
					case 1:
						PowerSNOId = SkillsSystem.Skills.DemonHunter.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.DemonHunter.AllActiveSkillsList.Count));
						break;
					case 2:
						PowerSNOId = SkillsSystem.Skills.Monk.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Monk.AllActiveSkillsList.Count));
						break;
					case 3:
						PowerSNOId = SkillsSystem.Skills.WitchDoctor.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.WitchDoctor.AllActiveSkillsList.Count));
						break;
					case 4:
						PowerSNOId = SkillsSystem.Skills.Wizard.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Wizard.AllActiveSkillsList.Count));
						break;
					case 5:
						PowerSNOId = SkillsSystem.Skills.Crusader.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Crusader.AllActiveSkillsList.Count));
						break;
					case 6:
						PowerSNOId = SkillsSystem.Skills.Necromancer.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Necromancer.AllActiveSkillsList.Count));
						break;
				}
				//this.Attributes[GameAttribute.Rune_Attuned_Power] = PowerSNOId;
			}
		}

		/// <summary>
		/// Re-attunes rune to player's class. Used for favoring.
		/// </summary>
		/// <param name="toonClass"></param>
		public void ReAttuneToClass(ToonClass toonClass)
		{
			int PowerSNOId = -1;
			switch (toonClass)
			{
				case ToonClass.Barbarian:
					PowerSNOId = SkillsSystem.Skills.Barbarian.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Barbarian.AllActiveSkillsList.Count));
					break;
				case ToonClass.DemonHunter:
					PowerSNOId = SkillsSystem.Skills.DemonHunter.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.DemonHunter.AllActiveSkillsList.Count));
					break;
				case ToonClass.Monk:
					PowerSNOId = SkillsSystem.Skills.Monk.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Monk.AllActiveSkillsList.Count));
					break;
				case ToonClass.WitchDoctor:
					PowerSNOId = SkillsSystem.Skills.WitchDoctor.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.WitchDoctor.AllActiveSkillsList.Count));
					break;
				case ToonClass.Wizard:
					PowerSNOId = SkillsSystem.Skills.Wizard.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Wizard.AllActiveSkillsList.Count));
					break;
				case ToonClass.Crusader:
					PowerSNOId = SkillsSystem.Skills.Wizard.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Wizard.AllActiveSkillsList.Count));
					break;
				case ToonClass.Necromancer:
					PowerSNOId = SkillsSystem.Skills.Wizard.AllActiveSkillsList.ElementAt(RandomHelper.Next(0, SkillsSystem.Skills.Wizard.AllActiveSkillsList.Count));
					break;
			}
			//this.Attributes[GameAttribute.Rune_Attuned_Power] = PowerSNOId;
		}
	}
}
