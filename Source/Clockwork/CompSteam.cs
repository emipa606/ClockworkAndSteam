using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Clockwork;

public class CompSteam : ThingComp
{
    public List<IntVec3> cachedAdjCellsCardinal;
    public SteamSystemMapComp mapComp;
    public bool registered;
    public SteamSystem steamSystem;

    public CompProperties_CompSteam Props => (CompProperties_CompSteam)props;

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

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        mapComp = parent.Map.GetComponent<SteamSystemMapComp>();
        if (!parent.Spawned)
        {
            return;
        }

        var comp = parent.GetComp<CompCanBeDormant>();
        if (comp != null)
        {
            if (!comp.Awake)
            {
            }
        }
        else if (parent.Position.Fogged(parent.Map))
        {
        }
        else
        {
            if (!registered)
            {
                RegisterToSteamSystem();
            }
        }

        //CreatePipes();
    }

    public override void PostDeSpawn(Map map)
    {
        steamSystem.UnregisterSteamThing(this);
        base.PostDeSpawn(map);
        //RemoveDecorativePipes();
    }

    public void RegisterToSteamSystem()
    {
        if (steamSystem == null)
        {
            TryConnectToSteamSystem();
        }

        steamSystem?.RegisterSteamThing(this);
        registered = true;
    }

    public void TryConnectToSteamSystem()
    {
        for (var i = 0; i < AdjCellsCardinalInBounds.Count; i++)
        {
            var thingList = AdjCellsCardinalInBounds[i].GetThingList(parent.Map);
            foreach (var thing in thingList)
            {
                var steamThing = thing.TryGetComp<CompSteam>();

                var newSystem = steamThing?.SteamSystem();
                if (newSystem == null)
                {
                    continue;
                }

                if (steamSystem == null)
                {
                    steamSystem = newSystem;
                }
                else if (steamSystem != newSystem)
                {
                    steamSystem.MergeSystem(newSystem);
                }
            }
        }

        if (steamSystem != null)
        {
            return;
        }

        steamSystem = new SteamSystem();
        mapComp.RegisterSteamSystem(steamSystem);
    }

    public override string CompInspectStringExtra()
    {
        if (!registered)
        {
            return null;
        }

        string str = "CW.SystemVolume".Translate(Math.Round(steamSystem.steamVolume, 1),
            Math.Round(steamSystem.systemVolume, 1), Math.Round(steamSystem.systemPressure));

        if (ClockworkAndSteamSettings.showSteamSystemID)
        {
            str += "CW.SystemID".Translate(steamSystem.ShowSystemID());
        }

        return str;
    }

    public void PrintForSteamGrid(SectionLayer layer)
    {
        if (Props.pipe)
        {
            SteamOverlayMats.LinkedOverlayGraphic.Print(layer, parent, 0f);
        }
    }

    public SteamSystem SteamSystem()
    {
        return steamSystem;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_References.Look(ref steamSystem, "steamSystem");
    }
}