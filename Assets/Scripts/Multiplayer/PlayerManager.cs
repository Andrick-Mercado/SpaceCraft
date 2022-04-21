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
    private void Start()
    {
        if(_view.IsMine)
            CreateController();
    }

    //spawns in the player prefab at a given location
    private static void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("Player/Player"),
            new Vector3(36.7000008f, 0.699999988f, 0f), Quaternion.identity);
    }
}
