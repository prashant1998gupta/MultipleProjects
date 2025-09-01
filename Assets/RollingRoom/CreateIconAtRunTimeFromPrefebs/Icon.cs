using UnityEngine.UI;
using UnityEngine;

public class Icon : MonoBehaviour
{
    [SerializeField] private IconGenerator iconGenerator;
    [SerializeField] private Image myUIIconImage;
    [SerializeField] private GameObject myPrefab;
    
    

    void Start()
    {
        var icon = iconGenerator.GenerateIcon(myPrefab);
        myUIIconImage.sprite = icon;
    }
}
