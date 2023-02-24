using RimWorld;
using Verse;

namespace Clockwork;

public class ThoughtWorker_LivingAutomaton : ThoughtWorker
{
    protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
    {
        if (!p.RaceProps.Humanlike)
        {
            return false;
        }

        if (!p.story.traits.HasTrait(TraitDefOf.LivingAutomaton))
        {
            return false;
        }

        if (!RelationsUtility.PawnsKnowEachOther(p, other))
        {
            return false;
        }

        if (other.def != p.def)
        {
            return false;
        }

        var num = other.health.hediffSet.CountAddedAndImplantedParts();
        return num > 0 ? ThoughtState.ActiveAtStage(num - 1) : false;
    }
}