using System.Collections.Generic;
using Verse;

namespace Clockwork;

public class SteamSystemMapComp : MapComponent
{
    public static Dictionary<int, SteamSystemMapComp> SteamComps = new Dictionary<int, SteamSystemMapComp>();

    public List<SteamSystem> steamSystems = new List<SteamSystem>();

    public SteamSystemMapComp(Map map) : base(map)
    {
        SteamComps[this.map.uniqueID] = this;
    }

    public override void MapComponentTick()
    {
        foreach (var steamSystem in steamSystems)
        {
            try
            {
                steamSystem.Tick();
            }
            catch
            {
                steamSystems.Remove(steamSystem);
            }
        }
    }

    public void RegisterSteamSystem(SteamSystem steamSystem)
    {
        if (steamSystems.Contains(steamSystem))
        {
            return;
        }

        steamSystem.steamSystemMapComp = this;
        steamSystems.Add(steamSystem);
    }

    public void UnregisterSteamSystem(SteamSystem steamSystem)
    {
        if (steamSystems.Contains(steamSystem))
        {
            steamSystems.Remove(steamSystem);
        }
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref steamSystems, "steamSystems", LookMode.Deep);
        base.ExposeData();
    }

    //public void DrawLayer()
    //{
    //    if (DebugViewSettings.drawThingsPrinted)
    //    {
    //        Designator_Build val = Find.DesignatorManager.SelectedDesignator as Designator_Build;
    //        if (val != null)
    //        {
    //            ThingDef val2 = val.PlacingDef as ThingDef;
    //            if (val2 != null && val2.comps.OfType<CompProperties_CompSteam>().Any())
    //            {
    //                DrawSteamNets();
    //            }
    //        }
    //    }
    //}

    //private void DrawSteamNets()
    //{
    //    if (Current.ProgramState != ProgramState.Playing || Find.CurrentMap != map)
    //    {
    //        return;
    //    }
    //    foreach (SteamSystem steamSystem in steamSystems)
    //    {
    //        foreach (CompSteam item in steamSystem.steamThingList)
    //        {
    //            foreach (IntVec3 item2 in GenAdj.CellsOccupiedBy(item.parent))
    //            {
    //                CellRenderer.RenderCell(item2, 0.25f);
    //            }
    //        }
    //    }
    //}
}