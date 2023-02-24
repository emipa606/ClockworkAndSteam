using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Clockwork;

public class SteamSystem : IExposable, ILoadReferenceable
{
    //P = ((M/18) * 1000 * 8.20574587 × 10-5 * 373.15K ) / V In atm. Multiply by 101.325 for kPA
    public static float DENSITY_OF_STEAM = 0.6f; //In kg/m3

    public static float
        STEAM_CONSTANT =
            172.363623769f; //Calculated from molucular mass of steam (18 kg/kmol) and gas constant (8.314) assuming 100C (373.15K).

    private readonly List<ThingComp_SteamConsumer> steamConsumerList = new List<ThingComp_SteamConsumer>();

    private readonly List<ThingComp_SteamGenerator> steamGeneratorList = new List<ThingComp_SteamGenerator>();

    public bool insufficentSteam = false;

    private bool loaded;
    public float steamConsumptionSum; //In kg/h

    public float steamGenerationSum; //In kg/h
    public float steamMass; //In kg

    public SteamSystemMapComp steamSystemMapComp = null;

    public List<CompSteam> steamThingList = new List<CompSteam>();
    //public float systemMinPressure = 0; //In kPa
    //public float systemMinConsumption = 0; //In kg/h

    public float steamVolume; //In m3

    protected int SystemID = -1;
    public float systemMaxMass; //In kg
    public float systemMaxPressure; //In kPa
    public float systemPressure; //In kPa

    public float systemVolume; //In m3

    public SteamSystem()
    {
        if (SystemID == -1)
        {
            SystemID = Find.UniqueIDsManager.GetNextThingID();
        }
    }

    public void ExposeData()
    {
        if (SystemID == -1)
        {
            SystemID = Find.UniqueIDsManager.GetNextThingID();
        }

        Scribe_Values.Look(ref steamMass, "steamMass");

        Scribe_Values.Look(ref SystemID, "SystemID", -1);

        if (loaded)
        {
            return;
        }

        UpdateSystem();
        loaded = true;
    }

    public string GetUniqueLoadID()
    {
        return "SteamSystem_" + SystemID;
    }

    public void RegisterSteamThing(CompSteam steamThing)
    {
        if (!steamThingList.Contains(steamThing))
        {
            steamThingList.Add(steamThing);
        }

        if (steamThing is ThingComp_SteamGenerator generator && !steamGeneratorList.Contains(steamThing))
        {
            steamGeneratorList.Add(generator);
        }

        if (steamThing is ThingComp_SteamConsumer thing && !steamConsumerList.Contains(steamThing))
        {
            steamConsumerList.Add(thing);
        }

        UpdateSystem();
    }

    public void UnregisterSteamThing(CompSteam steamThing)
    {
        steamThingList.Remove(steamThing);
        UpdateSystemConnections();
    }

    public void UpdateSystem()
    {
        try
        {
            systemVolume = steamThingList.Sum(x => x.Props.volume);
            systemMaxPressure = steamThingList.Max(x => x.Props.maxPressure);
            systemMaxMass = systemMaxPressure * systemVolume / STEAM_CONSTANT;
        }
        catch
        {
            // ignored
        }
    }

    public void UpdateSystemConnections()
    {
        steamGeneratorList.Clear();
        steamConsumerList.Clear();

        foreach (var steamThing in steamThingList)
        {
            steamThing.steamSystem = null;
        }

        steamThingList[0].steamSystem = this;

        var oldSteamThingList = new List<CompSteam>(steamThingList);
        steamThingList.Clear();

        foreach (var steamThing in oldSteamThingList)
        {
            steamThing.RegisterToSteamSystem();
        }

        UpdateSystem();
    }

    public void MergeSystem(SteamSystem system)
    {
        steamMass += system.steamMass;
        system.steamMass = 0;


        foreach (var steamThing in system.steamThingList)
        {
            if (!steamThingList.Contains(steamThing))
            {
                RegisterSteamThing(steamThing);
            }

            steamThing.steamSystem = this;
        }

        system.steamSystemMapComp?.UnregisterSteamSystem(system);

        UpdateSystem();
    }

    public void Tick()
    {
        steamGenerationSum = steamGeneratorList.Sum(x => x.steamOutput);
        steamConsumptionSum = steamConsumerList.Sum(x => x.steamNeeded);

        if (steamGenerationSum <= 0)
        {
            steamMass -= 0.01f; // Steam condenses in system
        }
        //else if ((steamGenerationSum < systemMinConsumption) && (systemPressure >= systemMinPressure)) 
        //{
        //    insufficentSteam = true;
        //    return;
        //}
        //else
        //{
        //    insufficentSteam = false;
        //}

        steamMass += (steamGenerationSum - steamConsumptionSum) / 2500f; //Divide by ticks per hour
        steamMass = Mathf.Clamp(steamMass, 0f, systemMaxMass);

        steamVolume = steamMass / DENSITY_OF_STEAM;
        steamVolume = Mathf.Clamp(steamVolume, 0f, systemVolume);

        if (steamVolume == systemVolume)
        {
            systemPressure = steamMass * STEAM_CONSTANT / systemVolume;
            systemPressure = Mathf.Clamp(systemPressure, 0f, systemMaxPressure);
        }
        else
        {
            systemPressure = 0;
        }
    }

    public int ShowSystemID()
    {
        return SystemID;
    }
}

//public class SteamPipeDecorative : ThingComp
//{
//    public void PrintForSteamGrid(SectionLayer layer)
//    {
//        SteamOverlayMats.LinkedOverlayGraphic.Print(layer, parent, 0f);
//    }
//}

//public class CompProperties_CompSteamPipeDecorative : CompProperties
//{
//    public bool pipe = true;
//    public CompProperties_CompSteamPipeDecorative()
//    {
//        compClass = typeof(SteamPipeDecorative);
//    }
//}