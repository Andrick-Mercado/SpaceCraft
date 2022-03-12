using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _view;

    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(_view.IsMine)
            CreateController();
    }

    private void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("Player"),
            new Vector3(36.7000008f, 0.699999988f, 0f), Quaternion.identity);
    }
}
