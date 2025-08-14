using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Music Clips")]
    public AudioClip mainMenuTheme;
    public AudioClip tutorialTheme;

    void Start()
    {
        // Szene beim Start �berpr�fen und Musik abspielen, wenn keine l�uft
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
                // Wird durch HauptcharakterMovement verz�gert abgespielt � NICHT hier starten
            }
            // weitere Szenen und Themes hier hinzuf�gen�
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}


