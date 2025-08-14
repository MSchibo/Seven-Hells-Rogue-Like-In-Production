using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip mainTheme;  // MainTheme Clip hier hinzufügen

    private bool sfxEnabled = true;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager initialisiert mit MusicSource: " + (musicSource != null ? musicSource.name : "null") + " am " + System.DateTime.Now);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource != null && mainTheme != null && !musicSource.isPlaying)
        {
            musicSource.clip = mainTheme;
            musicSource.loop = true;
            musicSource.Play();
            Debug.Log("MainTheme gestartet am " + System.DateTime.Now);
        }
        else
        {
            Debug.LogWarning("Kein AudioClip zugewiesen, MusicSource ist null oder Musik läuft bereits! Zeit: " + System.DateTime.Now);
        }
    }

    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("MusicSource ist null! Kann Musik nicht abspielen. Zeit: " + System.DateTime.Now);
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
        Debug.Log("Neue Musik abgespielt: " + (clip != null ? clip.name : "null") + " am " + System.DateTime.Now);
    }

    public AudioClip GetClipByName(string clipName)
    {
        switch (clipName)
        {
            case "MainTheme":
                return mainTheme;
            case "ButtonClick":
                return buttonClickSound;
            default:
                Debug.LogWarning("Kein AudioClip mit Namen " + clipName + " gefunden.");
                return null;
        }
    }

    public void ToggleMusic(bool isOn)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("MusicSource ist null! Kann Musik nicht togglen. Zeit: " + System.DateTime.Now);
            return;
        }

        if (isOn && !musicSource.isPlaying)
        {
            musicSource.Play();
            Debug.Log("Musik gestartet am " + System.DateTime.Now);
        }
        else if (!isOn && musicSource.isPlaying)
        {
            musicSource.Pause();
            Debug.Log("Musik pausiert am " + System.DateTime.Now);
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("Musik gestoppt am " + System.DateTime.Now);
        }
    }

    public bool IsSFXEnabled()
    {
        return sfxEnabled;
    }

    public void ToggleSFX(bool isOn)
    {
        sfxEnabled = isOn;
        if (sfxSource != null)
        {
            sfxSource.mute = !isOn;
            Debug.Log("SFX " + (isOn ? "aktiviert" : "deaktiviert") + " am " + System.DateTime.Now);
        }
        else
        {
            Debug.LogWarning("SFXSource ist null! Kann SFX nicht togglen. Zeit: " + System.DateTime.Now);
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (sfxEnabled && sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
            Debug.Log("Soundeffekt abgespielt: " + clip.name + " am " + System.DateTime.Now);
        }
    }

    public void PlayButtonClick()
    {
        PlaySoundEffect(buttonClickSound);
    }
}


