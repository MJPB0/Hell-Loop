using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuScript : MonoBehaviour
{
    public GameObject optionsScreen;
    public GameObject menuScreen;
    public void PlayGame()
    {
        SoundManager.PlayButtonClickSound();
    }

    public void GoOptions()
    {
        SoundManager.PlayButtonClickSound();
        optionsScreen.SetActive(true);
        menuScreen.SetActive(false);
    }
    public void GoMenuFromOptions()
    {
        SoundManager.PlayButtonClickSound();
        optionsScreen.SetActive(false);
        menuScreen.SetActive(true);
    }
    public void QuitGame()
    {
        SoundManager.PlayButtonClickSound();
        Application.Quit();
    }
}
