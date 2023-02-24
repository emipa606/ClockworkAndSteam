using Verse;

namespace Clockwork;

public class CompProperties_SteamGenerator : CompProperties_CompSteam
{
    public SoundDef soundAmbientProducingSteam;
    public float steamGeneration = 1;

    public CompProperties_SteamGenerator()
    {
        compClass = typeof(ThingComp_SteamGenerator);
    }
}