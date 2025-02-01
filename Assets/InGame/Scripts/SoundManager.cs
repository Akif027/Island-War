using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource sfxSource; // AudioSource for sound effects
    public AudioSource musicSource; // AudioSource for background music

    [Header("Sound Effect Clips")]
    public AudioClip BgPushIsland;
    public AudioClip CoinToss;
    public AudioClip CoinCollect;
    public AudioClip Score;
    public AudioClip CharacterChoose;
    public AudioClip Collision;
    public AudioClip DiscardFirstCard;
    public AudioClip TurnChange;
    public AudioClip TokenLeaving;
    public AudioClip DownPanelChanging;
    public AudioClip EndScreenOpen;
    public AudioClip ButtonTap;
    public AudioClip NotPossiblePlacement;
    public AudioClip WhenYouTryToTapOpponentChip;
    public AudioClip HiringChip;
    public AudioClip FlickTheChip;
    public AudioClip CoinIsStopped;

    private bool isMusicEnabled = true; // Tracks if music is enabled
    private bool isSfxEnabled = true;   // Tracks if SFX is enabled

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Music Controls
    public void EnableMusic()
    {
        isMusicEnabled = true;
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play(); // Resume music playback
        }
        Debug.Log("Music enabled.");
    }

    public void DisableMusic()
    {
        isMusicEnabled = false;
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause(); // Pause music playback
        }
        Debug.Log("Music disabled.");
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null || !isMusicEnabled) return;

        if (musicSource != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    #endregion

    #region SFX Controls
    public void EnableSfx()
    {
        isSfxEnabled = true;
        Debug.Log("SFX enabled.");
    }

    public void DisableSfx()
    {
        isSfxEnabled = false;
        Debug.Log("SFX disabled.");
    }

    public void PlaySFX(AudioClip clip)
    {
        if (isSfxEnabled && clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonTap()
    {
        PlaySFX(ButtonTap);
    }

    // Individual SFX methods
    public void PlayCoinToss() { PlaySFX(CoinToss); }
    public void PlayCoinCollect() { PlaySFX(CoinCollect); }
    public void PlayScore() { PlaySFX(Score); }
    public void PlayCharacterChoose() { PlaySFX(CharacterChoose); }
    public void PlayCollision() { PlaySFX(Collision); }
    public void PlayDiscardFirstCard() { PlaySFX(DiscardFirstCard); }
    public void PlayTurnChange() { PlaySFX(TurnChange); }
    public void PlayTokenLeaving() { PlaySFX(TokenLeaving); }
    public void PlayPanelChanging() { PlaySFX(DownPanelChanging); }
    public void PlayEndScreenOpen() { PlaySFX(EndScreenOpen); }
    public void PlayNotPossiblePlacement() { PlaySFX(NotPossiblePlacement); }
    public void PlayWhenTapOnOpponentChip() { PlaySFX(WhenYouTryToTapOpponentChip); }
    public void PlayDuringHiringChipPlaced() { PlaySFX(HiringChip); }
    public void PlayFlickTheChip() { PlaySFX(FlickTheChip); }
    public void PlayCoinTossStopped() { PlaySFX(CoinIsStopped); }
    #endregion
}
