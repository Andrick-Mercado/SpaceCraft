using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IconGenerator : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private List<GameObject> sceneObjects;
    [SerializeField]
    private List<InventoryItemData> dataObjects;

    [Header("Settings")] 
    [SerializeField]
    private string pathFolder;
    
    //inner methods
    private Camera _camera;
    
#if UNITY_EDITOR
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    [ContextMenu("ScreenShot")]
    private void ProcessScreenShots()
    {
        StartCoroutine(ScreenShot());
    }

    private IEnumerator ScreenShot()
    {
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            GameObject obj = sceneObjects[i];
            InventoryItemData data = dataObjects[i];
            
            obj.gameObject.SetActive(true);

            yield return null;
            
            TakeScreenshot($"{Application.dataPath}/{pathFolder}/{data.id}_Icon.png");

            yield return null;
            obj.gameObject.SetActive(false);

            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{pathFolder}/{data.id}_Icon.png");
            if (s != null)
            {
                data.icon = s;
                EditorUtility.SetDirty(data);
            }

            yield return null;

        }
        Debug.Log("Finished!");
    }
    
    private void TakeScreenshot(string fullPath)
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        RenderTexture rt = new RenderTexture(256, 256, 24);
        _camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        _camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0,0,256,256),0,0);
        _camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

    }
#endif
}
