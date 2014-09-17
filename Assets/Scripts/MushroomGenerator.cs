using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MushroomGenerator : MonoBehaviour
{
    public int StemAxisDivisions;
    public int StemHeightDivisions;
    public float StemRadius;
    public float StemHeight;

    private List<Vector3> _vertices;
    private List<int> _indices;

    private GameObject _mushroom;
    private Mesh _mesh;
    private List<Vector2> _uvs;

    public void Start()
    {
        _mushroom = new GameObject("Mushroom");
        _mushroom.AddComponent<MeshFilter>();
        _mushroom.AddComponent<MeshRenderer>();

        _mesh = new Mesh();

        _vertices = new List<Vector3>();
        _indices = new List<int>();
        _uvs = new List<Vector2>();
        _mesh.Clear();

        GenerateVertices();
        GenerateIndices();
        GenerateUvs();
        //DrawVertices();

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _indices.ToArray();
        _mesh.uv = _uvs.ToArray();

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();

        _mushroom.GetComponent<MeshFilter>().mesh = _mesh;
        _mushroom.renderer.material.color = Color.grey;
    }

    public void Update()
    {
        
    }

    private void GenerateVertices()
    {
        for (var i = 0; i <= StemHeightDivisions; i++)
        {
            for (var theta = 0.0f; theta < 2 * Mathf.PI - 0.001f; theta += 2 * Mathf.PI / StemAxisDivisions)
            {
                var x = Mathf.Cos(theta) * StemRadius;
                var y = i * (StemHeight / StemHeightDivisions);
                var z = Mathf.Sin(theta) * StemRadius;

                var vertex = new Vector3(x, y, z);
                _vertices.Add(vertex);
            }
        }

        Debug.Log(string.Format("Vertices Generated: " + _vertices.Count));
    }

    private void GenerateIndices()
    {
        for (var j = 0; j < StemHeightDivisions; j++)
        {
            for (var i = 0; i < StemAxisDivisions; i++)
            {
                if (i != StemAxisDivisions - 1)
                {
                    _indices.Add(i + (j * StemAxisDivisions));
                    _indices.Add(i + StemAxisDivisions + (j * StemAxisDivisions));
                    _indices.Add(i + 1 + (j * StemAxisDivisions));

                    _indices.Add(i + 1 + (j * StemAxisDivisions));
                    _indices.Add(i + StemAxisDivisions + (j * StemAxisDivisions));
                    _indices.Add(i + StemAxisDivisions + 1 + (j * StemAxisDivisions));
                }

                else
                {
                    _indices.Add(i + (j * StemAxisDivisions));
                    _indices.Add(i + StemAxisDivisions + (j * StemAxisDivisions));
                    _indices.Add(i - (StemAxisDivisions - 1) + (j * StemAxisDivisions));

                    _indices.Add(i - (StemAxisDivisions - 1) + (j * StemAxisDivisions));
                    _indices.Add(i + StemAxisDivisions + (j * StemAxisDivisions));
                    _indices.Add(i + 1 + (j * StemAxisDivisions));
                }
            }
        }

        Debug.Log(string.Format("Indices Generated: " + _indices.Count));
    }
    private void GenerateUvs()
    {
        for (var i = 0; i < _vertices.Count; i++)
        {
            _uvs.Add(new Vector2());
        }

        Debug.Log(string.Format("UVs Generated: " + _uvs.Count));
    }
    private void DrawVertices()
    {
        foreach (var vertex in _vertices)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (sphere != null)
            {
                sphere.transform.position = vertex;
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                sphere.renderer.material.color = Color.red;
                sphere.transform.parent = transform;
            }
        }

    }
}
