using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Clockwork;

public class ThingComp_ClockworkMachine : ThingComp
{
    private float assemblyProgress;

    private List<IntVec3> cachedAdjCellsCardinal;
    public CompProperties_ClockworkMachine Props => (CompProperties_ClockworkMachine)props;

    private bool PowerOn => parent.GetComp<CompPowerTrader>()?.PowerOn ?? false;
    private bool SteamOn => parent.GetComp<ThingComp_SteamConsumer>()?.running ?? false;

    public List<IntVec3> AdjCellsCardinalInBounds
    {
        get
        {
            if (cachedAdjCellsCardinal == null)
            {
                cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(parent)
                    where c.InBounds(parent.Map)
                    select c).ToList();
            }

            return cachedAdjCellsCardinal;
        }
    }


    public override void CompTick()
    {
        if (!parent.Spawned)
        {
            return;
        }

        var comp = parent.GetComp<CompCanBeDormant>();
        if (comp != null)
        {
            if (!comp.Awake)
            {
                return;
            }
        }
        else if (parent.Position.Fogged(parent.Map))
        {
            return;
        }

        if (Props.requiresPower && !PowerOn && !SteamOn)
        {
            return;
        }

        if (Props.machineInput == null)
        {
            var num = 1f / (Props.hoursToAssembly * 2500f);
            assemblyProgress += num;
            if (!(assemblyProgress > 1f))
            {
                return;
            }

            TryOutputNoInput();
            ResetCountdown();
        }
        else if (HasEnoughMaterialInHoppers() && HasOutput())
        {
            var num = 1f / (Props.hoursToAssembly * 2500f);
            assemblyProgress += num;
            CheckShouldSpawn();
        }
    }

    private void CheckShouldSpawn()
    {
        if (!(assemblyProgress > 1f))
        {
            return;
        }

        TryOutput();
        ResetCountdown();
    }

    private void ResetCountdown()
    {
        assemblyProgress = 0f;
    }

    public override string CompInspectStringExtra()
    {
        if (!HasOutput())
        {
            return "MachineNeedsOutput".Translate();
        }

        if (Props.machineInput != null && !HasEnoughMaterialInHoppers())
        {
            return "MachineNeedsMaterials".Translate() + ": " + Props.machineInput.label + " x" +
                   Props.machineMaterialCost;
        }

        return "AssemblyProgress".Translate() + ": " + assemblyProgress.ToStringPercent();
    }

    public void TryOutput()
    {
        if (!HasEnoughMaterialInHoppers() || !(assemblyProgress > 1f))
        {
            return;
        }

        var list = new List<ThingDef>();

        var thing = FindMaterialInAnyHopper();

        if (thing == null)
        {
            Log.Error("Did not find enough materials in hoppers.");
            return;
        }

        var num = Props.machineMaterialCost;
        list.Add(thing.def);
        thing.SplitOff(num);

        if (!TryFindOutput(parent, Props.machineOutput, Props.machineOutputAmount, out var result))
        {
            return;
        }

        var thing2 = ThingMaker.MakeThing(Props.machineOutput);
        thing2.stackCount = Props.machineOutputAmount;
        GenPlace.TryPlaceThing(thing2, result, parent.Map, ThingPlaceMode.Direct, out _);
    }

    public void TryOutputNoInput()
    {
        if (!(assemblyProgress > 1f))
        {
            return;
        }

        if (!TryFindOutput(parent, Props.machineOutput, Props.machineOutputAmount, out var result))
        {
            return;
        }

        var thing2 = ThingMaker.MakeThing(Props.machineOutput);
        thing2.stackCount = Props.machineOutputAmount;
        GenPlace.TryPlaceThing(thing2, result, parent.Map, ThingPlaceMode.Direct, out _);
    }

    public virtual Thing FindMaterialInAnyHopper()
    {
        for (var i = 0; i < AdjCellsCardinalInBounds.Count; i++)
        {
            Thing thing = null;
            Thing thing2 = null;
            var thingList = AdjCellsCardinalInBounds[i].GetThingList(parent.Map);
            foreach (var thing3 in thingList)
            {
                if (thing3.def == Props.machineInput)
                {
                    thing = thing3;
                }

                if (thing3.def == ThingDefOf.ClockworkInput)
                {
                    thing2 = thing3;
                }
            }

            if (thing != null && thing2 != null)
            {
                return thing;
            }
        }

        return null;
    }

    public virtual bool HasEnoughMaterialInHoppers()
    {
        var num = 0;
        foreach (var c in AdjCellsCardinalInBounds)
        {
            Thing thing = null;
            Thing thing2 = null;
            var thingList = c.GetThingList(parent.Map);
            foreach (var thing3 in thingList)
            {
                if (thing3.def == Props.machineInput)
                {
                    thing = thing3;
                }

                if (thing3.def == ThingDefOf.ClockworkInput)
                {
                    thing2 = thing3;
                }
            }

            if (thing != null && thing2 != null)
            {
                num += thing.stackCount;
            }

            if (num >= Props.machineMaterialCost)
            {
                return true;
            }
        }

        return false;
    }

    public virtual bool HasOutput()
    {
        foreach (var c in AdjCellsCardinalInBounds)
        {
            var thingList = c.GetThingList(parent.Map);
            foreach (var thing in thingList)
            {
                if (thing.def == ThingDefOf.ClockworkOutput)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool TryFindOutput(Thing parent, ThingDef thingToSpawn, int spawnCount, out IntVec3 result)
    {
        foreach (var item in GenAdj.CellsAdjacent8Way(parent))
        {
            var thingList = item.GetThingList(parent.Map);
            foreach (var thing in thingList)
            {
                if (thing.def != ThingDefOf.ClockworkOutput)
                {
                    continue;
                }

                result = item;
                return true;
            }
        }

        result = IntVec3.Invalid;
        return false;
    }
}