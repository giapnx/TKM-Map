using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeightMapReader
{
    float[,] Read(int multiplier, int zoomLevel);
}
