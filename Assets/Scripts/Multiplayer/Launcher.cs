using System.Collections.Generic;
using System.Linq;
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

    /** CONNECTING TO LOBBY = SERVER **/
    private void Start()
    {
        if(!PhotonNetwork.IsConnected) //for when player leaves a room
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
    }

    /** Creating/Joining ROOMS  **/
    
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createdRoomName.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(createdRoomName.text);
        MenuManager.Instance.OpenMenu("loadingMenu");
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed with message: " + message;
        MenuManager.Instance.OpenMenu("errorMenu");
    }
    
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loadingMenu");
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }

        // foreach (var t in roomList.Where(t => !t.RemovedFromList))
        // {
        //     Instantiate(roomListItemPrefab,
        //         roomListContent).GetComponent<RoomListItem>().SetUp(t);
        // }

        foreach (var t in roomList.Where(t => !t.RemovedFromList || !t.IsOpen))
        {
            Instantiate(roomListItemPrefab,
                roomListContent).GetComponent<RoomListItem>().SetUp(t);
        }
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.CloseAllOpenMenus();
        PhotonNetwork.LoadLevel(1); //name or number of scene here
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room join failed with message: " + message;
        MenuManager.Instance.OpenMenu("errorMenu");
    }

    /** other utilities **/
    
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
