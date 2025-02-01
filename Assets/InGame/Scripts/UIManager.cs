using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject playUpperPanel;
    [SerializeField] private GameObject playLowerPanel;
    [SerializeField] private GameObject playAttackLowerPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject hiringPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject WinningPanel;
    [SerializeField] private GameObject PlayerOneWinnerimg;
    [SerializeField] private GameObject PlayerTwoWinnerimg;



    public GameObject DraftPanel; // Main draft panel
    public GameObject SelectedCardPanel; // Panel for selected card

    public GameObject DisplayAllCardPanel;
    public Button OkButton;
    [SerializeField] private float fadeDuration = 0.5f; // Duration for fade animations
    [SerializeField] LoadingManager loadingManager;


    private void Awake()
    {
        loadingManager.gameObject.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    public void OpenPanel(GameObject panel)
    {
        if (panel != null)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                panel.SetActive(true); // Enable the GameObject first
                canvasGroup.alpha = 0; // Ensure alpha starts from 0 for fade-in
                DoTweenHelper.FadePanel(canvasGroup, true, fadeDuration);
            }
            else
            {
                panel.SetActive(true); // Fallback if CanvasGroup is not attached
            }
        }
        else
        {
            Debug.LogWarning("The panel you tried to open is not assigned. ");
        }
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel != null)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                DoTweenHelper.FadePanel(canvasGroup, false, fadeDuration, () =>
                {
                    panel.SetActive(false); // Disable the GameObject after fade-out
                });
            }
            else
            {
                panel.SetActive(false); // Fallback if CanvasGroup is not attached
            }
        }
        else
        {
            Debug.LogWarning("The panel you tried to close is not assigned.");
        }
    }

    // Helper methods for each specific panel
    public void OpenPlayUpperPanel() => OpenPanel(playUpperPanel);
    public void ClosePlayUpperPanel() => ClosePanel(playUpperPanel);

    public void OpenPlayLowerPanel()
    {
        if (!playLowerPanel.activeSelf)
        {
            SoundManager.Instance?.PlayPanelChanging();
            OpenPanel(playLowerPanel);
        }
    }
    public void ClosePlayLowerPanel()
    {

        if (playLowerPanel.activeSelf)
        {
            ClosePanel(playLowerPanel);

        }


    }
    public void playTapSound()
    {

        SoundManager.Instance?.PlayButtonTap();
    }
    public void OpenPlayAttackLowerPanel()
    {

        SoundManager.Instance?.PlayPanelChanging();
        OpenPanel(playAttackLowerPanel);
    }
    public void ClosePlayAttackLowerPanel() => ClosePanel(playAttackLowerPanel);

    public void OpenPausePanel() { SoundManager.Instance?.PlayButtonTap(); OpenPanel(pausePanel); }
    public void ClosePausePanel() { SoundManager.Instance?.PlayButtonTap(); ClosePanel(pausePanel); }

    public void OpenHiringPanel() { SoundManager.Instance?.PlayButtonTap(); OpenPanel(hiringPanel); }
    public void CloseHiringPanel() { SoundManager.Instance?.PlayButtonTap(); ClosePanel(hiringPanel); }

    public void OpenInfoPanel() { SoundManager.Instance?.PlayButtonTap(); OpenPanel(infoPanel); }
    public void CloseInfoPanel() { SoundManager.Instance?.PlayButtonTap(); ClosePanel(infoPanel); }
    public void OpenDraftPanel() { DraftPanel.SetActive(true); }
    public void CloseDraftPanel() { DraftPanel.SetActive(false); }
    public void OpenSelectedCardPanel() { SelectedCardPanel.SetActive(true); }
    public void CloseSelectedCardPanel() { SelectedCardPanel.SetActive(false); }
    public void OpenDisplayAllCardPanel() { OpenPanel(DisplayAllCardPanel); }
    public void CloseDisplayAllCardPanel() { ClosePanel(DisplayAllCardPanel); }
    public void EnablePlayLoyOut()
    {
        // SoundManager.Instance?.PlayPanelChanging();
        OpenPlayUpperPanel();
        OpenPlayLowerPanel();
        ClosePlayAttackLowerPanel();
        CloseHiringPanel();
        CloseInfoPanel();
    }
    public void OnWinningPanel(Int32 Playerno)
    {

        SoundManager.Instance?.PlayEndScreenOpen();
        WinningPanel.SetActive(true);
        GameManager.Instance.draftManager.SetSelectedIcons();
        Debug.LogWarning("Winning panelOpen winner is " + Playerno);
        PlayerOneWinnerimg.SetActive(Playerno == 1 ? true : false);
        PlayerTwoWinnerimg.SetActive(Playerno == 2 ? true : false);

    }
    public void DisablePlayLoyOut()
    {
        // SoundManager.Instance?.PlayEndScreenOpen();
        ClosePlayUpperPanel();
        ClosePlayLowerPanel();


    }
    public void MainMenu()
    {

        SoundManager.Instance?.PlayButtonTap();
        loadingManager.gameObject.SetActive(true);
    }
    // Method to exit the application
    public void QuitApplication()
    {

        SoundManager.Instance?.PlayButtonTap();
        loadingManager.gameObject.SetActive(true);
    }

}
