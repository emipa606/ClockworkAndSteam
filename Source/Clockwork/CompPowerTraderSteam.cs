using RimWorld;
using Verse;
using Verse.Sound;

namespace Clockwork;

public class CompPowerTraderSteam : CompPowerTrader
{
    private OverlayHandle? overlayNeedsPower;
    protected ThingComp_SteamConsumer steamConsumerComp;

    private Sustainer sustainerPowered;

    private bool wasPowered;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        steamConsumerComp = parent.GetComp<ThingComp_SteamConsumer>();
    }

    public override void CompTick()
    {
        if (steamConsumerComp.running)
        {
            if (wasPowered)
            {
                return;
            }

            PowerOn = true;
            parent.Map.overlayDrawer.Disable(parent, ref overlayNeedsPower);

            StartSustainerPoweredIfInactive();

            wasPowered = true;
        }
        else
        {
            if (wasPowered)
            {
                PowerOn = false;

                EndSustainerPoweredIfActive();

                wasPowered = false;
            }

            parent.Map.overlayDrawer.Disable(parent, ref overlayNeedsPower);
        }
    }

    public override string CompInspectStringExtra()
    {
        if (steamConsumerComp == null)
        {
            return "ERROR: MISSING STEAMCOMP";
        }

        return PowerOn ? "CW.Operational".Translate() : "CW.AwaitingPressure".Translate();
    }

    public override void ReceiveCompSignal(string signal)
    {
    }

    private void StartSustainerPoweredIfInactive()
    {
        var compPropertiesPower = Props;
        if (compPropertiesPower.soundAmbientPowered.NullOrUndefined() || sustainerPowered != null)
        {
            return;
        }

        var info = SoundInfo.InMap(parent);
        sustainerPowered = compPropertiesPower.soundAmbientPowered.TrySpawnSustainer(info);
    }

    private void EndSustainerPoweredIfActive()
    {
        if (sustainerPowered == null)
        {
            return;
        }

        sustainerPowered.End();
        sustainerPowered = null;
    }
}