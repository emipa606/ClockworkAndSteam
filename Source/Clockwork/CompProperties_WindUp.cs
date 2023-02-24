using Verse;

namespace Clockwork;

public class CompProperties_WindUp : CompProperties
{
    public float daysToAssembly = 0.01f;

    public ThingDef equipWeapon;

    public PawnKindDef spawnPawn;

    public CompProperties_WindUp()
    {
        compClass = typeof(ThingComp_Windup);
    }
}