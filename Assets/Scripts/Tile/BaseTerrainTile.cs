using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(ThreadQueue))]

public class BaseTerrainTile : MonoBehaviour
{
    // Editor-visible fields:
    public bool UseNormalMap = true;
    public Texture2D ElevationTexture;
    public Texture2D NormalTexture;

    protected ThreadQueue ThreadQueue;

    public MeshData MeshData { get; set; }

    public int Size { get; set; }

    public TileGenerator TileGenerator { get; set; }

    protected int ZoomLevel { get; set; }

    protected bool IsUseToggle { get; set; }

    public void Awake()
    {
        ThreadQueue = GetComponent<ThreadQueue>();
        Size = 256;
    }

    /**
     * Get relative vertex with vector
     */
    public Vector3 GetRelativeVertex(Vector3 vector, int index)
    {
        return MeshData.Vertices[index] + transform.position - vector;
    }

    /**
     * Get relative uv with vector
     */
    public Vector2 GetRelativeUV(Vector3 vector, int index)
    {
        return MeshData.Vertices[index];
    }

    public void ApplyNormalTexture(Material material)
    {
        // https://docs.unity3d.com/Manual/MaterialsAccessingViaScript.html
        material.EnableKeyword("_NORMALMAP");
        material.SetTexture("_BumpMap", NormalTexture);
    }

    public void RemoveNormalTexture(Material material)
    {
        material.DisableKeyword("_NORMALMAP");
    }
}
