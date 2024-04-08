using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PracticeSubUIManager : MonoBehaviour
{
    [SerializeField] private GameObject complexityTextCarrier;
    [SerializeField] private GameObject difficultyTextCarrier;

    [SerializeField] private GameObject complexitySliderCarrier;
    [SerializeField] private GameObject difficultySliderCarrier;

    private TMP_Text complexityText;
    private TMP_Text difficultyText;

    private UnityEngine.UI.Slider complexitySlider;
    private UnityEngine.UI.Slider difficultySlider;

    private void Start()
    {
        complexityText = complexityTextCarrier.GetComponent<TMP_Text>();
        difficultyText = difficultyTextCarrier.GetComponent<TMP_Text>();
        complexitySlider = complexitySliderCarrier.GetComponent<UnityEngine.UI.Slider>();
        difficultySlider = difficultySliderCarrier.GetComponent<UnityEngine.UI.Slider>();

        complexityText.text = $"Complexity: {Mathf.Round(complexitySlider.value * 100f) / 100f}";
        difficultyText.text = $"Сложность: {difficultySlider.value}";
    }

    public void OnComplexitySliderValueChanged()
    {
        complexityText.text = $"Complexity: {Mathf.Round(complexitySlider.value * 100f) / 100f}";
    }

    public void OnDiffucultySliderValueChanged()
    {
        difficultyText.text = $"Сложность: {difficultySlider.value}";
    }

    public void GetSliderValues(out float complexity, out int difficulty)
    {
        complexity = complexitySlider.value;
        difficulty = (int)difficultySlider.value;
    }
}
