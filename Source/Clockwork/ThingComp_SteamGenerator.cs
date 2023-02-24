using RimWorld;
using Verse;
using Verse.Sound;

namespace Clockwork;

public class ThingComp_SteamGenerator : CompSteam
{
    protected CompAutoPowered autoPoweredComp;
    protected CompBreakdownable breakdownableComp;
    protected CompFlickable flickableComp;
    private OverlayHandle? overlayNeedsPower;
    private OverlayHandle? overlayPowerOff;

    protected CompRefuelable refuelableComp;

    public float steamOutput;

    private Sustainer sustainerProducingPower;
    public CompProperties_SteamGenerator Props => (CompProperties_SteamGenerator)props;

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
        if (Props.soundAmbientProducingSteam == null)
        {
            return;
        }

        if (steamOutput > 0.01f)
        {
            if (sustainerProducingPower == null || sustainerProducingPower.Ended)
            {
                sustainerProducingPower = Props.soundAmbientProducingSteam.TrySpawnSustainer(SoundInfo.InMap(parent));
            }

            sustainerProducingPower.Maintain();
        }
        else if (sustainerProducingPower != null)
        {
            sustainerProducingPower.End();
            sustainerProducingPower = null;
        }
    }

    public void UpdateDesiredSteamOutput()
    {
        if (breakdownableComp is { BrokenDown: true } ||
            refuelableComp is { HasFuel: false } || flickableComp is { SwitchIsOn: false } ||
            autoPoweredComp is { WantsToBeOn: false })
        {
            steamOutput = 0f;
        }
        else
        {
            steamOutput = Props.steamGeneration;
        }
    }

    public override string CompInspectStringExtra()
    {
        var str = "CW.Unregistered".Translate();

        if (registered)
        {
            str = "CW.CurrentOutput".Translate(steamOutput);
        }

        return str + "\n" + base.CompInspectStringExtra();
    }
}