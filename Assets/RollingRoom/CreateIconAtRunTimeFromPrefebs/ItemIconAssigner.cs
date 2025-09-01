using UnityEngine;

public class ItemIconAssigner : MonoBehaviour
{
    public IconGenerator iconGenerator;
    public ItemData itemData;

    private void Start()
    {
        //GenerateAndAssignIcons(itemData);
    }

    [ContextMenu("Generate & Assign Icons")]
    public void GenerateAndAssignIcons(ItemData itemData)
    {
        int len = itemData.prefabVariants.Length;
        itemData.variantIcons = new Sprite[len];

        for (int i = 0; i < len; i++)
        {
            var prefab = itemData.prefabVariants[i];
            if (prefab != null)
            {
                Sprite icon = iconGenerator.GenerateIcon(prefab);
                itemData.variantIcons[i] = icon;
            }
        }

        Debug.Log($"Assigned {len} variant icons to '{itemData.name}'");
    }
}
