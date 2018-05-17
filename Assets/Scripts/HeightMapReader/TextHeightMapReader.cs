using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TextHeightMapReader : IHeightMapReader
{
    string dataText; 
    public TextHeightMapReader(string dataText)
    {
        this.dataText = dataText;
    }

    public float[,] Read(int multiplier, int zoomLevel)
    {
        const double earthCircumferenceMeters = 6378137.0 * Math.PI * 2.0;
        float tileSize = (float)earthCircumferenceMeters / (1 << zoomLevel);

        float[,] HeightMap = null;
        string[] eachLines = dataText.Split('\n');
        int width = eachLines.Length - 1;
        int height;
        for (int i = 0; i < eachLines.Length - 1; i++)
        {
            string[] nums = eachLines[i].Split(',');
            if (HeightMap == null)
            {
                height = nums.Length;
                HeightMap = new float[width, height];
            }
            for (int j = 0; j < nums.Length; j++)
            {
                if (!nums[j].Equals("e"))
                {
                    HeightMap[j, i] = float.Parse(nums[j]) * multiplier;
                }
                else
                {
                    HeightMap[j, i] = 0;
                }
            }
        }
        return HeightMap;
    }

}
