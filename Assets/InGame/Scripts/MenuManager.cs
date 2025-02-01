using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button PlayB;
    [SerializeField] Button PlayBRus;
    [SerializeField] Button Music;
    [SerializeField] Sprite Musicoff;
    [SerializeField] Sprite MusicOn;
    [SerializeField] Button Sfx;
    [SerializeField] Sprite SfxOn;
    [SerializeField] Sprite SfxOff;
    [SerializeField] GameObject RulesPanel;
    [SerializeField] LanguageSelector languageSelector;
    private bool isMusicOn = true; // Tracks the music state
    private bool isSfxOn = true;   // Tracks the SFX state

    void Start()
    {
        PlayB.onClick.AddListener(() => LoadTheScene("GamePlay"));
        PlayBRus.onClick.AddListener(() => LoadTheScene("GameplayRus"));
        Music.onClick.AddListener(SoundButtonHandler);
        Sfx.onClick.AddListener(MusicButtonHandler);
    }

    void SoundButtonHandler()
    {
        // Toggle music state
        isMusicOn = !isMusicOn;

        // Update button sprite
        Music.GetComponent<Image>().sprite = isMusicOn ? MusicOn : Musicoff;

        // Enable/disable music using SoundManager
        if (isMusicOn)
        {
            SoundManager.Instance?.EnableMusic();
            Debug.Log("Music enabled.");
        }
        else
        {
            SoundManager.Instance?.DisableMusic();
            Debug.Log("Music disabled.");
        }

        // Play button tap sound if applicable
        SoundManager.Instance?.PlayButtonTap();
    }

    void MusicButtonHandler()
    {
        // Toggle SFX state
        isSfxOn = !isSfxOn;

        // Update button sprite
        Sfx.GetComponent<Image>().sprite = isSfxOn ? SfxOn : SfxOff;

        // Enable/disable SFX using SoundManager
        if (isSfxOn)
        {
            SoundManager.Instance?.EnableSfx();
            Debug.Log("SFX enabled.");
        }
        else
        {
            SoundManager.Instance?.DisableSfx();
            Debug.Log("SFX disabled.");
        }

        // Play button tap sound if applicable
        SoundManager.Instance?.PlayButtonTap();
    }

    void LoadTheScene(string S)
    {
        CustomSceneManager.LoadSceneAsync(S);
        SoundManager.Instance?.PlayButtonTap();
    }

    void RulesInfo()
    {
        SoundManager.Instance?.PlayButtonTap();
        RulesPanel.SetActive(!RulesPanel.activeSelf);
    }

    void OnDisable()
    {
        PlayB.onClick.RemoveListener(() => LoadTheScene("GamePlay"));
        Music.onClick.RemoveListener(SoundButtonHandler);
        Sfx.onClick.RemoveListener(MusicButtonHandler);
    }
}
