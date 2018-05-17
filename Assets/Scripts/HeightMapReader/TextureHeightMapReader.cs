using UnityEngine;
using System;

public class TextureHeightMapReader : IHeightMapReader
{
    private Texture2D texture2D;


    public TextureHeightMapReader(Texture2D texture2D)
    {
        this.texture2D = texture2D;
    }

    public float[,] Read(int multiplier, int zoomLevel)
    {

        const double earthCircumferenceMeters = 6378137.0 * Math.PI * 2.0;
        float tileSize = (float)earthCircumferenceMeters / (1 << zoomLevel);
        int width = texture2D.width;
        int height = texture2D.height;
        float[,] HeightMap = new float[width, height];
        Color color;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                color = texture2D.GetPixel(x, height - y - 1);
                HeightMap[x, y] = ColorToElevation(color) * multiplier;
            }
        }

        return HeightMap;
    }

    private float ColorToElevation(Color color)
    {
        // Convert from color channel values in 0.0-1.0 range to elevation in meters:
        // https://mapzen.com/documentation/terrain-tiles/formats/#terrarium
        float x = color.r * 65536 + color.g * 256 + color.b;
        float h = 0;
        float u = 0.01f;
        if (x < 8388608)
            h = x * u;
        else if (x == 8388608)
            h = 0;
        else
            h = (x - 16777216) * u;
        return h;
        //return (color.r * 256.0f * 256.0f + color.g * 256.0f + color.b);
    }
}
