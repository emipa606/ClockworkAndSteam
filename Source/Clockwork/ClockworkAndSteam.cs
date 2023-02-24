using Mlie;
using UnityEngine;
using Verse;

namespace Clockwork;

public class ClockworkAndSteam : Mod
{
    public static string currentVersion;
    private ClockworkAndSteamSettings settings;

    public ClockworkAndSteam(ModContentPack content) : base(content)
    {
        settings = GetSettings<ClockworkAndSteamSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Clockwork And Steam";
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        ClockworkAndSteamSettings.DoWindowContents(inRect);
    }
}