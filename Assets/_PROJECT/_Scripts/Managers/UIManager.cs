using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private VerticalLayoutGroup landmarkUI;
    [SerializeField] private Button[] landmarkButtons;
    [SerializeField] private Button btnStart, btnNextLevel;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_InputField nameField, difficultyField;

    private void Start()
    {
        btnNextLevel.gameObject.SetActive(false);
        landmarkUI.gameObject.SetActive(false);
        btnStart.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(false);
        DeactivateLandmarkButtons();
    }

    public int ReadDifficulty()
    {
        if (int.TryParse(difficultyField.text, out int difficulty))
        {
            return difficulty;
        }

        Debug.LogError("Invalid difficulty input. Defaulting to 5.");
        return 5; // Default value if parsing fails
    }


    public string ReadName()
    {
        return nameField.text;
    }

    private void OnEnable()
    {
        EventManager.OnLevelEnd += OnLevelEnd;
        EventManager.OnLevelStart += OnLevelStart;
        EventManager.OnSecondPhaseStarted += OnSecondPhaseStart;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart -= OnLevelStart;
        EventManager.OnLevelEnd -= OnLevelEnd;
        EventManager.OnSecondPhaseStarted -= OnSecondPhaseStart;
    }

    private void OnDestroy()
    {
        EventManager.ClearAllSubscriptions();
    }

    private void SetButtons()
    {
        DOVirtual.DelayedCall(5f, (() =>
        {
            var landmarks = FindObjectsOfType<Landmark>().ToList();
            landmarks.Reverse();

            for (int i = 0; i < landmarks.Count; i++)
            {
                landmarks[i].LandmarkButton = landmarkButtons[i];
            }
        }));
    }

    public void TriggerLoadingBar(float loadingTime, Action<bool> onComplete)
    {
        loadingBar.gameObject.SetActive(true);
        loadingBar.value = 0f; // Reset the bar to start from 0

        loadingBar.DOValue(1f, loadingTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                onComplete?.Invoke(true);
                landmarkUI.GetComponent<Image>().enabled = true;
                loadingBar.gameObject.SetActive(false);
            }); 
    }

    private void OnLevelStart()
    {
        landmarkUI.gameObject.SetActive(true);
        landmarkUI.GetComponent<Image>().enabled = false;
        mainMenu.gameObject.SetActive(false);
        btnStart.gameObject.SetActive(false);
        SetButtons();
        DeactivateLandmarkButtons();
    }


    private void OnSecondPhaseStart()
    {
        mainMenu.gameObject.SetActive(false);
        btnNextLevel.gameObject.SetActive(false);
        foreach (var button in landmarkButtons)
        {
            button.enabled = true;
        }
    }

    private void OnLevelEnd()
    {
        mainMenu.gameObject.SetActive(false);
        btnStart.gameObject.SetActive(false);
        btnNextLevel.gameObject.SetActive(true);
    }


    private void DeactivateLandmarkButtons()
    {
        foreach (var button in landmarkButtons)
        {
            button.enabled = false;
            button.GetComponent<Image>().enabled = false;
        }
    }
}