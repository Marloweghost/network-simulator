using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private InternalSceneManager sceneManager;
    [SerializeField] private PracticeSubUIManager practiceSubUIManager;

    [Header("Panel References")]
    [SerializeField] private GameObject MainMenuSubPanel;
    [SerializeField] private GameObject LevelsSubPanel;
    [SerializeField] private GameObject PracticeSubPanel;

    private List<GameObject> panels = new List<GameObject>();

    private void AddPanel(GameObject panel)
    {
        panels.Add(panel);
    }

    public void ChangePanel(GameObject panelToOpen)
    {
        foreach (GameObject panel in panels)
        {
            if (panel == panelToOpen)
            {
                panel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        ChangePanel(MainMenuSubPanel);
    }

    private void Awake()
    {
        AddPanel(MainMenuSubPanel);
        AddPanel(LevelsSubPanel);
        AddPanel(PracticeSubPanel);
    }

    public void OnUIButtonLevelsClicked()
    {
        ChangePanel(LevelsSubPanel);
    }

    public void OnUIButtonPracticeClicked()
    {
        ChangePanel(PracticeSubPanel);
    }

    public void OnUIButtonCloseSubMenuClicked()
    {
        ChangePanel(MainMenuSubPanel);
    }

    public void OnUIButtonLevelClicked(string levelName)
    {
        sceneManager.SwitchScene(levelName);
    }

    public void OnUIButtonGenerateClicked()
    {
        float complexity;
        int difficulty;

        practiceSubUIManager.GetSliderValues(out complexity, out difficulty);

        sceneManager.SwitchScene("Level_Generated", complexity, difficulty);
    }
}
