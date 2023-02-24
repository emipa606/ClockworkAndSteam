using UnityEngine;
using Verse;

namespace Clockwork;

[StaticConstructorOnStartup]
public static class SteamOverlayMats
{
    private static readonly Shader TransmitterShader;

    public static readonly Graphic LinkedOverlayGraphic;

    static SteamOverlayMats()
    {
        TransmitterShader = ShaderDatabase.MetaOverlay;
        var graphic = GraphicDatabase.Get<Graphic_Single>("Things/Special/Steam/SteamAtlas", TransmitterShader);
        LinkedOverlayGraphic = GraphicUtility.WrapLinked(graphic, LinkDrawerType.Basic);
    }
}