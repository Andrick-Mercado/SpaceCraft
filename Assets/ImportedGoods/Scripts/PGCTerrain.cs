using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class PGCTerrain : MonoBehaviour
{
    [SerializeField]
    private int _size = 1;

    [SerializeField]
    private int _width = 1;

    [SerializeField]
    private int _depth = 20;

    [SerializeField]
    private float frequency = 20f;


    [SerializeField]
    private MyEnum SelectAlgorithm = new MyEnum();
    public enum MyEnum
    {
        Sin,
        Cos,
        Perlin
    };

    //[SerializeField]
    //private bool AutoUpdateOnAnyChange = false;

    private Terrain _currentTerrain;
    private TerrainData _currentTerrainData;

    //private bool _hasChanged;
    //private string _previous;

    private int heightCopy;

    public int randomHeightRange = 0;
    
    void OnEnable()
    {
        _currentTerrain = GetComponent<Terrain>();
        _currentTerrainData = Terrain.activeTerrain.terrainData;
        //_currentTerrain.terrainData = GenerateTerrainData(_currentTerrain.terrainData);

    }

    //void Update()
    //{
        

    //    if (!_hasChanged || AutoUpdateOnAnyChange)
    //    {
    //        updateValues();
    //        _currentTerrain.terrainData = GenerateTerrainData(_currentTerrain.terrainData);
    //    }
    //    else if (!SelectAlgorithm.ToString().Equals(_previous))
    //    {
    //        _hasChanged = false;
    //    }
        
    //}

    public void GenerateTerrainData(bool isRandom)
    {
        if(isRandom)
        {
            updateValues();
            _currentTerrainData.heightmapResolution = _width + 1;
            _currentTerrainData.size = new Vector3(_width, _depth, heightCopy);
            _currentTerrainData.SetHeights(0, 0, GenerateRandomHeights());
        }
        else
        {
            updateValues();
            _currentTerrainData.heightmapResolution = _width + 1;
            _currentTerrainData.size = new Vector3(_width, _depth, heightCopy);
            _currentTerrainData.SetHeights(0, 0, GenerateHeights());
        }
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[_width, heightCopy];

        for(int x = 0; x< _width; x++)
        {
            for (int y = 0; y < heightCopy; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight (int x, int y)
    {
        float xCoord = (float) x / _width * frequency;
        float yCoord = (float) y / heightCopy * frequency;

        string currentString = SelectAlgorithm.ToString();

        if (currentString.Equals("Sin"))
        {
            return Mathf.Sin(xCoord) + Mathf.Sin(yCoord);
        }
        else if (currentString.Equals("Cos"))
        {
            return Mathf.Cos(xCoord) * Mathf.Cos(yCoord);
        }
        else //if (currentString.Equals("Perlin") )
        {
            return Mathf.PerlinNoise(xCoord, yCoord);
        }
         
    }

    float[,] GenerateRandomHeights()
    {
        float[,] heights = new float[_width, heightCopy];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < heightCopy; y++)
            {
                heights[x, y] = CalculateRandomHeight(x,y);
            }
        }
        return heights;
    }

    float CalculateRandomHeight(int x, int y)
    {
        float xCoord = (float)UnityEngine.Random.Range(0f, randomHeightRange) / _width * x;
        float yCoord = (float)UnityEngine.Random.Range(0f, randomHeightRange) / heightCopy * y;

        return Mathf.Sin(xCoord) * Mathf.Sin(yCoord);
    }

    void updateValues()
    {
        if (_size == 0)
        {
            heightCopy = 0;
            _width = heightCopy;
            return;
        }
        heightCopy = (int) Mathf.Pow(2f, _size + 4);//  (_height * 32);
        _width = heightCopy;
    }
}
