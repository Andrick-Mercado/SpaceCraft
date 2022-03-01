using System;
using UnityEngine;
using Photon.Pun;

public class SimpleMovementSystem : MonoBehaviour
{
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float speed = 9.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(!_view.IsMine) return;
        
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontal, 0, vertical) * (speed * Time.deltaTime));
        
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            _moveDirection *= speed;

            if (Input.GetKey("space"))
            {
                _moveDirection.y = jumpSpeed;
            }
        _moveDirection.y -= gravity * Time.deltaTime;
    }
}
