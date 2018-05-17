using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class TileGenerator : MonoBehaviour {

    public GameObject tilePref;

    public InputField zoomLevelIF;
    public InputField xCoorIF;
    public InputField yCoorIF;

    public InputField xNumIF;
    public InputField yNumIF;

    public int TerrainSize;

    public Toggle usePNGtoggle;

    private bool isUseToggle = false;

    public Dictionary<Vector2i, NormalTerrainTile> LoadedNormalTiles;

    private ThreadQueue threadQueue;

    public GameObject MeshGOPref;

    void Start()
    {
        threadQueue = GetComponent<ThreadQueue>();
    }

    // Runs once per frame.
    public void Update()
    {
        // When the mouse button is held, use the mouse position to rotate the model.
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(Vector3.up, -Input.GetAxis("Mouse X") * 10.0f);
        }

    }

    void create(Vector2i coordinate, Vector3 pos)
    {
        //StartCoroutine(CheckURLValid(coordinate, pos));
        NormalTerrainTile terrainTile =  CreateNormalTerrainTile(pos, coordinate);
        int zoomLevel = int.Parse(zoomLevelIF.text);
        terrainTile.CreateMesh(zoomLevel, isUseToggle);
    }

    public GameObject CreateMeshGO(Transform parent)
    {
        GameObject go = Instantiate(MeshGOPref, parent.position, Quaternion.identity);
        go.transform.SetParent(parent);
        return go;
    }

    public NormalTerrainTile CreateNormalTerrainTile(Vector3 position, Vector2i coordinate)
    {
        GameObject aTerrain = Instantiate(tilePref, position, Quaternion.identity);
        aTerrain.transform.SetParent(transform, false);
        NormalTerrainTile terrainTile = aTerrain.AddComponent<NormalTerrainTile>();
        terrainTile.TileGenerator = this;
        terrainTile.Coordinate = coordinate;
        terrainTile.Size = TerrainSize;
        return terrainTile;
    }

    public EdgeTerrainTile CreateEdgeTerrainTile(Vector3 position)
    {
        GameObject aTerrain = Instantiate(tilePref, position, Quaternion.identity);
        aTerrain.transform.SetParent(transform, false);
        EdgeTerrainTile terrainTile = aTerrain.AddComponent<EdgeTerrainTile>();
        terrainTile.TileGenerator = this;
        return terrainTile;
    }

    public void AddNewLoadedTerrainTile(Vector2i coordinate, NormalTerrainTile terrainTile)
    {
        lock(LoadedNormalTiles)
        {
            if (coordinate != null)
            {
                LoadedNormalTiles.Add(coordinate, terrainTile);
            }
        }
    }

    public void CheckNeighBour(Vector2i coordinate)
    {
        lock(LoadedNormalTiles)
        {
            NormalTerrainTile terrainTile;
            if (!LoadedNormalTiles.TryGetValue(coordinate, out terrainTile))
            {
                return;
            }

            NormalTerrainTile otherTerrain = null;
            Vector2i neightbourCoor;

            //Get Terran Left
            neightbourCoor = new Vector2i(coordinate.x - 1, coordinate.y);
            NeighBoursDirection direction = NeighBoursDirection.LEFT;
            if (LoadedNormalTiles.TryGetValue(neightbourCoor, out otherTerrain))
            {
                direction = NeighBoursDirection.LEFT;
                //Create edge tile map
                EdgeTerrainTile neighbour = CreateEdgeTerrainTile(terrainTile.transform.position - transform.position);
                neighbour.CreateMesh(terrainTile, direction, otherTerrain);
            }

            //Get Terran Top
            neightbourCoor = new Vector2i(coordinate.x, coordinate.y - 1);

            if (LoadedNormalTiles.TryGetValue(neightbourCoor, out otherTerrain))
            {
                direction = NeighBoursDirection.TOP;
                //Create edge tile map
                EdgeTerrainTile neighbour = CreateEdgeTerrainTile(terrainTile.transform.position - transform.position);
                neighbour.CreateMesh(terrainTile, direction, otherTerrain);
            }

            //Get Terran Right
            neightbourCoor = new Vector2i(coordinate.x + 1, coordinate.y);

            if (LoadedNormalTiles.TryGetValue(neightbourCoor, out otherTerrain))
            {
                direction = NeighBoursDirection.RIGHT;
                //Create edge tile map
                EdgeTerrainTile neighbour = CreateEdgeTerrainTile(terrainTile.transform.position - transform.position);
                neighbour.CreateMesh(terrainTile, direction, otherTerrain);
            }

            //Get Terran Bottom
            neightbourCoor = new Vector2i(coordinate.x, coordinate.y + 1);

            if (LoadedNormalTiles.TryGetValue(neightbourCoor, out otherTerrain))
            {
                direction = NeighBoursDirection.BOTTOM;
                //Create edge tile map
                EdgeTerrainTile neighbour = CreateEdgeTerrainTile(terrainTile.transform.position - transform.position);
                neighbour.CreateMesh(terrainTile, direction, otherTerrain);
            }

        }
    }

    public void CreateExampleMap()
    {
        LoadedNormalTiles = new Dictionary<Vector2i, NormalTerrainTile>();
        double lon = 139.819038;
        double lat = 35.666048;
        int xCoor = CoordinatesConverter.LonToX(lon, 14);
        int yCoor = CoordinatesConverter.LatToY(lat, 14);

        xCoor = 14555;
        yCoor = 6452;

        int xNum = 1;
        int yNum = 1;

        isUseToggle = usePNGtoggle.isOn;

        Vector3 startPos = new Vector3(0, 0, 0);

        //draw map
        for (int i = xCoor; i < xCoor + xNum; i++)
        {
            for (int j = yCoor; j < yCoor + yNum; j++)
            {
                create(new Vector2i(i, j), startPos);
                startPos.z += TerrainSize;
            }
            startPos.x += TerrainSize;
            startPos.z = 0;
        }
    }

    public void createMap()
    {
        LoadedNormalTiles = new Dictionary<Vector2i, NormalTerrainTile>();

        int xCoor = int.Parse(xCoorIF.text);
        int yCoor = int.Parse(yCoorIF.text);

        int xNum = int.Parse(xNumIF.text);
        int yNum = int.Parse(yNumIF.text);

        isUseToggle = usePNGtoggle.isOn;

        Vector3 startPos = new Vector3(0, 0, 0);

        //draw map
        for (int i = xCoor; i < xCoor + xNum; i++)
        {
            for (int j = yCoor; j < yCoor + yNum; j++)
            {
                create(new Vector2i(i,j), startPos);
                startPos.z += TerrainSize;
            }
            startPos.x += TerrainSize;
            startPos.z = 0;
        }
    }
}
