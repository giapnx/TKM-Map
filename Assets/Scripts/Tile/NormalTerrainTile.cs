using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(ThreadQueue))]

public class NormalTerrainTile : BaseTerrainTile
{
    public float[,] HeightMap { get; set; }

    public Vector2i Coordinate { get; set; }

    private string GetURL()
    {
        string sampleURL;

        if (!IsUseToggle)
            sampleURL = "http://cyberjapandata.gsi.go.jp/xyz/dem/" + ZoomLevel + "/";
        else
            sampleURL = "https://cyberjapandata.gsi.go.jp/xyz/dem_png/" + ZoomLevel + "/";

        if (!IsUseToggle)
            sampleURL += Coordinate.x + "/" + Coordinate.y + ".txt";
        else sampleURL += Coordinate.x + "/" + Coordinate.y + ".png";
        return sampleURL;
    }

    public void CreateMesh(int zoomLevel, bool isUseToggle)
    {
        this.ZoomLevel = zoomLevel;
        this.IsUseToggle = isUseToggle;

        StartCoroutine(CreateMeshByTileMapFile());
    }

    /**
     * Create mesh by tile map data
     */
    private IEnumerator CreateMeshByTileMapFile()
    {
        string sampleURL = GetURL();
        // Start a download of the given URL
        WWW www = CacheFile.LoadOrDownLoad(sampleURL, new TileMapUrlParse(Settings.TILE_MAP_FILE_FOLDER), this);

        // Wait for download to complete
        yield return www;
        if (www.error == null)
        {
            //Get HeigtMap
            int multiplier = 1;
            IHeightMapReader heightMapReader;
            if (!IsUseToggle)
            {
                heightMapReader = new TextHeightMapReader(www.text);
            }
            else
            {
                Texture2D texture2D = new Texture2D(www.texture.width, www.texture.height);
                www.LoadImageIntoTexture(texture2D);
                heightMapReader = new TextureHeightMapReader(texture2D);
            }

            //Get height map
            HeightMap = heightMapReader.Read(multiplier, ZoomLevel);
            //Create mesh action
            CreateMeshByHeightMap();
        }
    }

    // Generate a uniform grid of vertices, elevate them according to a texture, and apply a normal map.
    private void CreateMeshByHeightMap()
    {
        Action createMeshAction = () =>
        {
            MeshData = MeshDataProvider.GenerateMeshData(HeightMap, Settings.TERRAIN_TILE_OFFSET, Settings.MESH_COLOR, Size);

            Action updateMeshAction = () =>
            {
                // Apply or remove the normal map from our material, based on the option set in the editor.

                GameObject g1 = TileGenerator.CreateMeshGO(transform);
                GameObject g2 = TileGenerator.CreateMeshGO(transform);

                Mesh mesh1 = g1.GetComponent<MeshFilter>().mesh;
                Mesh mesh2 = g2.GetComponent<MeshFilter>().mesh;
               

                var material1 = g1.GetComponent<MeshRenderer>().material;
                var material2 = g2.GetComponent<MeshRenderer>().material;
                if (UseNormalMap)
                {
                    ApplyNormalTexture(material1);
                    ApplyNormalTexture(material2);
                }
                else
                {
                    RemoveNormalTexture(material2);
                    RemoveNormalTexture(material1);
                }

                TileGenerator.AddNewLoadedTerrainTile(Coordinate, this);
                TileGenerator.CheckNeighBour(Coordinate);

                MeshData.UpdateHalfMesh(mesh1, true);
                MeshData.UpdateHalfMesh(mesh2, false);
            };

            ThreadQueue.StartFunctionInMainThread(updateMeshAction);
        };
        ThreadQueue.StartThreadFunction(createMeshAction);
    }
}

