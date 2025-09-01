using Unity.VisualScripting;
using UnityEngine;

public enum ItemCategory
{
    Room,
    Kitchen,
    Bathroom,
    Props,
    Curtains,
    Food,
    KitchenProps,
   /* Appliances,
    Cabinets,
    Furnitures,
    Lightings,
    DoorsAndWindows,
    Patio,
    Plumbing*/
}

[CreateAssetMenu(fileName = "Item", menuName = "RoomDesigner/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public Sprite[] variantIcons;
    public GameObject[] prefabVariants;
    public ItemCategory category;
}
