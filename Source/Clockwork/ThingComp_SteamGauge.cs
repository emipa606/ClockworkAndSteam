using System;
using Verse;

namespace Clockwork;

public class ThingComp_SteamGauge : CompSteam
{
    public override string CompInspectStringExtra()
    {
        var str = "CW.Unregistered".Translate();
        if (registered)
        {
            str = "CW.SteamFullInfo".Translate(Math.Round(steamSystem.steamMass), Math.Round(steamSystem.systemMaxMass),
                steamSystem.steamGenerationSum - steamSystem.steamConsumptionSum, steamSystem.systemMaxPressure);
        }

        return str + "\n" + base.CompInspectStringExtra();
    }
}