using UnityEngine;
using UnityEngine.UI;

public class ItemListLoader : MonoBehaviour
{
    public Transform contentParent; // ScrollView > Content
    public GameObject buttonPrefab; // A simple button prefab
    public ItemData[] allItems;

    void Start()
    {
        foreach (ItemData item in allItems)
        {
            GameObject btn = Instantiate(buttonPrefab, contentParent);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            btn.GetComponent<Button>().onClick.AddListener(() => SelectItem(item));
        }
    }

    void SelectItem(ItemData item)
    {
        FindFirstObjectByType<ItemSpawner>().SetSelectedItem(item);
    }
}
