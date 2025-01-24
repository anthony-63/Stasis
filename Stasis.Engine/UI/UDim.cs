using Raylib_cs;

namespace Stasis.Engine.UI;

public struct UDim {
    public float Scale { get; set; }
    public float Offset { get; set; }

    public static UDim Zero => new UDim(0, 0);

    public UDim(float scale, float offset) {
        Scale = scale;
        Offset = offset;
    }
}

public struct UDim2 {
    public UDim X;
    public UDim Y;

    public static UDim2 Zero => new UDim2(0, 0, 0, 0);
    public static UDim2 Fill => new UDim2(1, 0, 1, 0);

    public UDim2(UDim x, UDim y) {
        X = x;
        Y = y;
    }
    public UDim2(float xScale, float xOffset, float yScale, float yOffset) {
        X = new UDim(xScale, xOffset);
        Y = new UDim(yScale, yOffset);
    }

    public static UDim2 ScaleByPixels(float xPixels, float yPixels) {
        return new UDim2(xPixels / Raylib.GetRenderWidth(), 0, yPixels / Raylib.GetRenderHeight(), 0f);
    }
}