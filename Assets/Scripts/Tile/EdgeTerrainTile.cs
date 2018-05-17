using UnityEngine;
using System;

public class EdgeTerrainTile : BaseTerrainTile
{
    public NormalTerrainTile ThisTerrainTile;

    public NeighBoursDirection Direction;

    public NormalTerrainTile OtherTerrainTile;

    private void CreateMesh(Vector3[,] vertices, Vector2[,] uvs, NeighBoursDirection direction)
    {
        Action createMeshAction = () =>
        {
            MeshData = MeshDataProvider.GenerateMeshData(vertices, uvs, Color.white, direction);

            Action updateMeshAction = () =>
            {

                GameObject go = TileGenerator.CreateMeshGO(transform);

                // Apply or remove the normal map from our material, based on the option set in the editor.
                Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                var material = go.GetComponent<MeshRenderer>().material;
                if (UseNormalMap)
                {
                    ApplyNormalTexture(material);
                }
                else
                {
                    RemoveNormalTexture(material);
                }
                MeshData.UpdateMesh(mesh);
            };

            ThreadQueue.StartFunctionInMainThread(updateMeshAction);
        };
        ThreadQueue.StartThreadFunction(createMeshAction);
    }

    public void CreateMesh(NormalTerrainTile thisTerrainTile, NeighBoursDirection direction, NormalTerrainTile otherTerrainTile)
    {
        int minX = 0, maxX = 0, minY = 0, maxY = 0;

        int maxIndexX = thisTerrainTile.HeightMap.GetLength(0) - 1;
        int maxIndexY = thisTerrainTile.HeightMap.GetLength(1) - 1;

        switch (direction)
        {
            case NeighBoursDirection.BOTTOM:
                minX = 0;
                maxX = maxIndexX;
                minY = maxY = 0;
                break;
            case NeighBoursDirection.LEFT:
                minX = maxX = maxIndexX;
                minY = 0;
                maxY = maxIndexY;
                break;
            case NeighBoursDirection.RIGHT:
                minX = maxX = 0;
                minY = 0;
                maxY = maxIndexY;
                break;
            case NeighBoursDirection.TOP:
                minX = 0;
                maxX = maxIndexX;
                minY = maxY = maxIndexY;
                break;
            default:
                return;
        }
        int thisX, thisY;
        Vector3[] otherVertices = null, thisVertices = null;
        Vector3[,] vertices = null;
        Vector2[] otherUVS = null, thisUVS = null;
        Vector2[,] uvs = null;
        int currentIndex = 0;

        if (maxX - minX > maxY - minY)
        {
            vertices = new Vector3[2, maxX - minX + 1];
            uvs = new Vector2[2, maxX - minX + 1];
            otherVertices = new Vector3[maxX - minX + 1];
            thisVertices = new Vector3[maxX - minX + 1];
            otherUVS = new Vector2[maxX - minX + 1];
            thisUVS = new Vector2[maxX - minX + 1];
        }
        else
        {
            vertices = new Vector3[2, maxY - minY + 1];
            uvs = new Vector2[2, maxY - minY + 1];
            otherVertices = new Vector3[maxY - minY + 1];
            thisVertices = new Vector3[maxY - minY + 1];
            thisUVS = new Vector2[maxY - minY + 1];
            otherUVS = new Vector2[maxY - minY + 1];
        }
        for (int otherY = minY; otherY <= maxY; ++otherY)
        {
            for (int otherX = minX; otherX <= maxX; ++otherX)
            {
                thisX = otherX;
                thisY = otherY;

                if (minX == maxX)
                {
                    thisX = maxIndexX - otherX;
                }
                if (minY == maxY)
                {
                    thisY = maxIndexY - otherY;
                }

                thisVertices[currentIndex] = thisTerrainTile.MeshData.Vertices[thisY * (maxIndexX + 1) + thisX];
                thisUVS[currentIndex] = thisTerrainTile.MeshData.Vertices[thisY * (maxIndexX + 1) + thisX];

                otherVertices[currentIndex] = otherTerrainTile.GetRelativeVertex(thisTerrainTile.transform.position, otherY * (maxIndexX + 1) + otherX);
                otherUVS[currentIndex] = otherTerrainTile.GetRelativeUV(thisTerrainTile.transform.position, otherY * (maxIndexX + 1) + otherX);
                ++currentIndex;
            }
        }

        for (currentIndex = 0; currentIndex < thisVertices.Length; ++currentIndex)
        {
            vertices[0, currentIndex] = thisVertices[currentIndex];
            uvs[0, currentIndex] = thisUVS[currentIndex];
        }

        for (currentIndex = 0; currentIndex < otherVertices.Length; ++currentIndex)
        {
            vertices[1, currentIndex] = otherVertices[currentIndex];
            uvs[1, currentIndex] = otherUVS[currentIndex];
        }

        CreateMesh(vertices, uvs, direction);
    }
}
