using System.Collections.Generic;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine;
using System.Collections;

using Assets.Scripts;

public class MushroomEditor : EditorWindow
{
    public int StemAxisDivisions;
    public int StemHeightDivisions;
    public float StemBaseRadius;
    public float StemTopRadius;
    public float StemHeight;

    private List<Vector3> _stemVertices;
    private List<int> _stemIndices;
    private List<Vector2> _stemUvs;

    private List<Vector3> _capVertices;
    private List<int> _capIndices;
    private List<Vector2> _capUvs;

    private GameObject _mushroom;
    private GameObject _mushroomParent;
    private Mesh _stemMesh;
    private Mesh _capMesh;

    private Material _stemMaterial;
    private Material _capMaterial;

    private SplineController _stemSplineController;
    private SplineController _capSplineController;

    private bool _createDebugVertices;

    private Stack<GameObject> _mushrooms = new Stack<GameObject>();
    private int _numberOfTwists;
    private float _curviness;
    private GameObject _stemSplineObject;
    private GameObject _capSplineObject;

    private float _uTiling = 1.0f;
    private float _vTiling = 1.0f;
    private GameObject _stem;
    private GameObject _cap;
    private Vector3 _lastStemNode;
    
    private float _capRadius;
    private float _capHeight;

    [MenuItem("Window/MushroomWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<MushroomEditor>();
    }
	
    public void OnGUI()
    {
        _stemMaterial = EditorGUILayout.ObjectField("Stem Material", _stemMaterial, typeof(Material), false) as Material;
        _capMaterial = EditorGUILayout.ObjectField("Cap Material", _capMaterial, typeof(Material), false) as Material;
       
        //_capSplineController = EditorGUILayout.ObjectField("CapSplineController", _capSplineController, typeof(SplineController), true) as SplineController;
        
        StemAxisDivisions = (int)EditorGUILayout.Slider("StemAxisDivisions", StemAxisDivisions, 3, 32);
        StemHeightDivisions = (int)EditorGUILayout.Slider("StemHeightDivisions", StemHeightDivisions, 1, 128);

        StemBaseRadius = EditorGUILayout.Slider("StemBaseRadius", StemBaseRadius, 0.01f, 50);
        StemTopRadius = EditorGUILayout.Slider("StemTopRadius", StemTopRadius, 0.01f, 50);

        StemHeight = EditorGUILayout.Slider("StemHeight", StemHeight, 0.1f, 100f);

        _numberOfTwists = (int)EditorGUILayout.Slider("Number Of Twists", _numberOfTwists, 0f, 10f);
        _curviness = EditorGUILayout.Slider("Curviness", _curviness, 0f, 5f);

        _uTiling = EditorGUILayout.Slider("u Tiling", _uTiling, 0f, 30.0f);
        _vTiling = EditorGUILayout.Slider("v Tiling", _vTiling, 0f, 30.0f);

        _capRadius = EditorGUILayout.Slider("Cap Radius", _capRadius, 0.01f, 30.0f);
        _capHeight = EditorGUILayout.Slider("Cap Height", _capHeight, 0.01f, 30.0f);

        _createDebugVertices = GUILayout.Toggle(_createDebugVertices, "Create Debug Vertices");

        if (GUILayout.Button("Generate Spline"))
        {
            GenerateStemSpline();
            GenerateCapSpline();
        }

        if (GUILayout.Button("Generate Mushroom"))
            GenerateMushroom();

        if (GUILayout.Button("Destroy Last Mushroom"))
            DestroyLastMushroom();
    }

    private void DestroyLastMushroom()
    {
        if (_mushrooms.Count == 0)
        {
            Debug.Log("No mushroom to delete!");
            return;
        }
        
        Debug.Log("Deleting Mushroom!");
        var mushroom = _mushrooms.Pop();
        DestroyImmediate(mushroom);
    }

    private void GenerateMushroom()
    {
        //GenerateStemSpline();

        if (_stemSplineObject == null)
        {
            Debug.LogError("SplineController must be created before generating mushroom!");
            return;
        }

        Debug.Log("Generating Mushroom");

        _mushroom = new GameObject("Mushroom");
        _stem = new GameObject("Stem");
        _stem.transform.parent = _mushroom.transform;

        _stem.AddComponent<MeshFilter>();
        _stem.AddComponent<MeshRenderer>();

        _stemMesh = new Mesh();

        _stemVertices = new List<Vector3>();
        _stemIndices = new List<int>();
        _stemUvs = new List<Vector2>();
        _stemMesh.Clear();

        GenerateStemVertices();
        GenerateIndices(_stemIndices);

        _stemMesh.vertices = _stemVertices.ToArray();
        _stemMesh.triangles = _stemIndices.ToArray();
        _stemMesh.uv = _stemUvs.ToArray();

        _stemMesh.RecalculateBounds();
        _stemMesh.RecalculateNormals();

        FixNormals(_stemMesh);
        CalculateMeshTangents(_stemMesh);

        _stem.GetComponent<MeshFilter>().mesh = _stemMesh;
        _stem.renderer.sharedMaterial = _stemMaterial;

        _cap = new GameObject("Cap");
        _cap.transform.parent = _mushroom.transform;

        _cap.AddComponent<MeshFilter>();
        _cap.AddComponent<MeshRenderer>();

        _capMesh = new Mesh();

        _capVertices = new List<Vector3>();
        _capIndices = new List<int>();
        _capUvs = new List<Vector2>();
        _capMesh.Clear();

        GenerateCapVertices();
        GenerateIndices(_capIndices);

        _capMesh.vertices = _capVertices.ToArray();
        _capMesh.triangles = _capIndices.ToArray();
        _capMesh.uv = _capUvs.ToArray();

        _capMesh.RecalculateBounds();
        _capMesh.RecalculateNormals();

        FixNormals(_capMesh);
        CalculateMeshTangents(_capMesh);

        _cap.GetComponent<MeshFilter>().mesh = _capMesh;
        _cap.renderer.sharedMaterial = _capMaterial;

        _cap.AddComponent<MushroomColorChooser>();
        _stem.AddComponent<MushroomColorChooser>();

        if (_createDebugVertices)
            DrawVertices();

        _mushrooms.Push(_mushroom);
        Debug.Log(string.Format("Number of mushrooms: {0}", _mushrooms.Count));

        //_stemSplineController.transform.parent = _mushroom.transform;

        DestroyImmediate(_stemSplineObject);
        DestroyImmediate(_capSplineObject);

        //TestSplineController();
    }

    private void GenerateStemVertices()
    {
        for (var i = 0f; i <= 1.001f; i += 1f / (StemHeightDivisions))
        {
            for (var theta = 0.0f; theta < 2 * Mathf.PI + 0.001f; theta += 2 * Mathf.PI / StemAxisDivisions)
            {
                var vertex = _stemSplineController.GetHermiteAtTime(i);
                
                vertex.x += Mathf.Cos(theta) * Mathf.Lerp(StemBaseRadius, StemTopRadius, i);
                vertex.z += Mathf.Sin(theta) * Mathf.Lerp(StemBaseRadius, StemTopRadius, i);

                _stemVertices.Add(vertex);

                var uv = new Vector2(theta / (2 * Mathf.PI) * _uTiling, i * _vTiling);
                _stemUvs.Add(uv);
            }
        }

        Debug.Log(string.Format("Vertices Generated: " + _stemVertices.Count));
    }

    private void GenerateIndices(List<int> indices)
    {
        for (var j = 0; j < StemHeightDivisions; j++)
        {
            for (var i = 0; i < StemAxisDivisions; i++)
            {
                indices.Add(i + (j * (StemAxisDivisions + 1)));
                indices.Add(i + (j * (StemAxisDivisions + 1)) + StemAxisDivisions + 1);
                indices.Add(i + (j * (StemAxisDivisions + 1)) + StemAxisDivisions + 2);
                
                indices.Add(i + (j * (StemAxisDivisions + 1)));
                indices.Add(i + (j * (StemAxisDivisions + 1)) + StemAxisDivisions + 2);
                indices.Add(i + (j * (StemAxisDivisions + 1)) + 1);
            }
        }

        //Debug.Log(string.Format("Indices Generated: " + _indices.Count));
    }

    private void GenerateCapVertices()
    {
        for (var i = 0f; i <= 1.001f; i += 1f / (StemHeightDivisions))
        {
            var baseVertex = _capSplineController.GetHermiteAtTime(i);
            //_stemVertices.Add(vertex);

            for (var theta = 0.0f; theta < 2 * Mathf.PI + 0.001f; theta += 2 * Mathf.PI / StemAxisDivisions)
            {
                var vertex = new Vector3();

                vertex.x = Mathf.Cos(theta) * baseVertex.x;
                vertex.y = baseVertex.y;
                vertex.z = Mathf.Sin(theta) * baseVertex.x;

                _capVertices.Add(vertex);
  
                var uv = new Vector2(theta / (2 * Mathf.PI) * _uTiling, i * _vTiling);
                _capUvs.Add(uv);
            }
        }

        Debug.Log(string.Format("Vertices Generated: " + _stemVertices.Count));
    }

    public void GenerateStemSpline()
    {
        Debug.Log("Generating Stem Spline");

        _stemSplineObject = new GameObject("StemSplineController");
        _stemSplineObject.AddComponent<SplineInterpolator>();
        _stemSplineController = _stemSplineObject.AddComponent<SplineController>();

        var root = new GameObject("root");
        root.transform.parent = _stemSplineObject.transform;

        _stemSplineController.SplineRoot = root;
        _stemSplineController.AutoClose = false;
        _stemSplineController.AutoStart = false;

        var firstNode = new GameObject("Node0");
        firstNode.transform.parent = root.transform;

        for (var i = 1; i < _numberOfTwists + 2; i++)
        {
            var node = new GameObject(string.Format("Node{0}", i));
            node.transform.parent = root.transform;

            var x = Random.Range(-_curviness, _curviness);
            var y = (i / (_numberOfTwists + 1f)) * StemHeight;
            var z = Random.Range(-_curviness, _curviness);

            if (i == _numberOfTwists + 1)
                x = z = 0;

            node.transform.position = new Vector3(x, y, z);
            _lastStemNode = node.transform.position;
        }

        //TestSplineController();
    }

    public void GenerateCapSpline()
    {
        Debug.Log("Generating Cap Spline");

        _capSplineObject = new GameObject("CapSplineController");
        _capSplineObject.AddComponent<SplineInterpolator>();
        _capSplineController = _capSplineObject.AddComponent<SplineController>();

        var root = new GameObject("root");
        root.transform.parent = _capSplineObject.transform;
        root.transform.position = _lastStemNode + new Vector3(StemTopRadius, 0f, 0f);

        _capSplineController.SplineRoot = root;
        _capSplineController.AutoClose = false;
        _capSplineController.AutoStart = false;

        var node = new GameObject("Node0");
        node.transform.parent = root.transform;
        node.transform.localPosition = new Vector3();

        node = new GameObject("Node1");
        node.transform.parent = root.transform;
        node.transform.localPosition = new Vector3(_capRadius * 0.25f, _capHeight * 0.3f, 0f);

        node = new GameObject("Node2");
        node.transform.parent = root.transform;
        node.transform.localPosition = new Vector3(_capRadius * 0.6f, _capHeight * 0.15f, 0f);

        node = new GameObject("Node3");
        node.transform.parent = root.transform;
        node.transform.localPosition = new Vector3(_capRadius, 0f, 0f);

        node = new GameObject("Node4");
        node.transform.parent = root.transform;
        node.transform.localPosition = new Vector3(_capRadius * 0.8f, _capHeight * 0.65f, 0f);

        node = new GameObject("Node5");
        node.transform.parent = root.transform;
        node.transform.localPosition = new Vector3(_capRadius * 0.45f, _capHeight * 0.95f, 0f);

        node = new GameObject("Node6");
        node.transform.parent = root.transform;
        node.transform.position = new Vector3(0f, StemHeight +  _capHeight , 0f);

//        for (var i = 1; i < 7; i++)
//        {
//            var node = new GameObject(string.Format("Node{0}", i));
//            node.transform.parent = root.transform;
//            node.transform.localPosition = new Vector3();
//
//            var x = Random.Range(-_curviness, _curviness);
//            var y = (i / (_numberOfTwists + 1f)) * StemHeight;
//            var z = Random.Range(-_curviness, _curviness);
//
//            if (i == _numberOfTwists + 1)
//                x = z = 0;
//
//            node.transform.position = new Vector3(x, y, z);
//        }

        //TestSplineController();
    }

    private void DrawVertices()
    {
        foreach (var vertex in _stemVertices)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (sphere != null)
            {
                sphere.transform.position = vertex;
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                sphere.renderer.sharedMaterial.color = Color.red;
                sphere.transform.parent = _mushroom.transform;
            }
        }
        
        foreach (var vertex in _capVertices)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (sphere != null)
            {
                sphere.transform.position = vertex;
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                sphere.renderer.sharedMaterial.color = Color.red;
                sphere.transform.parent = _mushroom.transform;
            }
        }
    }

    public void CalculateMeshTangents(Mesh mesh)
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

        var row = 1;

        for (long a = 0; a < vertexCount; a++)
        {

            var n = normals[a];
            var t = tan1[a];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;

            if (a > 0 && a == row * StemAxisDivisions + row - 1)
            {
                var average = (tangents[a - StemAxisDivisions] + tangents[a]).normalized;

                tangents[a] = average;
                tangents[a - StemAxisDivisions] = average;

                row++;
            }

            var dotProduct = Vector3.Dot(n, tangents[a]);
            if (dotProduct > 0.0001)
            {
                Debug.LogError(string.Format("a: {0} | DotProduct: {1}", a, dotProduct));
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = vertices[a];
            }
        }

        mesh.tangents = tangents;
    }

    private void FixNormals(Mesh mesh)
    {
        var normals = mesh.normals;

        var row = 1;
        for (var i = StemAxisDivisions; i < normals.Length; row++, i = row * StemAxisDivisions + (row - 1))
        {
            Debug.Log("i: " + i);

            var average = (normals[i - StemAxisDivisions] + normals[i]).normalized;

            normals[i] = average;
            normals[i - StemAxisDivisions] = average;
        }

        mesh.normals = normals;
    }

    public void TestSplineController()
    {
        for (var i = 0f; i <= 1.0f; i += 0.1f)
        {
            var position = _capSplineController.GetHermiteAtTime(i);

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            
            Debug.Log(string.Format("i: {0}", i));
            Debug.Log(string.Format("Interp: {0}", position));
        }
    }
}
