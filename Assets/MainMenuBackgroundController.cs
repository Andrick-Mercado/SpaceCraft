using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackgroundController : MonoBehaviour
{
    [Header("Dependencies")] 
    [SerializeField]
    private RectTransform rectTransform;
    
    [Header("Movement Settings")] 
    [SerializeField]
    private Vector3 startPos;
    [SerializeField] 
    private Vector3 endPos;
    [SerializeField] [Range(0f, 50f)] 
    private float moveSpeed;
    
    private void Update()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        Vector3 recPos = rectTransform.position;
        transform.position = Vector3.Lerp (recPos + startPos, recPos + endPos, (Mathf.Sin(moveSpeed * Time.time) + 1.0f) / 2.0f);
    }
}
