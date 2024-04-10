using TMPro;
using UnityEngine;

public class UITextHandler : MonoBehaviour
{
    [SerializeField] private TextMeshPro infoText;
    [SerializeField] private TextMeshPro nameText;

    public void TextUpdateIP(string IPAddress, string nodeName)
    {
        if (infoText != null && nameText != null)
        {
            infoText.text = IPAddress;
            nameText.text = nodeName;
        }
        else return;
    }
}
