using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    
    [SerializeField] private TMP_InputField createdRoomName;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("mainMenu");
        //Debug.Log("Connected To Server");
    }

    /**  Methods below are for after connecting to server through UI  **/
    
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createdRoomName.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(createdRoomName.text);
        MenuManager.Instance.OpenMenu("loadingMenu");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.CloseAllOpenMenus();
        PhotonNetwork.LoadLevel(1); //name or number of scene here
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed with message: " + message;
        MenuManager.Instance.OpenMenu("errorMenu");
    }

    public void LeaveRoom()
    {
        //MenuManager.Instance.OpenMenu("loadingMenu");
        //MenuManager.Instance.CloseMenu("pauseMenu");
        
        //Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //MenuManager.Instance.OpenMenu("mainMenu");
        PhotonNetwork.LoadLevel(0);
        //MenuManager.Instance.OpenMenu("mainMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }
        
        foreach (var t in roomList)
        {
            Instantiate(roomListItemPrefab,
                roomListContent).GetComponent<RoomListItem>().SetUp(t);
        }
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loadingMenu");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
