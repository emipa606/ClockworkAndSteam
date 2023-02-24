using Verse;

namespace Clockwork;

public class CompProperties_ClockworkMachine : CompProperties
{
    public float hoursToAssembly = 1f;
    public ThingDef machineInput;
    public int machineMaterialCost = 1;
    public ThingDef machineOutput;
    public int machineOutputAmount = 1;
    public bool requiresPower = true;

    public CompProperties_ClockworkMachine()
    {
        compClass = typeof(ThingComp_ClockworkMachine);
    }
}