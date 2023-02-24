using RimWorld;
using Verse;

namespace Clockwork;

public class Building_SteamWorkTable : Building_WorkTable, IBillGiver
{
    private CompBreakdownable breakdownableComp;
    private ThingComp_SteamConsumer steamConsumerComp;

    public new bool CurrentlyUsableForBills()
    {
        if (!UsableForBillsAfterFueling())
        {
            return false;
        }

        return steamConsumerComp == null || steamConsumerComp.running;
    }

    public new bool UsableForBillsAfterFueling()
    {
        if (steamConsumerComp is { running: false })
        {
            return false;
        }

        return breakdownableComp is not { BrokenDown: true };
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        steamConsumerComp = GetComp<ThingComp_SteamConsumer>();
        breakdownableComp = GetComp<CompBreakdownable>();
        base.SpawnSetup(map, respawningAfterLoad);
    }
}