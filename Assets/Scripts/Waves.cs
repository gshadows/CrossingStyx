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
        public bool alternate;
    }

    [Range(2, 100)] public int linesX = 10;
    [Range(2, 100)] public int linesZ = 20;
    [Range(0.01f, 10f)] public float densityX = 1f;
    [Range(0.01f, 10f)] public float densityZ = 1f;
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
        for (int x = 0; x < linesX; x++) {
            for (int z = 0; z < linesZ; z++) {
                verts[index(x, z, linesZ)] = new Vector3(x * densityX, 0, z * densityZ);
            }
        }
        mesh.vertices = verts;

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
                verts[index(x, z, linesZ)].y = y;
                foreach (var octave in ocataves) {
                    y += Mathf.Cos(octave.speed.magnitude * Time.time) * octave.height;
                }
            }
        }
        mesh.vertices = verts;
    }
}
