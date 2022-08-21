//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect
{
    [Message(Opcodes.PlayEffectMessage)]
    public class PlayEffectMessage : GameMessage
    {
        public uint ActorId; // Actor's DynamicID
        public Effect Effect;
        public int? OptionalParameter;
        public int? PlayerId;

        public PlayEffectMessage() : base(Opcodes.PlayEffectMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
            Effect = (Effect)buffer.ReadInt(7) + (-1);
            if (buffer.ReadBool())
            {
                OptionalParameter = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                PlayerId = buffer.ReadInt(32);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
            buffer.WriteInt(7, (int)Effect - (-1));
            buffer.WriteBool(OptionalParameter.HasValue);
            if (OptionalParameter.HasValue)
            {
                buffer.WriteInt(32, OptionalParameter.Value);
            }
            buffer.WriteBool(PlayerId.HasValue);
            if (PlayerId.HasValue)
            {
                buffer.WriteInt(32, PlayerId.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayEffectMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorId.ToString("X8") + " (" + ActorId + ")");
            b.Append(' ', pad); b.AppendLine("Effect: 0x" + ((int)Effect).ToString("X8") + " (" + Effect + ")");
            if (OptionalParameter.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("OptionalParameter.Value: 0x" + OptionalParameter.Value.ToString("X8") + " (" + OptionalParameter.Value + ")");
            }
            if (PlayerId.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("PlayerId.Value: 0x" + PlayerId.Value.ToString("X8") + " (" + PlayerId.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

    public enum Effect
    {
		Hit = 0,                        // plays a hit sound, takes another parameter for the specific sound
		Unknown1 = 1,                   // character becomes dark(wtf?)
		Unknown2 = 2,                  // Used on goldcoins, healthglobes...items				  
		Unknown3 = 3,                  // Used on goldcoins, healthglobes...items
		UnknownSound = 4,              // Weird sound effect...pickup sound?  only used on PlayerActors

		/// <summary>
		/// Gold pickup (golden glow)
		/// </summary>
		GoldPickup = 5,

		/// <summary>
		/// Level up message (sign) in the center of the screen
		/// TODO: it's not level-up, it's current player level info (including unlocked skills/slots)
		/// </summary>
		LevelUp = 6,

		/// <summary>
		/// Health orb pickup (red imapct effect and noise)
		/// </summary>
		HealthOrbPickup = 7,

		/// <summary>
		/// violet / pink circular effect on the actor
		/// </summary>
		ArcanePowerGain = 8,

		/// <summary>
		/// Holy effect
		/// TODO find out what that is and give the enum a proper name
		/// </summary>
		Holy1 = 9,

		/// <summary>
		/// Holy effect
		/// TODO find out what that is and give the enum a proper name
		/// </summary>
		Holy2 = 10,

		Unknown11 = 11,              // played on chests, switches, spawner ...
		Unknown12 = 12,              // played on a lot of things

		/// <summary>
		/// Breathing effect. Takes a particle effect sno as parameter. See comments
		/// TODO find out if that is the same as Breathing2
		/// </summary>
		Breathing1 = 13,          // OOOkay... you CAN play this with a particle sno in field2 but i dont know if that is the intention. The place suggests its a breathing effect like 27958

		/// <summary>
		/// Sound effect. Takes a sound sno as parameter
		/// </summary>
		Sound = 14,

		Unknown15 = 15,



		/// <summary>
		/// Breathing effect. Takes a particle effect sno as parameter.
		/// TODO find out if that is the same as Breathing1
		/// </summary>
		Breathing2 = 16,          // same as BreathingEffect1

		/// <summary>
		/// Plays a sound and shows a text informing the player he cannot carry any more
		/// TODO find out if that is the same as PickupFailOverburden2
		/// </summary>
		PickupFailOverburden1 = 17,

		/// <summary>
		/// Plays a sound and shows a text informing the player he cannot carry any more
		/// TODO find out if that is the same as PickupFailOverburden1
		/// </summary>
		PickupFailOverburden2 = 18,  // same as PickupFail1
		PickupFailOverburden3 = 19,  // same as PickupFail1

		/// <summary>
		/// Plays a sound and shows a text informing the player he is
		/// not allowed to have more of this item type
		/// </summary>
		PickupFailNotAllowedMore = 20,

		/// <summary>
		/// Shows a text informing the player that the item he wants to pick up
		/// does not belong to him
		/// </summary>
		PickupFailIsNotYours = 21,

		Unknown21 = 22,                 //crashes client
		Unknown22 = 23,                 //char becomes white
		Unknown23 = 24,              // character disappears

		/// <summary>
		/// Splashes blood towards another actor. Takes the splash target dynamic id as parameter or -1 for an undirected splash
		/// </summary>
		BloodSplash = 25,

		Unknown25 = 26, //nothing
		Unknown26 = 27, //char goes dark
		IcyEffect = 28, // light blue cloud
		IcyEffect2 = 29,  // darker blue
		IcyEffect3 = 30, // bright blue glow
		Unknown30 = 31, //nothing
		Unknown31 = 32, // takes another value that looks like an id but is not an actor

		/// <summary>
		/// Plays an effect group. Takes the sno of the effect group as parameter
		/// </summary>
		PlayEffectGroup = 33,

		Unknown33 = 34, //nothing
		LoudNoise = 35,              // plays a loud sound: TODO rename this enum value, testet with monk
		LoudNoise2 = 36,                // plays a loud sound: TODO rename this enum value, tested with monk
		Unknown36 = 37, //nothing

		/// <summary>
		/// Energy / Furty / Mana etc pickup indicator, right globe flashes
		/// </summary>
		SecondaryRessourceEffect = 38,

		Unknown38 = 39, //nothing
		Unknown39 = 40, //nothing

		/// <summary>
		/// Plays a gore effect
		/// </summary>
		Gore = 41,

		/// <summary>
		/// Plays a gore effect with fire component
		/// </summary>
		GoreFire = 42,

		/// <summary>
		/// Plays a gore effect with poison component
		/// </summary>
		GorePoison = 43,

		/// <summary>
		/// Plays a gore effect with arcane component
		/// </summary>
		GoreArcane = 44,

		/// <summary>
		/// Plays a gore effect with holy component
		/// </summary>
		GoreHoly = 45,

		/// <summary>
		/// Plays a gore effect with electro component
		/// </summary>
		GoreElectro = 46,

		IceBreak = 47,
		Inferno = 48,   // infernolike explosion
		Darker = 49,
		Red = 50, //char covered in blood
		Lila1 = 51, //purple mana glowing
		Lila2 = 52,
		Burned1 = 53,
		Blue1 = 54,
		Blue2 = 55,
		Burned2 = 56,
		Green = 57,
		Unknown57 = 58,
		Unknown58 = 59,// shield block metal sound
		Unknown59 = 60, // takes another value. have seen only field2==1, mostly used on shield skeletons
		Unknown60 = 61, // takes another parameter. have seen it only once with field2 == 1 on a shield skeleton
		Unknown61 = 62,// used often but i dont see anything

		/// <summary>
		/// Blue bubbles around the actor
		/// </summary>
		ManaPickup = 63,

		Unknown63 = 64, //nothing
		Unknown64 = 65, //nothing
		Unknown65 = 66, //nothing
		Unknown66 = 67,
		Unknown67 = 68,  // actor starts to drowing down - second parameter is ticks to fully drow
						 //Unknown68 = 69,  // that one crashes
		Unknown69 = 70, //book sound

		/// <summary>
		/// Displays a checkpoint (reached) sign
		/// </summary>
		Checkpoint = 71,

		ParagonLevelUp = 72,

		Unknown73 = 73, //character holy-like effect
		Unknown74 = 74 //potion sound
	}
}
