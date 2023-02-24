using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Clockwork;

public class MainTabWindow_Windups : MainTabWindow_PawnTable
{
    private static PawnTableDef pawnTableDef;

    protected override PawnTableDef PawnTableDef
    {
        get
        {
            if (pawnTableDef == null)
            {
                pawnTableDef = DefDatabase<PawnTableDef>.GetNamed("Windups");
            }

            return pawnTableDef;
        }
    }

    protected override IEnumerable<Pawn> Pawns => from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
        where p.RaceProps.IsMechanoid
        select p;

    public override void PostOpen()
    {
        base.PostOpen();
        Find.World.renderer.wantedMode = WorldRenderMode.None;
    }
}