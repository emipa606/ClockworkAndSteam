using RimWorld;
using Verse;

namespace Clockwork;

public class ThingComp_SteamConsumer : CompSteam
{
    protected CompAutoPowered autoPoweredComp;
    protected CompBreakdownable breakdownableComp;
    protected CompFlickable flickableComp;
    private OverlayHandle? overlayNeedsPower;

    private OverlayHandle? overlayPowerOff;

    protected CompRefuelable refuelableComp;
    public bool running;

    public float steamNeeded;
    public CompProperties_SteamConsumer Props => (CompProperties_SteamConsumer)props;

    private bool pressureReached => steamSystem.systemPressure > Props.minPressure;
    private bool insufficentSteam => steamSystem.insufficentSteam;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        refuelableComp = parent.GetComp<CompRefuelable>();
        breakdownableComp = parent.GetComp<CompBreakdownable>();
        flickableComp = parent.GetComp<CompFlickable>();
        autoPoweredComp = parent.GetComp<CompAutoPowered>();
    }

    public override void CompTick()
    {
        base.CompTick();
        UpdateDesiredSteamOutput();
    }

    public void UpdateDesiredSteamOutput()
    {
        if (breakdownableComp is { BrokenDown: true } ||
            refuelableComp is { HasFuel: false } || flickableComp is { SwitchIsOn: false } ||
            autoPoweredComp is { WantsToBeOn: false })
        {
            steamNeeded = 0f;
            running = false;
            UpdateOverlays();
        }
        else
        {
            steamNeeded = Props.steamConsumption;
            running = pressureReached;

            UpdateOverlays();
        }
    }

    public override string CompInspectStringExtra()
    {
        var str = "CW.Unregistered".Translate();
        if (!registered)
        {
            return str + "\n" + base.CompInspectStringExtra();
        }

        if (!running)
        {
            str = flickableComp is { SwitchIsOn: false }
                ? "CW.SwitchedOff".Translate()
                : "CW.PressureInfo".Translate(Props.minPressure, steamNeeded);
        }
        else
        {
            str = "CW.CurrentConsumption".Translate(steamNeeded);
        }

        return str + "\n" + base.CompInspectStringExtra();
    }

    private void UpdateOverlays()
    {
        if (!parent.Spawned)
        {
            return;
        }

        parent.Map.overlayDrawer.Disable(parent, ref overlayPowerOff);
        parent.Map.overlayDrawer.Disable(parent, ref overlayNeedsPower);
        if (parent.IsBrokenDown())
        {
            return;
        }

        if (flickableComp is { SwitchIsOn: false } && !overlayPowerOff.HasValue)
        {
            overlayPowerOff = parent.Map.overlayDrawer.Enable(parent, OverlayTypes.PowerOff);
        }

        if (FlickUtility.WantsToBeOn(parent) && !running && !overlayNeedsPower.HasValue)
        {
            overlayNeedsPower = parent.Map.overlayDrawer.Enable(parent, OverlayTypes.NeedsPower);
        }
    }
}