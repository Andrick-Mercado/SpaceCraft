using UnityEngine;

public class PGC_Mesh : MonoBehaviour
{
    //material for mesh
    [SerializeField]
    [Tooltip ("Material for Wave mesh")]
    private Material materialMesh;

    //material for sphere
    [SerializeField]
    [Tooltip("Material for sphere mesh")]
    private Material materialSphere;

    //grid size
    [SerializeField]
    [Tooltip("grid size on x-axis")]
    private int xSize;

    [SerializeField]
    [Tooltip("grid size on z-axis")]
    private int zSize;

    //wave variables
    [SerializeField]
    [Tooltip("Waves height")]
    private float WaveHeight;

    [SerializeField]
    [Tooltip("Waves speed")]
    private float WaveSpeed;

    //holds original heights
    private Vector3[] BaseHeights;

    //components for wave mesh
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private Vector3[] _vertices;
    private Vector2[] _uv;

    //camera to follow ball
    [SerializeField]
    [Tooltip("Camera that will follow the sphere")]
    private Camera _sceneCamera;

    [SerializeField]
    [Tooltip("Offset for camera control")]
    private float xOffsetCamera, yOffsetCamera, zOffsetCamera;

    //sphere object
    private GameObject _sphere;


    private void Awake()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>(); 

    }

    // Start is called before the first frame update
    void Start()
    {
        MeshBuilder();
        //CreateSpherePrimitive();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //create wave effects
        MeshWaves();

        //make camera follow sphere
        // _sceneCamera.transform.position = new Vector3( _sphere.transform.position.x + xOffsetCamera,
        //     _sphere.transform.position.y + yOffsetCamera, _sphere.transform.position.z + zOffsetCamera);
    }

    void MeshBuilder()
    {
        _meshFilter.mesh = _mesh = new Mesh();

        _vertices = new Vector3[( xSize + 1 ) * (zSize + 1 )];
        _uv = new Vector2[_vertices.Length];

        //makes vertices based on xSize and zSize
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                _vertices[i] = new Vector3(x, 0, z);
                _uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
            }
        }

        int[] triangles = new int[xSize * zSize * 6];

        //creates triangles based on vertices above
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        // draw triangles
        _mesh.Clear();
        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();

        // add renderer material
        _meshRenderer.material = materialMesh;

        // add collider to mesh
        _meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    void CreateSpherePrimitive()
    {
        //create a sphere and add it to the middle of the mesh
        _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphere.transform.position = new Vector3(xSize/2, WaveHeight*2, zSize / 2);
        _sphere.AddComponent<Rigidbody>();
        _sphere.GetComponent<MeshRenderer>().material = materialSphere;
    }

    void MeshWaves()
    {
        if (BaseHeights == null)//base case
            BaseHeights = _mesh.vertices;

        Vector3[] vertices = new Vector3[BaseHeights.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = BaseHeights[i];
            vertex.y += (Mathf.Sin(Time.time * WaveSpeed + BaseHeights[i].x + BaseHeights[i].y + BaseHeights[i].z) * WaveHeight);
            vertices[i] = vertex;
        }

        //update vertices
        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();

        //update mesh collider
        _meshCollider.sharedMesh = _mesh;
    }
}
