using Verse;

namespace Clockwork;

public class CompProperties_CompSteam : CompProperties
{
    public float maxPressure = 100;
    public bool pipe = false;
    public float volume = 1;

    public CompProperties_CompSteam()
    {
        compClass = typeof(CompSteam);
    }
}