using UnityEngine;
using Verse;

namespace Clockwork;

public class ClockworkAndSteamSettings : ModSettings
{
    public static bool steamGuns;
    public static bool teslaGun;
    public static bool turrets;

    public static bool brazenRot = true;
    public static bool brazenRotClockwork = true;
    public static bool brazenRotSteamwork = true;
    public static bool brazenRotIsDeadly;
    public static float brazenRotMultiplier = 1f;

    public static bool integratedResearch;

    public static bool showSteamSystemID;

    private static readonly float height_modifier = 100f;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref steamGuns, "steamGuns");
        Scribe_Values.Look(ref teslaGun, "teslaGun");
        Scribe_Values.Look(ref turrets, "turrets");
        Scribe_Values.Look(ref brazenRot, "brazenRot", true);
        Scribe_Values.Look(ref brazenRotClockwork, "brazenRotClockwork", true);
        Scribe_Values.Look(ref brazenRotSteamwork, "brazenRotSteamwork", true);
        Scribe_Values.Look(ref brazenRotIsDeadly, "brazenRotSteamwork");
        Scribe_Values.Look(ref brazenRotMultiplier, "brazenRotMultiplier", 1f);
        Scribe_Values.Look(ref integratedResearch, "integratedResearch");
        Scribe_Values.Look(ref showSteamSystemID, "showSteamSystemID");
        base.ExposeData();
    }

    public static void DoWindowContents(Rect inRect)
    {
        var viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + height_modifier);

        var listingStandard = new Listing_Standard
        {
            maxOneColumn = true,
            ColumnWidth = viewRect.width
        };
        listingStandard.Begin(inRect);

        Text.Font = GameFont.Medium;
        listingStandard.Label("CW.Research".Translate());
        Text.Font = GameFont.Small;
        listingStandard.Gap(4f);
        listingStandard.CheckboxLabeled("CW.IntegratedResearch".Translate(), ref integratedResearch,
            "CW.IntegratedResearchTT".Translate());

        Text.Font = GameFont.Medium;
        listingStandard.Label("CW.BrazenRot".Translate());
        Text.Font = GameFont.Small;
        listingStandard.Gap(4f);
        listingStandard.CheckboxLabeled("CW.EnableBrazenRot".Translate(), ref brazenRot,
            "CW.EnableBrazenRotTT".Translate());
        if (brazenRot)
        {
            listingStandard.CheckboxLabeled("CW.ClockworkBrazenRot".Translate(), ref brazenRotClockwork,
                "CW.ClockworkBrazenRotTT".Translate());
            listingStandard.CheckboxLabeled("CW.SteamworkBrazenRot".Translate(), ref brazenRotSteamwork,
                "CW.SteamworkBrazenRotTT".Translate());
            listingStandard.CheckboxLabeled("CW.LeathalBrazenRot".Translate(), ref brazenRotIsDeadly,
                "CW.LeathalBrazenRotTT".Translate());
            listingStandard.Gap(20f);
            listingStandard.Label("CW.BrazenRotMultiplier".Translate(brazenRotMultiplier), -1f,
                "CW.BrazenRotMultiplierTT".Translate());
            brazenRotMultiplier = listingStandard.Slider(brazenRotMultiplier, 0f, 10f);
            if (listingStandard.ButtonText("CW.ResetMultiplier".Translate()))
            {
                brazenRotMultiplier = 1f;
            }
        }

        Text.Font = GameFont.Medium;
        listingStandard.Label("CW.Secrets".Translate());
        Text.Font = GameFont.Small;
        listingStandard.Gap(4f);
        listingStandard.CheckboxLabeled("CW.Bird".Translate(), ref steamGuns, "CW.BirdTT".Translate());

        Text.Font = GameFont.Medium;
        listingStandard.Label("CW.Debug".Translate());
        Text.Font = GameFont.Small;
        listingStandard.Gap(4f);
        listingStandard.CheckboxLabeled("CW.ShowId".Translate(), ref showSteamSystemID, "CW.ShowIdTT".Translate());
        if (ClockworkAndSteam.currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("CW.CurrentModVersion".Translate(ClockworkAndSteam.currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }
}