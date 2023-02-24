using RimWorld;
using UnityEngine;

namespace Clockwork;

public class Building_CapacitorBank : Building_Battery
{
    private static readonly Vector2 BarSize = new Vector2(0.7f, 0.7f);

    public override void Draw()
    {
        var comp = GetComp<CompPowerBattery>();
        var r = default(ClockworkGraphics.FadingBarRequest);
        r.center = DrawPos + (Vector3.up * 0.1f);
        r.size = BarSize;
        r.fillPercent = comp.StoredEnergy / comp.Props.storedEnergyMax;
        r.filledMat = ClockworkGraphics.CapacitorBarFilledMat;
        r.unfilledMat = ClockworkGraphics.CapacitorBarUnfilledMat;
        r.rotation = Rotation;
        ClockworkGraphics.DrawFadingBar(r);
    }

    public override void Tick()
    {
    }
}