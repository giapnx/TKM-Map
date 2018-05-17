using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static string TILE_MAP_FILE_FOLDER = Application.dataPath + "/Resources/Files/TileMap/";

    public static string MESH_DATA_FOLDER = Application.dataPath + "/Resources/Files/MeshData/";

    public static Vector3 TERRAIN_TILE_OFFSET = new Vector3(-0.5f, 0.0f, -0.5f);

    public static Color MESH_COLOR = Color.white;
}
