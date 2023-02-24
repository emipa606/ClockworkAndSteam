using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Clockwork;

public class AirshipLeaving : Skyfaller, IActiveDropPod
{
    private static readonly List<Thing> tmpActiveDropPods = new List<Thing>();

    private bool alreadyLeft;

    public TransportPodsArrivalAction arrivalAction;

    public bool createWorldObject = true;

    public int destinationTile = -1;
    public int groupID = -1;

    public WorldObjectDef worldObjectDef;

    public ActiveDropPodInfo Contents
    {
        get => ((ActiveDropPod)innerContainer[0]).Contents;
        set => ((ActiveDropPod)innerContainer[0]).Contents = value;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref groupID, "groupID");
        Scribe_Values.Look(ref destinationTile, "destinationTile");
        Scribe_Deep.Look(ref arrivalAction, "arrivalAction");
        Scribe_Values.Look(ref alreadyLeft, "alreadyLeft");
        Scribe_Values.Look(ref createWorldObject, "createWorldObject", true);
        Scribe_Defs.Look(ref worldObjectDef, "worldObjectDef");
    }

    protected override void LeaveMap()
    {
        if (alreadyLeft || !createWorldObject)
        {
            base.LeaveMap();
            return;
        }

        if (groupID < 0)
        {
            Log.Error("Drop pod left the map, but its group ID is " + groupID);
            Destroy();
            return;
        }

        if (destinationTile < 0)
        {
            Log.Error("Drop pod left the map, but its destination tile is " + destinationTile);
            Destroy();
            return;
        }

        var lord = TransporterUtility.FindLord(groupID, Map);
        if (lord != null)
        {
            Map.lordManager.RemoveLord(lord);
        }

        var travelingTransportPods =
            (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(worldObjectDef ??
                                                                     WorldObjectDefOf.TravelingTransportPods);
        //TravelingTransportPods travelingTransportPods = (TravelingTransportPods)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("TravellingAirship", true));
        travelingTransportPods.Tile = Map.Tile;
        travelingTransportPods.SetFaction(Faction.OfPlayer);
        travelingTransportPods.destinationTile = destinationTile;
        travelingTransportPods.arrivalAction = arrivalAction;
        Find.WorldObjects.Add(travelingTransportPods);
        tmpActiveDropPods.Clear();
        tmpActiveDropPods.AddRange(Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod));
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < tmpActiveDropPods.Count; i++)
        {
            if (tmpActiveDropPods[i] is not AirshipLeaving airshipLeaving || airshipLeaving.groupID != groupID)
            {
                continue;
            }

            airshipLeaving.alreadyLeft = true;
            travelingTransportPods.AddPod(airshipLeaving.Contents, true);
            airshipLeaving.Contents = null;
            airshipLeaving.Destroy();
        }
    }
}