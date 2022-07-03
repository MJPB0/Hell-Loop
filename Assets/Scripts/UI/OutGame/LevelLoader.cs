using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject menuScreen;
    public Slider slider;
    public TextMeshProUGUI loadingText;
    public void LoadLevel (int sceneIndex)
    {
        loadingScreen.SetActive(true);
        menuScreen.SetActive(false);
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            loadingText.text = Mathf.Round(progress * 100f) + "%";
            yield return null;
        }
    }

}
