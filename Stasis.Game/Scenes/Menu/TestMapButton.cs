using System.Numerics;
using Raylib_cs;
using Stasis.Content.Beatmaps;
using Stasis.Engine.UI;
using Stasis.Engine.UI.Elements;

namespace Stasis.Game.Scenes.Menu;

public class TestMapButton : Button
{
    public Frame HoverFrame = new Frame()
    {
        Size = UDim2.Fill,
        Color = new Color(255, 255, 255, 150),
    };

    public delegate void MapSelectEvent(IBeatmapSet map);

    public IBeatmapSet Map;

    public TestMapButton(IBeatmapSet map, MapSelectEvent SelectMap)
    {
        AddChild(HoverFrame);
        Map = map;
        ClipContents = true;
        PressedOnce += () => SelectMap(map);
    }

    public override void Update(double dt)
    {
        HoverFrame.Roundness = NormalFrame.Roundness;
        HoverFrame.Visible = State == ButtonState.Hovering || State == ButtonState.Pressed;
        base.Update(dt);
    }

    public void Unload()
    {
        foreach (var child in Children)
        {
            if (child is ImageFrame img)
            {
                img.Unload();
            }
        }
    }

    public override void UpdateAbsoluteValues(Vector2 parentSize, Vector2 parentPosition)
    {
        base.UpdateAbsoluteValues(parentSize, parentPosition);
    }

    public override void Render()
    {
        base.Render();
    }
}