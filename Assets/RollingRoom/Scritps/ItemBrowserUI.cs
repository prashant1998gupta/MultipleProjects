using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public class ItemBrowserUI : MonoBehaviour
{
    public Transform categoryPanel;
    public Transform itemContentPanel;
    public GameObject categoryButtonPrefab;
    public GameObject itemButtonPrefab;

    public List<ItemData> allItems;

    public ItemSpawner spawner;
    private ItemIconAssigner itemIconAssigner;

    private ItemCategory currentCategory;

    void Start()
    {
        itemIconAssigner = GetComponent<ItemIconAssigner>();
        itemIconAssigner.GenerateAndAssignIcons(allItems[(int)ItemCategory.Room]);
        LoadCategories();
        FilterByCategory(ItemCategory.Room);
    }

    void LoadCategories()
    {
        foreach (ItemCategory category in System.Enum.GetValues(typeof(ItemCategory)))
        {
            GameObject btn = Instantiate(categoryButtonPrefab, categoryPanel);
            btn.name = category.ToString() + "Button";
            btn.GetComponentInChildren<TextMeshProUGUI>().text = category.ToString();
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                itemIconAssigner.GenerateAndAssignIcons(allItems[(int)category]);
                FilterByCategory(category);
            });
        }
    }

    void FilterByCategory(ItemCategory category)
    {
        currentCategory = category;

        foreach (Transform child in itemContentPanel)
            Destroy(child.gameObject);

        var filtered = allItems.Where(i => i.category == category);

        Debug.Log($"this is category {filtered}");

        foreach (var item in filtered)
        {
            for (int i = 0; i < item.prefabVariants.Length; i++)
            {
                GameObject btn = Instantiate(itemButtonPrefab, itemContentPanel);
                btn.name = $"{item.prefabVariants[i].name}";
                // Label it clearly (e.g., Bathroom 01)
                TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = $"{item.prefabVariants[i].name} {i + 1}";

                Image img = btn.GetComponentInChildren<Image>();
                if (img != null)
                {
                    if (item.variantIcons != null && i < item.variantIcons.Length)
                        img.sprite = item.variantIcons[i];
                    else
                        img.sprite = item.icon; // fallback
                }

                int variantIndex = i; // capture for closure
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    spawner.SetSelectedItemVariant(item, variantIndex);
                });
            }
        }
    }
}
