using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, 
 RequireComponent(typeof(MeshFilter)), 
 RequireComponent(typeof(MeshRenderer))]
public class EntityShadow : MonoBehaviour
{
    private const int VERTICES = 16;
    private const float ELLIPSE_RATIO = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Create mesh
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[VERTICES + 1];
        int[] triangles = new int[VERTICES * 3];

        // Create center vertex
        vertices[0] = new Vector3(0, 0);

        for (int i = 0; i < VERTICES; i++)
        {
            // Calculate angle in radians
            float angle = (float)i / VERTICES * 2 * Mathf.PI;

            // Create new vertex
            vertices[i + 1] = new Vector3(
                Mathf.Cos(angle) * 0.5f,
                Mathf.Sin(angle) * 0.5f * ELLIPSE_RATIO);

            // Create new triangle
            triangles[i * 3] = 0; // Center point
            triangles[i * 3 + 1] = i + 1; // New vertex
            triangles[i * 3 + 2] = i + 2; // Next vertex
        }

        // Last triangle should use first vertex
        triangles[triangles.Length - 1] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Set component properties
        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().sortingLayerName = "Shadows";
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
