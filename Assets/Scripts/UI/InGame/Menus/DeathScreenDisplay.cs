using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class DeathScreenDisplay : MonoBehaviour
{
    private ICollection<Transform> componentsList;
    public GameplayManager gameplayManager;

    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject damageStats;
    [SerializeField] private GameObject otherStats;

    public void Awake()
    {
        gameplayManager = GameplayManager.Instance;
        componentsList = transform.GetComponentsInChildren<Transform>();
    }

    public static void Create()
    {
       Canvas parent = FindObjectOfType<Canvas>();
       Transform deathScreenDisplayTransfrom = Instantiate(GameAssets.Instance.pfDeathScreen, Vector3.zero, Quaternion.identity, parent.transform);
       deathScreenDisplayTransfrom.localPosition = new(0, 0, 0);

       DeathScreenDisplay deathScreenDisplay = deathScreenDisplayTransfrom.GetComponent<DeathScreenDisplay>();
       deathScreenDisplay.Setup();
    }

    public void RestartGame()
    {
        SoundManager.PlayButtonClickSound();

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
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
        foreach (var child in componentsList)
        {
            //DAMAGE STATS
            if (child.name == "knife")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.KnifeDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedKnifeDamage.ToString();
            }
            if (child.name == "sword")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.SwordDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedSwordDamage.ToString();
            }
            if (child.name == "tomahawk")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.TomahawkDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedTomahawkDamage.ToString();
            }
            if (child.name == "axe")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.AxeDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedAxeDamage.ToString();
            }
            if (child.name == "ice_wand")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.IceWandDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedIceWandDamage.ToString();
            }
            if (child.name == "fire_wand")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.FireWandDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedFireWandDamage.ToString();
            }
            if (child.name == "earth_wand")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EarthWandDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedEarthWandDamage.ToString();
            }
            if (child.name == "wind_wand")
            {
                child.Find("normaldamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.WindWandDamage.ToString();
                child.Find("evolveddamage").GetComponent<TextMeshProUGUI>().text = gameplayManager.EvolvedWindWandDamage.ToString();
            }


            //OTHER STATS
            if (child.name == "ovadamage")
                child.Find("statvalue").GetComponent<TextMeshProUGUI>().text = gameplayManager.OverallDamage.ToString();
            if (child.name == "damagetaken")
                child.Find("statvalue").GetComponent<TextMeshProUGUI>().text = gameplayManager.DamageTaken.ToString();
            if (child.name == "healing")
                child.Find("statvalue").GetComponent<TextMeshProUGUI>().text = gameplayManager.HealingDone.ToString();
            if (child.name == "enemieskilled")
                child.Find("statvalue").GetComponent<TextMeshProUGUI>().text = gameplayManager.EnemiesKilled.ToString();
            if (child.name == "expgained")
                child.Find("statvalue").GetComponent<TextMeshProUGUI>().text = gameplayManager.ExperienceGained.ToString();
            if (child.name == "lvlreached")
                child.Find("statvalue").GetComponent<TextMeshProUGUI>().text = gameplayManager.LevelReached.ToString();
            if (child.name == "timealive")
                child.Find("statvalue").GetComponent<TextMeshProUGUI>().text = gameplayManager.TimeAlive.ToString();
        }
    }

    public void HideDeathScreen (bool toggleVisible)
    {
        gameOver.SetActive(toggleVisible);
        restartButton.SetActive(toggleVisible);
        menuButton.SetActive(toggleVisible);
        quitButton.SetActive(toggleVisible);
        damageStats.SetActive(toggleVisible);
        otherStats.SetActive(toggleVisible);
    }
}