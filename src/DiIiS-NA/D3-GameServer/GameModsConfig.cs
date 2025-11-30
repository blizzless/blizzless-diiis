using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using DiIiS_NA;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer;
using Newtonsoft.Json;

namespace DiIiS_NA.D3_GameServer;

public class RateConfig
{
    public float GetDamageByDifficulty(int diff)
    {
        diff = Math.Clamp(diff, 0, 19);
        return !DamageByDifficulty.ContainsKey(diff) ? 1f : DamageByDifficulty[diff];
    }
    public Dictionary<int, float> HealthByDifficulty { get; set; } = new()
    {
        [0] = 1.0f, [1] = 1.0f, [2] = 1.0f, [3] = 1.0f, [4] = 1.0f, [5] = 1.0f,
        [6] = 1.0f, [7] = 1.0f, [8] = 1.0f, [9] = 1.0f, [10] = 1.0f, [11] = 1.0f,
        [12] = 1.0f, [13] = 1.0f, [14] = 1.0f, [15] = 1.0f, [16] = 1.0f,
        [17] = 1.0f, [18] = 1.0f, [19] = 1.0f,
    };
    
    public Dictionary<int, float> DamageByDifficulty { get; set; } = new()
    {
        [0] = 1.0f, [1] = 1.0f, [2] = 1.0f, [3] = 1.0f, [4] = 1.0f, [5] = 1.0f,
        [6] = 1.0f, [7] = 1.0f, [8] = 1.0f, [9] = 1.0f, [10] = 1.0f, [11] = 1.0f,
        [12] = 1.0f, [13] = 1.0f, [14] = 1.0f, [15] = 1.0f, [16] = 1.0f,
        [17] = 1.0f, [18] = 1.0f, [19] = 1.0f,
    };
    public float Experience { get; set; } = 1;
    public float Gold { get; set; } = 1;
    public float Drop { get; set; } = 1;
    public float ChangeDrop { get; set; } = 1;
}

public class HealthConfig
{
    public float PotionRestorePercentage { get; set; } = 60f;
    public float PotionCooldown { get; set; } = 30f;
    public int ResurrectionCharges { get; set; } = 3;
}

public class HealthDamageMultiplier
{
    public float HealthMultiplier { get; set; } = 1;
    public float DamageMultiplier { get; set; } = 1;
}

public class MonsterConfig
{
    public float AttacksPerSecond { get; set; } = 1.2f;

    public float HealthMultiplier { get; set; } = 1;
    public float HealthBonusMultiplier { get; set; } = 1;
    public float DamageMultiplier { get; set; } = 1;

    /// <summary>
    /// Attack target range
    /// </summary>
    public float LookupRange { get; set; } = 80f;

    /// <summary>
    /// Total health bonus multiplier that can be applied to a monster
    /// </summary>
    public float HealthBonusMultiplierCap { get; set; } = 1.025f;
}

public class QuestConfig
{
    public bool AutoSave { get; set; } = false;
    public bool UnlockAllWaypoints { get; set; } = false;
}

public class PlayerMultiplierConfig
{
    public ParagonConfig<float> Strength { get; set; } = new(1f);
    public ParagonConfig<float> Dexterity { get; set; } = new(1f);
    public ParagonConfig<float> Intelligence { get; set; } = new(1f);
    public ParagonConfig<float> Vitality { get; set; } = new(1f);
}
public class PlayerConfig
{
    public PlayerMultiplierConfig Multipliers = new();
}

public class ItemsConfig
{
    public UnidentifiedDrop UnidentifiedDropChances { get; set; } = new();
}

public class UnidentifiedDrop
{
    public float HighQuality { get; set; } = 30f;
    public float NormalQuality { get; set; } = 5f;
}

public class MinimapConfig
{
    public bool ForceVisibility { get; set; } = false;
}

public class NephalemRiftConfig
{
    public float ProgressMultiplier { get; set; } = 1f;
    public bool AutoFinish { get; set; } = false;
    public int AutoFinishThreshold { get; set; } = 2;
    public float OrbsChance { get; set; } = 0f;
}

public class GameModsConfig
{
    public RateConfig Rate { get; set; } = new();
    public HealthConfig Health { get; set; } = new();
    public MonsterConfig Monster { get; set; } = new();
    public HealthDamageMultiplier Boss { get; set; } = new();
    public QuestConfig Quest { get; set; } = new();
    public PlayerConfig Player { get; set; } = new();
    public ItemsConfig Items { get; set; } = new();
    public MinimapConfig Minimap { get; set; } = new();
    public NephalemRiftConfig NephalemRift { get; set; } = new();
    
    private static readonly Logger Logger = LogManager.CreateLogger();

    public GameModsConfig() {}

    static GameModsConfig()
    {
        CreateInstance();
    }
    
    public static void ReloadSettings()
    {
        CreateInstance();
    }

    private static readonly object InstanceCreationLock = new();
    public static GameModsConfig Instance { get; private set; }

    private static void CreateInstance()
    {
        lock (InstanceCreationLock)
        {
            if (!File.Exists("config.mods.json"))
            {
                Instance = CreateDefaultFile();
            }
            else
            {
                var content = File.ReadAllText("config.mods.json");
                if (content.TryFromJson(out GameModsConfig config, out Exception ex))
                {
                    Logger.Success("Game mods loaded successfully!");
                    var @new = config.ToJson(Formatting.Indented);
                    File.WriteAllText(@"config.mods.json", @new);
                    Instance = config;
                    return;
                }

                Logger.Fatal("An error occurred whilst loading $[white on red]$config.mods.json$[/]$ file. Please verify if the file is correct. Delete the file and try again.");
                Program.Shutdown(ex);
            }
        }
    }

    private static GameModsConfig CreateDefaultFile()
    {
        var migration = GameServerConfig.Instance;
        File.WriteAllText("config.mods.json", new GameModsConfig().ToJson());
        Logger.Success("Game mods file created successfully!");
        return new GameModsConfig();
    }
}

public static class JsonExtensions
{
    private const bool Indented = true;

    public static string ToJson(this object obj, Formatting? formatting = null)
    {
        return JsonConvert.SerializeObject(obj, formatting ?? (Indented ? Formatting.Indented : Formatting.None));
    }

    public static bool TryFromJson<T>(this string obj, out T value)
        where T: class, new()
    {
        try
        {
            value = obj.FromJson<T>();
            return true;
        }
        catch (Exception ex)
        {
            value = default;
            return false;
        }
    }
    
    public static bool TryFromJson<T>(this string obj, out T value, out Exception exception)
        where T: class, new()
    {
        try
        {
            value = obj.FromJson<T>();
            exception = null;
            return true;
        }
        catch (Exception ex)
        {
            value = default;
            exception = ex;
            return false;
        }
    }
    
    public static T FromJson<T>(this string obj)
        where T: class, new()
    {
        return JsonConvert.DeserializeObject<T>(obj);
    }

    public static dynamic FromJsonDynamic(this string obj)
    {
        return obj.FromJson<ExpandoObject>();
    }
}