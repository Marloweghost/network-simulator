using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject notificationStartSubPanel;
    [SerializeField] private GameObject notificationSuccessSubPanel;

    [SerializeField] private TMP_Text notificationStartText;
    [SerializeField] private TMP_Text notificationSuccessText;

    public void ActivateNotificationStartPanel(string text)
    {
        notificationStartText.text = text;
        notificationStartSubPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ActivateNotificationSuccessPanel(string text)
    {
        notificationSuccessText.text = text;
        notificationSuccessSubPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void OnButtonStartCloseClicked()
    {
        notificationStartSubPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnButtonToMainMenuClicked() 
    {
        GameObject.Find("SceneManager").GetComponent<InternalSceneManager>().SwitchScene("MainMenu");
    }

    public void OnButtonToNextLevelClicked() 
    {
        
    }
}
