using System;
using UnityEngine;


public class MainMenuCameraController : MonoBehaviour
{
    [Header("Rotation Settings")] 
    [SerializeField]
    private float xRotation;
    [SerializeField]
    private float yRotation;
    [SerializeField]
    private float zRotation;

    [Header("Movement Settings")] 
    [SerializeField]
    private Vector3 startPos;
    [SerializeField] 
    private Vector3 endPos;
    [SerializeField] [Range(0f, 50f)] 
    private float moveSpeed;

    private void Start()
    {
        if(TryGetComponent<RectTransform>(out RectTransform rectTransform))
        {
            startPos = rectTransform.position;
            endPos = rectTransform.position;
        }
    }

    private void FixedUpdate()
    {
        RotateCamera();
        MoveCamera();
    }

    private void RotateCamera()
    {
        transform.Rotate (xRotation*Time.deltaTime,yRotation*Time.deltaTime,zRotation*Time.deltaTime); 
    }

    private void MoveCamera()
    {
        transform.position = Vector3.Lerp (startPos, endPos, (Mathf.Sin(moveSpeed * Time.time) + 1.0f) / 2.0f);
    }
}
