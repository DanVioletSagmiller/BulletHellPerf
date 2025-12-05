using UnityEngine;

public static class CurveTextureUtil
{




    /// <summary>
    /// Creates a Texture2D from an AnimationCurve. Works at runtime.
    /// </summary>
    /// 
    public static Texture2D CurveToTexture(
        AnimationCurve curve,
        int width,
        int height,
        Color background,
        Color curveColor,
        float paddingPercent = 0.05f,
        Texture2D tex = null,
        Color[] pixels = null)
    {
        if (curve == null || curve.keys == null || curve.keys.Length == 0)
            return MakeSolid(width, height, background);

        if (tex == null) tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        if (pixels == null) pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = background;

        float tMin = curve.keys[0].time;
        float tMax = curve.keys[curve.keys.Length - 1].time;


        float vMin = curve.keys[0].value;
        float vMax = vMin;
        for (int i = 1; i < curve.keys.Length; i++)
        {
            float v = curve.keys[i].value;
            if (v < vMin) vMin = v;
            if (v > vMax) vMax = v;
        }


        float vRange = Mathf.Max(Mathf.Epsilon, vMax - vMin);
        float pad = vRange * paddingPercent;
        vMin -= pad;
        vMax += pad;
        vRange = vMax - vMin;

        int Index(int x, int y) => y * width + x;

        int prevY = -1;
        for (int x = 0; x < width; x++)
        {
            float t01 = (width == 1) ? 0f : (float)x / (width - 1);
            float t = Mathf.Lerp(tMin, tMax, t01);
            float v = curve.Evaluate(t);

            float vNorm = Mathf.Clamp01((v - vMin) / vRange);
            int y = Mathf.RoundToInt(vNorm * (height - 1));

            if (prevY >= 0)
            {
                int y0 = prevY;
                int y1 = y;
                int step = y1 >= y0 ? 1 : -1;
                for (int yy = y0; yy != y1 + step; yy += step)
                {
                    if (yy < 0 || yy >= height) continue;
                    pixels[Index(x, yy)] = curveColor;
                }
            }
            else
            {
                if (y >= 0 && y < height)
                    pixels[Index(x, y)] = curveColor;
            }

            prevY = y;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    static Texture2D MakeSolid(int width, int height, Color c)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        var pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = c;
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
