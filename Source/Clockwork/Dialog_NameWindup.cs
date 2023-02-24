using RimWorld;
using UnityEngine;
using Verse;

namespace Clockwork;

public class Dialog_NameWindup : Window
{
    private readonly Pawn pawn;
    private string curName;

    public Dialog_NameWindup(Pawn pawn)
    {
        this.pawn = pawn;
        curName = pawn.Name.ToStringShort;
        forcePause = true;
        absorbInputAroundWindow = true;
        closeOnClickedOutside = true;
        closeOnAccept = false;
    }

    private Name CurPawnName => new NameSingle(curName);

    public override Vector2 InitialSize => new Vector2(500f, 175f);

    public override void DoWindowContents(Rect inRect)
    {
        var height = inRect.height;
        var width = inRect.width;
        var enterPressed = false;
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            enterPressed = true;
            Event.current.Use();
        }

        Text.Font = GameFont.Medium;
        var text = CurPawnName.ToString().Replace(" '' ", " ");
        Widgets.Label(new Rect(15f, 15f, 500f, 50f), text);
        Text.Font = GameFont.Small;
        var text2 = Widgets.TextField(new Rect(15f, 50f, (width / 2f) - 20f, 35f), curName);
        if (text2.Length < 16 && CharacterCardUtility.ValidNameRegex.IsMatch(text2))
        {
            curName = text2;
        }

        if (!Widgets.ButtonText(new Rect((width / 2f) + 20f, height - 35f, (width / 2f) - 20f, 35f), "OK".Translate(),
                true, false) && !enterPressed)
        {
            return;
        }

        pawn.Name = CurPawnName;
        Find.WindowStack.TryRemove(this);
        Messages.Message("WindupGainsName".Translate(curName), pawn, MessageTypeDefOf.PositiveEvent, false);
    }
}