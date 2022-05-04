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
    private int _birdCount;
    
    private void Awake()
    {
        Instance = this;
    }

    public void UpdateInventory(string item)
    {
        if (item == "Mineral")
        {
            _mineralCount++;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
        else if (item == "Flint")
        {
            _flintCount++;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
        else if (item == "Tree")
        {
            _treeCount++;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
        else if (item == "Birds")
        {
            _birdCount++;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
    }

    public void removeItemAmount(string item, int amount)
    {
        if (item == "Mineral")
        {
            _mineralCount -= amount;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
        else if (item == "Flint")
        {
            _flintCount -= amount;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
        else if (item == "Tree")
        {
            _treeCount -= amount;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
        else if (item == "Birds")
        {
            _birdCount -= amount;
            UpdateInventoryMultiplayer( _mineralCount, _flintCount, _treeCount,_birdCount);
        }
    }
    
    private void UpdateInventoryMultiplayer(int mineralCount, int flintCount, int treeCount, int birdCount)
    {
        textBoxInventory.text = $"Mineral: {mineralCount}, Flint: {flintCount}, Tree: {treeCount}, Bird: {birdCount}";
    }

}
