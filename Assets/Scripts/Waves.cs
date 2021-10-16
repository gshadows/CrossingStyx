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
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }


    private void OnDrawGizmos() {
        if (mesh == null) {
            mesh = new Mesh();
            mesh.name = gameObject.name;
            generateMesh();
        }
        Gizmos.color = Color.blue;
        Vector3 size = mesh.bounds.size;
        foreach (var octave in ocataves) {
            if (size.y < octave.height) size.y = octave.height;
        }
        size.y *= 2;
        Gizmos.DrawWireCube(mesh.bounds.center, size);
    }

}
