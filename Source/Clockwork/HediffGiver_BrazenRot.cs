using Verse;

namespace Clockwork;

public class HediffGiver_BrazenRot : HediffGiver
{
    public enum HediffType
    {
        clockwork,
        steamwork
    }

    public HediffDef cure;
    public HediffDef cureImplant;
    public HediffDef hediffDeadly;

    private HediffType hediffType;
    public float severityPerDay;

    public override void OnIntervalPassed(Pawn pawn, Hediff notUsed)
    {
        if (!ClockworkAndSteamSettings.brazenRot)
        {
            return;
        }

        ApplyBrazenRot(pawn, ClockworkAndSteamSettings.brazenRotIsDeadly ? hediffDeadly : hediff);
    }

    public void ApplyBrazenRot(Pawn pawn, HediffDef type)
    {
        if ((hediffType != HediffType.clockwork || !ClockworkAndSteamSettings.brazenRotClockwork) &&
            (hediffType != HediffType.steamwork || !ClockworkAndSteamSettings.brazenRotSteamwork))
        {
            return;
        }

        if (pawn.health.hediffSet.HasHediff(cure) ||
            pawn.health.hediffSet.HasHediff(cureImplant)) // If we have cure, remove the hediff
        {
            if (type == null)
            {
                return;
            }

            try
            {
                var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(type);
                pawn.health.RemoveHediff(firstHediffOfDef);
            }
            catch
            {
                // ignored
            }
        }
        else
        {
            HealthUtility.AdjustSeverity(pawn, type,
                severityPerDay * 0.00333333341f *
                ClockworkAndSteamSettings.brazenRotMultiplier); // Converts severity to severity per day
        }
    }
}