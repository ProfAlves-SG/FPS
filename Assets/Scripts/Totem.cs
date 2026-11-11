using UnityEngine;

public class Totem : MonoBehaviour
{
    [SerializeField] private string itemTag = "Item";
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemSpawnPoint;
    
    public void Deposit(PlayerInteractor player)
    {
        if (itemTag == "Item" && player.hasItem)
        {
            player.hasItem = false;
            player.itemIcon.SetActive(false);
            
            GameObject item = Instantiate(itemPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
            item.tag = "Item";
        }
    }
}
