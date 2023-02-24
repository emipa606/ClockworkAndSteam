using RimWorld;
using UnityEngine;
using Verse;

namespace Clockwork;

[StaticConstructorOnStartup]
public class WindupRename : PawnColumnWorker_Label
{
    public static readonly Texture2D rename = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");

    public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
    {
        if (Widgets.ButtonImage(rect, rename))
        {
            if (pawn == null || !pawn.Spawned)
            {
                return;
            }

            if (pawn.Name == null)
            {
                var name = (NameSingle)(pawn.Name = new NameSingle(pawn.Label));
                pawn.Name = name;
            }

            Find.WindowStack.Add(new Dialog_NameWindup(pawn));
        }

        if (Mouse.IsOver(rect))
        {
            GUI.DrawTexture(rect, TexUI.HighlightTex);
        }

        TipSignal tip = pawn.Label;
        tip.text = "RenameWindup".Translate();
        TooltipHandler.TipRegion(rect, tip);
    }

    public override int GetMinWidth(PawnTable table)
    {
        return 30;
    }

    public override int GetOptimalWidth(PawnTable table)
    {
        return Mathf.Clamp(30, GetMinWidth(table), GetMaxWidth(table));
    }

    public override int GetMaxWidth(PawnTable table)
    {
        return Mathf.Min(base.GetMaxWidth(table), GetMinWidth(table));
    }
}