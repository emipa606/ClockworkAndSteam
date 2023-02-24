using System.Linq;
using RimWorld;
using Verse;

namespace Clockwork;

public class SectionLayer_ThingsSteamGrid : SectionLayer_Things
{
    public SectionLayer_ThingsSteamGrid(Section section)
        : base(section)
    {
        requireAddToMapMesh = false;
        relevantChangeTypes = MapMeshFlag.Buildings;
    }

    public override void DrawLayer()
    {
        if (Find.DesignatorManager.SelectedDesignator is not Designator_Build val)
        {
            return;
        }

        if (val.PlacingDef is ThingDef val2 && val2.comps.OfType<CompProperties_CompSteam>().Any())
        {
            base.DrawLayer();
        }
    }

    public override void Regenerate()
    {
        ClearSubMeshes(MeshParts.All);
        foreach (var item in section.CellRect)
        {
            var list = Map.thingGrid.ThingsListAt(item);
            var count = list.Count;
            for (var i = 0; i < count; i++)
            {
                var thing = list[i];
                if ((thing.def.seeThroughFog ||
                     !Map.fogGrid.fogGrid[CellIndicesUtility.CellToIndex(thing.Position, Map.Size.x)]) &&
                    thing.def.drawerType != 0 &&
                    (thing.def.drawerType != DrawerType.RealtimeOnly || !requireAddToMapMesh) &&
                    (!(thing.def.hideAtSnowDepth < 1f) ||
                     !(Map.snowGrid.GetDepth(thing.Position) > thing.def.hideAtSnowDepth)) &&
                    thing.Position.x == item.x && thing.Position.z == item.z)
                {
                    TakePrintFrom(thing);
                }
            }
        }

        FinalizeMesh(MeshParts.All);
    }

    protected override void TakePrintFrom(Thing t)
    {
        if (t.Faction != null && t.Faction != Faction.OfPlayer)
        {
            return;
        }

        var comp = t.TryGetComp<CompSteam>();
        comp?.PrintForSteamGrid(this);
    }
}