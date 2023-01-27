using System.Collections.Generic;
using System.Reflection;

namespace DiIiS_NA.GameServer.Core.Types.TagMap
{
	public class MarkerKeys
	{
		#region compile a dictionary to access keys from ids. If you need a readable name for a TagID, look up its key and get its name
		private static Dictionary<int, TagKey> tags = new Dictionary<int, TagKey>();

		public static TagKey GetKey(int index)
		{
			return tags.ContainsKey(index) ? tags[index] : null;
		}

		static MarkerKeys()
		{
			foreach (FieldInfo field in typeof(MarkerKeys).GetFields())
			{
				TagKey key = field.GetValue(null) as TagKey;
				key.Name = field.Name;
				tags.Add(key.ID, key);
			}
		}
		#endregion

		//524864 == hasinteractionoptions?

		public static TagKeySNO QuestRange = new TagKeySNO(524544);
		public static TagKeySNO QuestRange2 = new TagKeySNO(544768);
		public static TagKeyInt AdventureModeOnly = new TagKeyInt(2054);
		public static TagKeyInt StoryModeOnly = new TagKeyInt(526341);
		public static TagKeyInt RiftOnly = new TagKeyInt(2055);
		public static TagKeySNO ConversationList = new TagKeySNO(526080);
		public static TagKeyFloat Scale = new TagKeyFloat(524288);
		public static TagKeySNO OnActorSpawnedScript = new TagKeySNO(524808);
		//TODO: Thes probably should be under actor keys...
		public static TagKeyInt Group1Hash = new TagKeyInt(524814);
		public static TagKeyInt Group2Hash = new TagKeyInt(524815);
		public static TagKeySNO SpawnActor = new TagKeySNO(532496);

		public static TagKeySNO BossEncounter = new TagKeySNO(66477);
		public static TagKeySNO BossEncounterSnoLevelArea = new TagKeySNO(60714);

		// Used for portal destination resolution
		public static TagKeySNO DestinationWorld = new TagKeySNO(526850);
		public static TagKeyInt DestinationActorTag = new TagKeyInt(526851);
		public static TagKeyInt ActorTag = new TagKeyInt(526852);
		public static TagKeySNO DestinationLevelArea = new TagKeySNO(526853);

		public static TagKeyInt SavepointId = new TagKeyInt(526976);

		public static TagKeySNO TriggeredConversation = new TagKeySNO(528128);
		public static TagKeySNO TriggeredConversation1 = new TagKeySNO(528129);
		public static TagKeyInt TriggerDistance = new TagKeyInt(528384); // raven_pecking
		public static TagKeySNO TriggeredActor = new TagKeySNO(526592);

		public static TagKeySNO MinimapTexture = new TagKeySNO(548864);
	}
}
