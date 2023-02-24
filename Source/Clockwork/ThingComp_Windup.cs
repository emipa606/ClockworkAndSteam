using RimWorld;
using Verse;

namespace Clockwork;

public class ThingComp_Windup : ThingComp
{
    private float assemblyProgress;
    public CompProperties_WindUp Props => (CompProperties_WindUp)props;

    public override void CompTick()
    {
        var num = 1f / (Props.daysToAssembly * 60000f);
        assemblyProgress += num;
        if (assemblyProgress > 1f)
        {
            AssembleWindup();
        }
    }

    public void AssembleWindup()
    {
        try
        {
            var request = new PawnGenerationRequest(Props.spawnPawn, Faction.OfPlayer,
                developmentalStages: DevelopmentalStage.Newborn);
            for (var i = 0; i < parent.stackCount; i++)
            {
                var pawn = PawnGenerator.GeneratePawn(request);
                Name name = new NameSingle(Props.spawnPawn.label);
                pawn.Name = name;
                if (Props.equipWeapon != null && pawn.equipment.Primary == null)
                {
                    var thingWithComps = (ThingWithComps)ThingMaker.MakeThing(Props.equipWeapon);
                    pawn.equipment.AddEquipment(thingWithComps);
                }

                PawnUtility.TrySpawnHatchedOrBornPawn(pawn, parent);
            }
        }
        finally
        {
            parent.Destroy();
        }
    }

    public override string CompInspectStringExtra()
    {
        return "AssemblyProgress".Translate() + ": " + assemblyProgress.ToStringPercent();
    }
}