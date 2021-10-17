using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    [Serializable]
    public struct Octave {
        public Vector2 speed;
        public Vector2 scale;
        public float height;
        public bool enabled;
    }

    [Range(2, 100)] public int linesX = 10;
    [Range(2, 100)] public int linesZ = 20;
    [Range(0.01f, 10f)] public float densityX = 1f;
    [Range(0.01f, 10f)] public float densityZ = 1f;
    [Range(0.01f, 100f)] float UVscale = 1f;
    public Octave[] ocataves;

    protected MeshFilter meshFilter;
    protected Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        mesh.name = gameObject.name;
        generateMesh();
        mesh.RecalculateBounds();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private void generateMesh() {
        // Vertices.
        var verts = new Vector3[linesX * linesZ];
        var uv = new Vector2[linesX * linesZ];
        for (int x = 0; x < linesX; x++) {
            for (int z = 0; z < linesZ; z++) {
                int idx = index(x, z, linesZ);
                // Vertex.
                verts[idx] = new Vector3(x * densityX, 0, z * densityZ);
                // UVs.
                float u = (x / UVscale) % 2;
                float v = (z / UVscale) % 2;
                // Flip every 2nd tile to make it look seamless.
                if (u > 1) u = 2 - u;
                if (v > 1) v = 2 - v;
                uv[idx] = new Vector2(u, v);
            }
        }
        mesh.vertices = verts;
        mesh.uv = uv;

        // Triangles.
        var tris = new int[(linesX-1) * (linesZ-1) * 6];
        for (int x = 0; x < linesX - 1; x++) {
            for (int z = 0; z < linesZ - 1; z++) {
                int idx = index(x, z, linesZ - 1) * 6;
                tris[  idx] = index(x+0, z+0, linesZ);
                tris[++idx] = index(x+1, z+1, linesZ);
                tris[++idx] = index(x+1, z+0, linesZ);
                tris[++idx] = index(x+0, z+0, linesZ);
                tris[++idx] = index(x+0, z+1, linesZ);
                tris[++idx] = index(x+1, z+1, linesZ);
            }
        }
        mesh.triangles = tris;
    }

    private static int index(int x, int z, int linesZ) {
        return x * linesZ + z;
    }

    void Update()
    {
        var verts = mesh.vertices;
        for (int x = 0; x < linesX; x++) {
            for (int z = 0; z < linesZ; z++) {
                float y = 0f;
                foreach (var octave in ocataves) {
                    if (!octave.enabled) continue;
                    var noise = Mathf.PerlinNoise((x * octave.scale.x) / linesX, (z * octave.scale.y) / linesZ) * Mathf.PI * 2f;
                    y += Mathf.Cos(noise + octave.speed.magnitude * Time.time) * octave.height;
                }
                verts[index(x, z, linesZ)].y = y;
            }
        }
        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public float? getHeight (Vector3 worldPoint) {
        Vector3 point = worldPoint - transform.position; // Make local
        //Debug.LogFormat("HEIGHT: point {0} -> {1}", worldPoint, point);
        float wx = linesX * densityX; // Total width.
        float wz = linesZ * densityZ; // Total height.
        // Check if outside bounds.
        if ((point.x < 0) || (point.x >= wx) || (point.z < 0) || (point.z >= wz)) {
            //Debug.LogFormat("HEIGHT: pt ({0}, {1}) outside bounds ({2}, {3})", point.x, point.z, wx, wz);
            return null;
        }
        // Calculate cell coordinates.
        //Debug.LogFormat("HEIGHT: wx {0}, wz {1}", wx, wz);
        int cellX = (int)Mathf.Floor(point.x);
        int cellZ = (int)Mathf.Floor(point.z);
        //Debug.LogFormat("HEIGHT: cx {0}, cz {1}", cellX, cellZ);
        // Get 4 vertices around.
        var verts = mesh.vertices;
        var v1 = verts[index(cellX+0, cellZ+0, linesZ)];
        var v2 = verts[index(cellX+1, cellZ+0, linesZ)];
        var v3 = verts[index(cellX+0, cellZ+1, linesZ)];
        var v4 = verts[index(cellX+1, cellZ+1, linesZ)];
        //Debug.LogFormat("HEIGHT: {0}, {1}, {2}, {3}", v1, v2, v3, v4);
        // Get distances.
        var p1 = new Vector2(v1.x, v1.z);
        var p2 = new Vector2(v2.x, v2.z);
        var p3 = new Vector2(v3.x, v3.z);
        var p4 = new Vector2(v4.x, v4.z);
        var p = new Vector2(point.x, point.z);
        //Debug.LogFormat("HEIGHT: 2D {0}, {1}, {2}, {3} -> {4}", p1, p2, p3, p4, p);
        var d1 = Vector2.Distance(p, p1);
        var d2 = Vector2.Distance(p, p2);
        var d3 = Vector2.Distance(p, p3);
        var d4 = Vector2.Distance(p, p4);
        //Debug.LogFormat("HEIGHT: dist {0}, {1}, {2}, {3}", d1, d2, d3, d4);
        //Debug.LogFormat("HEIGHT: dist/diag {0}, {1}, {2}, {3}", d1 / 1.414214f, d2 / 1.414214f, d3 / 1.414214f, d4 / 1.414214f);
        float averageY = (v1.y * d1 + v2.y * d2 + v3.y * d3 + v4.y * d4) / 1.414214f;
        //Debug.LogFormat("HEIGHT: avgY {0}", averageY);
        {
            vv1 = v1; vv2 = v2; vv3 = v3; vv4 = v4; wwp = worldPoint;
            hhh = new Vector3(worldPoint.x, averageY, worldPoint.z);
        }
        return averageY;
    }

    Vector3 vv1, vv2, vv3, vv4, wwp, hhh;


    private void OnDrawGizmos() {
        if (mesh == null) {
            mesh = new Mesh();
            mesh.name = gameObject.name;
            generateMesh();
            mesh.RecalculateBounds();
        }
        Gizmos.color = Color.blue;
        Vector3 size = mesh.bounds.size;
        foreach (var octave in ocataves) {
            if (size.y < octave.height) size.y = octave.height;
        }
        size.y *= 2;
        Gizmos.DrawWireCube(mesh.bounds.center, size);

        if (Application.isPlaying) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(vv1, 0.1f);
            Gizmos.DrawWireSphere(vv2, 0.1f);
            Gizmos.DrawWireSphere(vv3, 0.1f);
            Gizmos.DrawWireSphere(vv4, 0.1f);
            Gizmos.DrawLine(vv1, vv2);
            Gizmos.DrawLine(vv2, vv3);
            Gizmos.DrawLine(vv3, vv4);
            Gizmos.DrawLine(vv4, vv1);
            Gizmos.DrawLine(vv1, vv3);
            Gizmos.DrawLine(vv2, vv4);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(wwp, 0.1f);
            Gizmos.DrawLine(wwp, hhh);
        }
    }

}
