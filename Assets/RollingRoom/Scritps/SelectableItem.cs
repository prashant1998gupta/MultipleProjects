using UnityEngine;

public class SelectableItem : MonoBehaviour
{
    private static SelectableItem currentSelected;

    public static SelectableItem Current => currentSelected;

    private bool isBeingMoved = false;

    public void Select()
    {
        if (currentSelected != null && currentSelected != this)
            currentSelected.Deselect();

        currentSelected = this;
        Highlight(true);
    }

    public void Deselect()
    {
        Highlight(false);
        if (currentSelected == this)
            currentSelected = null;
    }

    void Highlight(bool isOn)
    {
        // Optional: Add outline, scale up slightly, or change material
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = isOn ? Color.yellow : Color.white;
        }
    }

    void OnMouseDown()
    {
        Select();
        isBeingMoved = true; // Start move on click
    }

    public bool IsBeingMoved => isBeingMoved;
    public void ConfirmPlacement() => isBeingMoved = false;

}
