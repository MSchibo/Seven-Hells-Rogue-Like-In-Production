using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Music Clips")]
    public AudioClip mainMenuTheme;
    public AudioClip tutorialTheme;

    void Start()
    {
        // Szene beim Start überprüfen und Musik abspielen, wenn keine läuft
        string currentScene = SceneManager.GetActiveScene().name;

        if (AudioManager.Instance == null) return;

        if (!AudioManager.Instance.IsMusicPlaying())
        {
            if (currentScene == "MainMenu")
            {
                AudioManager.Instance.PlayMusic(mainMenuTheme, true);
            }
            else if (currentScene == "Start Tutorial")
            {
                // Wird durch HauptcharakterMovement verzögert abgespielt – NICHT hier starten
            }
            // weitere Szenen und Themes hier hinzufügen…
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}


