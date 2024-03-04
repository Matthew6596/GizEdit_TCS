using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoMeshes : MonoBehaviour
{
    static public Mesh CubeMesh(Vector3 scale)
    {
        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (scale.x, 0, 0),
            new Vector3 (scale.x, scale.y, 0),
            new Vector3 (0, scale.y, 0),
            new Vector3 (0, scale.y, scale.z),
            new Vector3 (scale.x, scale.y, scale.z),
            new Vector3 (scale.x, 0, scale.z),
            new Vector3 (0, 0, scale.z),
        };

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        Vector2[] uvs = {
            new Vector2(0, 0.66f),
            new Vector2(0.25f, 0.66f),
            new Vector2(0, 0.33f),
            new Vector2(0.25f, 0.33f),

            new Vector2(0.5f, 0.66f),
            new Vector2(0.5f, 0.33f),
            new Vector2(0.75f, 0.66f),
            new Vector2(0.75f, 0.33f),
        };

        Mesh mesh = new();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
