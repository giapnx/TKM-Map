using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class MeshDataProvider
{
    public static MeshData GenerateMeshData(Vector3[,] verticesMatrix, Vector2[,] uvs, Color color, NeighBoursDirection direction)
    {
        int width = verticesMatrix.GetLength(0);
        int height = verticesMatrix.GetLength(1);
        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                meshData.Vertices[vertexIndex] = verticesMatrix[x,y];
                meshData.UVS[vertexIndex] = uvs[x,y];
                meshData.Colors[vertexIndex] = color;

                if (x < width - 1 && y < height - 1)
                {
                    if (direction == NeighBoursDirection.BOTTOM || direction == NeighBoursDirection.LEFT)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }
                    else
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                    }
                }
                vertexIndex++;
            }
        }
        return meshData;
    }

    public static  MeshData  GenerateMeshData(float[,] HeightMap, Vector3 offset, Color color, int size)
    {
        int width = HeightMap.GetLength(0);
        int height = HeightMap.GetLength(1);
        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                meshData.Vertices[vertexIndex] = new Vector3(x / (float)width * size, HeightMap[x,y], y / (float)height * size) + offset;
                meshData.UVS[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                meshData.Colors[vertexIndex] = color;

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                }
                vertexIndex++;

            }
        }

        return meshData;
    }

    public MeshData GenerateMeshData(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException("File not found: " + fileName);
        }
        string jsonData = File.ReadAllText(fileName);

        MeshData meshData = JsonUtility.FromJson<MeshData>(jsonData);
        return meshData;
    }
}

[Serializable]
public class MeshData
{
    public Vector3[] Vertices;

    public int[] Triangles;

    public Vector2[] UVS;

    public Color[] Colors;

    private int triableIndex;

    public int Width, Height;

    public MeshData(int width, int height)
    {
        Width = width;
        Height = height;
        Vertices = new Vector3[width * height];
        Triangles = new int[(width - 1) * (height - 1) * 6];
        UVS = new Vector2[width * height];
        Colors = new Color[width * height];
        triableIndex = 0;
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[triableIndex] = a;
        Triangles[triableIndex + 1] = b;
        Triangles[triableIndex + 2] = c;
        triableIndex += 3;
    }


    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVS;
        mesh.RecalculateNormals();
        return mesh;
    }

    public void UpdateMesh(Mesh mesh)
    {
        mesh.Clear();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVS;
        mesh.RecalculateNormals();
    }

    public void UpdateHalfMesh(Mesh mesh, bool isLast)
    {
        mesh.Clear();
        int halfHeight = (Height / 2) + 1;
        int takeVer = halfHeight * Width;
        int takeTriangle = (halfHeight - 1) * (Width - 1) * 6;
        if (isLast)
        {
            mesh.vertices = Vertices.Skip(takeVer - Width).ToArray();
            var newTriagles = Triangles.Skip(takeTriangle).ToArray();
            int first = newTriagles[0];
            for (int i = 0; i < newTriagles.Length; ++i)
            {
                newTriagles[i] -= first;
            }
            mesh.triangles = newTriagles;
            mesh.uv = UVS.Skip(takeVer - Width).ToArray();
            mesh.RecalculateNormals();
        } else
        {
            mesh.vertices = Vertices.Take(takeVer).ToArray();
            mesh.triangles = Triangles.Take(takeTriangle).ToArray();
            mesh.uv = UVS.Take(takeVer).ToArray();
            mesh.RecalculateNormals();
        }
    }
}