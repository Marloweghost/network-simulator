using TMPro;
using UnityEngine;

public class UITextHandler : MonoBehaviour
{
    [SerializeField] private TextMeshPro infoText;

    public void TextUpdateIP(string IPAddress)
    {
        if (infoText != null)
        {
            infoText.text = IPAddress;
        }
        else return;
    }
}
