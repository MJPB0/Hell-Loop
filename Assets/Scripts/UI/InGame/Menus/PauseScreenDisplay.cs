using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseScreenDisplay : MonoBehaviour
{
    public static bool IsPaused = false;

    public static PauseScreenDisplay Create()
    {
        Canvas parent = FindObjectOfType<Canvas>();
        Transform pauseScreenDisplayTransform = Instantiate(GameAssets.Instance.pfPauseScreen, Vector3.zero, Quaternion.identity, parent.transform);
        pauseScreenDisplayTransform.localPosition = Vector3.zero;
        
        PauseScreenDisplay pauseScreenDisplay = pauseScreenDisplayTransform.GetComponent<PauseScreenDisplay>();
        pauseScreenDisplay.Setup();

        return pauseScreenDisplay;
    }
    public void ResumeGame()
    {
        SoundManager.PlayButtonClickSound();

        GameplayManager.Instance.ResumeGame();
        IsPaused = false;

        Destroy(gameObject);
    }
    public void GoToMenu()
    {
        SoundManager.PlayButtonClickSound();

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void QuitGame()
    {
        SoundManager.PlayButtonClickSound();

        Application.Quit();
    }
    public void Setup()
    {
        IsPaused = true;
        GameplayManager.Instance.PauseGame();
    }
}
