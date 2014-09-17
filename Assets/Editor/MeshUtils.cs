using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MeshUtils
{
    public static void CalculateMeshTangents(Mesh mesh)
    {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        var uv = mesh.uv;
        var normals = mesh.normals;

        var triangleCount = triangles.Length;
        var vertexCount = vertices.Length;

        var tan1 = new Vector3[vertexCount];
        var tan2 = new Vector3[vertexCount];

        var tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3)
        {
            long i1 = triangles[a + 0];
            long i2 = triangles[a + 1];
            long i3 = triangles[a + 2];

            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];

            var w1 = uv[i1];
            var w2 = uv[i2];
            var w3 = uv[i3];

            var x1 = v2.x - v1.x;
            var x2 = v3.x - v1.x;
            var y1 = v2.y - v1.y;
            var y2 = v3.y - v1.y;
            var z1 = v2.z - v1.z;
            var z2 = v3.z - v1.z;

            var s1 = w2.x - w1.x;
            var s2 = w3.x - w1.x;
            var t1 = w2.y - w1.y;
            var t2 = w3.y - w1.y;

            var r = 1.0f / (s1 * t2 - s2 * t1);

            var sDir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            var tDir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sDir;
            tan1[i2] += sDir;
            tan1[i3] += sDir;

            tan2[i1] += tDir;
            tan2[i2] += tDir;
            tan2[i3] += tDir;
        }

        for (long a = 0; a < vertexCount; a++)
        {
            var n = normals[a];
            var t = tan1[a];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
}
