using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    public Button englishButton;          // Button for English
    public Button russianButton;          // Button for Russian

    public Sprite englishSelectedIcon;    // Selected icon for English
    public Sprite englishDeselectedIcon;  // Deselected icon for English
    public Sprite russianSelectedIcon;    // Selected icon for Russian
    public Sprite russianDeselectedIcon;  // Deselected icon for Russian

    public GameObject mainButton1;        // Main button 1 (active for Russian)
    public GameObject mainButton2;        // Main button 2 (active for English)

    private bool isEnglishSelected = true;

    private const string LanguageKey = "SelectedLanguage"; // Key for saving language in PlayerPrefs

    private void Start()
    {
        // Add listeners to the buttons
        englishButton.onClick.AddListener(() => ChangeLanguage("English"));
        russianButton.onClick.AddListener(() => ChangeLanguage("Russian"));

        LoadLang();
    }

    public void ChangeLanguage(string language)
    {
        SoundManager.Instance?.PlayButtonTap();

        if (language == "English")
        {
            isEnglishSelected = true;

            // Update button icons
            englishButton.GetComponent<Image>().sprite = englishSelectedIcon;
            russianButton.GetComponent<Image>().sprite = russianDeselectedIcon;

            // Show mainButton2 and hide mainButton1
            mainButton2.SetActive(true);
            mainButton1.SetActive(false);
        }
        else if (language == "Russian")
        {
            isEnglishSelected = false;

            // Update button icons
            englishButton.GetComponent<Image>().sprite = englishDeselectedIcon;
            russianButton.GetComponent<Image>().sprite = russianSelectedIcon;

            // Show mainButton1 and hide mainButton2
            mainButton1.SetActive(true);
            mainButton2.SetActive(false);
        }

        // Save the selected language in PlayerPrefs
        PlayerPrefs.SetString(LanguageKey, language);
        PlayerPrefs.Save();
    }

    public void LoadLang()
    {
        // Load the saved language or initialize with English as the default
        string savedLanguage = PlayerPrefs.GetString(LanguageKey, "English");
        ChangeLanguage(savedLanguage);
    }
}
