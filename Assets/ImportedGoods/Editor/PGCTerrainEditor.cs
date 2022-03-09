using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PGCTerrain))]
[CanEditMultipleObjects]
public class PGCTerrainEditor : Editor
{
    SerializedProperty SizeOfTerrain;

    //SerializedProperty Width;

    SerializedProperty Depth;

    SerializedProperty Scale;

    SerializedProperty SelectAlgorithm;

    //SerializedProperty AutoUpdateOnAnyChange;

    SerializedProperty randomHeightRange;


    private string currentValue;
    private bool showRandom = false;
    private bool showCustom = false;
    private float _customWidth = 12f;

    void OnEnable()
    {
        SizeOfTerrain = serializedObject.FindProperty("_size");
        //Width = serializedObject.FindProperty("_width");
        Depth = serializedObject.FindProperty("_depth");
        Scale = serializedObject.FindProperty("frequency");
        SelectAlgorithm = serializedObject.FindProperty("SelectAlgorithm");
        //AutoUpdateOnAnyChange = serializedObject.FindProperty("AutoUpdateOnAnyChange");
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PGCTerrain terrain = (PGCTerrain)target;

        showCustom = EditorGUILayout.Foldout(showCustom, "Custom");

        if (showCustom)
        {
            EditorGUILayout.IntSlider(SizeOfTerrain, 0, 5, new GUIContent("Size of Terrain", "Size of terrain"));
            currentValue = CalculateDimensions(SizeOfTerrain.intValue);
            EditorGUILayout.LabelField("Size of terrain HxW: " + currentValue + "x" + currentValue);
            EditorGUILayout.Space(_customWidth);

            EditorGUILayout.Slider(Scale, -100f, 100f, new GUIContent("Frequency", "Frequency of terrain"));
            //EditorGUILayout.LabelField("Frequency of Terrain");
            EditorGUILayout.Space(_customWidth);

            EditorGUILayout.IntSlider(Depth, -100, 100, new GUIContent("Depth", "Depth of terrain"));
            //EditorGUILayout.LabelField("Depth of Terrain");
            EditorGUILayout.Space(_customWidth);

            EditorGUILayout.PropertyField(SelectAlgorithm, new GUIContent("Select Algorithm", "Algorithm to use on terrain"));
            //EditorGUILayout.LabelField("Which algorithm to use for the terrain");
            EditorGUILayout.Space(_customWidth);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Apply Changes"))
            {
                terrain.GenerateTerrainData(false);
            }
        }

        showRandom = EditorGUILayout.Foldout(showRandom, "Random");

        if (showRandom)
        {
            EditorGUILayout.IntSlider(SizeOfTerrain, 0, 5, new GUIContent("Size of Terrain", "Size of terrain"));
            currentValue = CalculateDimensions(SizeOfTerrain.intValue);
            EditorGUILayout.LabelField("Size of terrain HxW: " + currentValue + "x" + currentValue);
            EditorGUILayout.Space(_customWidth);

            EditorGUILayout.PropertyField(randomHeightRange, new GUIContent("Max height range", "Highest points to be made in terrain"));
            //GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Apply Changes"))
            {
                terrain.GenerateTerrainData(true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    string CalculateDimensions(int cur)
    {
        currentValue = cur != 0 ? ((int)Mathf.Pow(2, cur + 4)).ToString() : "0";
        return currentValue;
    }

    

    
}
