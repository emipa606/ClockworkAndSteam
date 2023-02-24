using RimWorld;
using Verse;

namespace Clockwork;

public class ThoughtWorker_FancyClothing : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.apparel == null || ThoughtUtility.ThoughtNullified(p, def))
        {
            return ThoughtState.Inactive;
        }

        float fanciness = 0;
        foreach (var app in p.apparel.WornApparel)
        {
            if (app.def is ThingDef_FancyApparel gear_def)
            {
                fanciness += gear_def.fanciness;
            }
        }

        if (fanciness >= 1)
        {
            return ThoughtState.ActiveAtStage(4);
        }

        if (fanciness >= 0.75)
        {
            return ThoughtState.ActiveAtStage(3);
        }

        if (fanciness >= 0.50)
        {
            return ThoughtState.ActiveAtStage(2);
        }

        if (fanciness >= 0.25)
        {
            return ThoughtState.ActiveAtStage(1);
        }

        if (fanciness > 0)
        {
            return ThoughtState.ActiveAtStage(0);
        }

        return ThoughtState.Inactive;
    }
}