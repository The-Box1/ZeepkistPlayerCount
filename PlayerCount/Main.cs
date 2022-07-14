using BepInEx;
using HarmonyLib;

namespace PlayerCount;

[BepInAutoPlugin("dev.thebox1.zeepkistplayercount")]
[BepInProcess("Zeepkist.exe")]
public partial class Main : BaseUnityPlugin
{
    public new static BepInEx.Logging.ManualLogSource Logger;

    private Harmony Harmony { get; } = new (Id);
    
    public static byte MaxPlayers = 16;

    public void Awake()
    {
        Logger = base.Logger;
        Harmony.PatchAll();
        Logger.LogMessage("Loaded PlayerCount");
    }
}