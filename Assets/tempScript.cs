using Photon.Pun;
using TMPro;
using UnityEngine;

public class tempScript : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI textBoxInventory;
    
    public static tempScript Instance;

    private int _mineralCount;
    private int _flintCount;
    private int _treeCount;
    private PhotonView _view;
    
    private void Awake()
    {
        Instance = this;
        _view = GetComponent<PhotonView>();
    }

    public void UpdateInventory(string item)
    {
        if (!_view.IsMine) return;
        
        if (item == "Mineral")
        {
            _mineralCount++;
            _view.RPC(nameof(UpdateInventoryMultiplayer), RpcTarget.AllBuffered, _mineralCount, _flintCount, _treeCount);
        }
        else if (item == "Flint")
        {
            _flintCount++;
            _view.RPC(nameof(UpdateInventoryMultiplayer), RpcTarget.AllBuffered, _mineralCount, _flintCount, _treeCount);
        }
        else if (item == "Tree")
        {
            _treeCount++;
            _view.RPC(nameof(UpdateInventoryMultiplayer), RpcTarget.AllBuffered, _mineralCount, _flintCount, _treeCount);
        }
    }

    [PunRPC]
    private void UpdateInventoryMultiplayer(int mineralCount, int flintCount, int treeCount)
    {
        textBoxInventory.text = $"Mineral: {mineralCount}, Flint: {flintCount}, Tree: {treeCount}";
    }

}
