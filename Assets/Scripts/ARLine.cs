using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
public class ARLine 
{

    private int positionCount = 0;

    private Vector3 prevPointDistance = Vector3.zero;
    
    private LineRenderer LineRenderer { get; set; }

    public GameObject go { get; set; }

    private MeshCollider MeshCollider { get; set; }

    public ARLine(Transform parent, ARAnchor anchor, Vector3 position, float width, Color color, Material material)
    {
        positionCount = 2;
        go = new GameObject($"LineRenderer");
        
        go.transform.parent = anchor?.transform ?? parent;
        go.transform.position = position;
        go.tag = "Line";
        
        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        MeshCollider goMeshCollider = go.AddComponent<MeshCollider>();
        goLineRenderer.startWidth = width;
        goLineRenderer.endWidth = width;

        goLineRenderer.startColor = color;
        goLineRenderer.endColor = color;

        goLineRenderer.material = material;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.positionCount = positionCount;

        goLineRenderer.numCornerVertices = 5;
        goLineRenderer.numCapVertices = 5;

        goLineRenderer.SetPosition(0, position);
        goLineRenderer.SetPosition(1, position);

        MeshCollider = goMeshCollider;
        LineRenderer = goLineRenderer;
    }

    public ARLine(LineRenderer lr)
    {
        this.LineRenderer = lr;

    }

    public void AddPoint(Vector3 position)
    {
        if(prevPointDistance == null)
            prevPointDistance = position;

        if(prevPointDistance != null && Mathf.Abs(Vector3.Distance(prevPointDistance, position)) >= 0.001f)
        {
            prevPointDistance = position;
            positionCount++;
            LineRenderer.positionCount = positionCount;

            // index 0 positionCount must be - 1
            LineRenderer.SetPosition(positionCount - 1, position);

        }   
    }

    public void BakeTheMesh()
    {
        Mesh mesh = new Mesh();
        LineRenderer.BakeMesh(mesh, true);
        MeshCollider.sharedMesh = mesh;
    }
}