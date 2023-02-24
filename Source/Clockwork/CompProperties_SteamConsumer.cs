namespace Clockwork;

public class CompProperties_SteamConsumer : CompProperties_CompSteam
{
    public float minPressure = 100;
    public float steamConsumption = 1;

    public CompProperties_SteamConsumer()
    {
        compClass = typeof(ThingComp_SteamConsumer);
    }
}