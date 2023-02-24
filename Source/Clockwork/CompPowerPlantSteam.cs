using Verse;

namespace Clockwork;

public class CompPowerPlantSteam : CompPowerTraderSteam
{
    protected float desiredPowerOutput;


    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        steamConsumerComp = parent.GetComp<ThingComp_SteamConsumer>();
        desiredPowerOutput = 0f - Props.PowerConsumption;
        if (!Props.transmitsPower && !parent.def.ConnectToPower)
        {
            return;
        }

        parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.PowerGrid, true, false);
        if (Props.transmitsPower)
        {
            parent.Map.powerNetManager.Notify_TransmitterSpawned(this);
        }

        if (parent.def.ConnectToPower)
        {
            parent.Map.powerNetManager.Notify_ConnectorWantsConnect(this);
        }
    }

    public override void CompTick()
    {
        switch (desiredPowerOutput)
        {
            case > 0f when steamConsumerComp.running:
                PowerOutput = desiredPowerOutput;
                break;
            case > 0f when !steamConsumerComp.running:
                PowerOutput = 0f;
                break;
        }
    }

    public override string CompInspectStringExtra()
    {
        if (PowerNet == null)
        {
            return "PowerNotConnected".Translate();
        }

        string str = "PowerOutput".Translate() + ": " + PowerOutput.ToString("#####0") + " W";
        var value = (PowerNet.CurrentEnergyGainRate() / WattsToWattDaysPerTick).ToString("F0");
        var value2 = PowerNet.CurrentStoredEnergy().ToString("F0");
        return str + "\n" + "PowerConnectedRateStored".Translate(value, value2);
    }
}